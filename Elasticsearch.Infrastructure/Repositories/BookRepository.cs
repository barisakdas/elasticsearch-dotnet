namespace Elasticsearch.Infrastructure.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(ElasticsearchClient client) : base(client)
    {
    }

    /// <summary>Ekrandan gelen parametre ile Makalelerin `Title` ve `Abstract` alanlarında full text search araması yapmak istiyoruz.
    /// Bunun için repositorymize özel olarak bir sorgu hazırladık. Bu sorgu makaleleri ararken elasticsearch veritabanının Full Text Search özelliğini kullanacak.</summary>
    public async Task<(IEnumerable<Book> data, string message)> GetFilterAsync(string indexName, string searchText)
    {
        // Not: Bu sorguda Title/Abstract alanında FullText search yerine MatchBoolPrefix kullanmak daha doğru bir yaklaşım olacaktır.
        // Çünkü; kullanıcı Title/Abstract içerisindeki tam kelimeyi yazmıyor olabilir.
        // Örneğin: makalemizin başlığı `Elasticsearch ve .Net8 Yenilikleri` olsun. Bu durumda kullanıcı aramaya `yenilik` ya da `elastic` yazarsa bu makalenin gelmesini istiyoruz.
        // Eğer ki burada Match kullanırsak bu metot arka planda direkt olarak gelen kelimelerle eş bir kelime arayacağından hiç bir veri getirmez.
        // Bu yüzden bu sorguda Title/Abstract alanı için `MatchBoolPrefix` metodunu kullanacağız.

        // Should içerisinde bir Match ifadesi yazıp hemen arkadasından başka bir sorguyu nokta koyarak yazarsak elasticsearch bunu `and` bağlacı ile bağlar.
        // Bunun önüne geçmek için Should metodunu birden fazla parametre ile beslemeliyiz.
        // Örneğin: ......Should(s => s.Match().Match())... => Bu durumda iki match metodu `and` bağlacına alınır. Bunun yerinne şu şekilde oluşturursak:
        // ......Should(s => s.Match(), s => s.Match()).... => Bu iki match metodu `or` metodu ile bağlanacaktır. Burada bağlaçların yönetimi veriyi doğru alabilmek adına önemlidir.
        var result = await _client
            .SearchAsync<Book>(
                s => s.Index(indexName)
                       .Query(q => q.Bool(
                                b => b.Should(
                                        s => s.MatchBoolPrefix(
                                                m => m.Field(
                                                        f => f.Abstract)
                                                       .Query(searchText)),
                                        s => s.MatchBoolPrefix(
                                                m => m.Field(
                                                        f => f.Title)
                                                       .Query(searchText))))));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<Book>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>Belirtilen arama modeline göre kitapları filtreleyerek getirir.</summary>
    /// <param name="indexName">Elasticsearch index adı.</param>
    /// <param name="model">Arama kriterlerini içeren model.</param>
    /// <returns>Filtrelenmiş kitapların listesi ve işlem mesajı içeren bir tuple döner.</returns>
    public async Task<(IEnumerable<Book> data, string message)> GetFilterAsync(string indexName, SearchBookModel model)
    {
        // Sorgu filtrelerimizi tutacak bir liste başlatıyoruz.
        List<Action<QueryDescriptor<Book>>> listQuery = new();

        // Eğer model nesnesi boş ise, tüm kitapları getirecek bir sorgu ekliyoruz.
        if (model is null)
        {
            listQuery.Add(
                g => g.MatchAll(
                    new MatchAllQuery()));

            // Filtre uygulama fonksiyonumuzu çağırıyoruz.
            return await ApplyFilter(indexName, listQuery, model.Page, model.PageSize);
        }

        // Modelde başlık varsa, başlığa göre bir filtre ekliyoruz.
        // Fuziness:2 dememizin sebebi 2 harfte hata yapmasına müsade etmek.
        if (!string.IsNullOrWhiteSpace(model.Title))
            listQuery.Add(
                q => q.MatchBoolPrefix(
                        m => m.Field(f => f.Title)
                              .Query(model.Title)
                              .Fuzziness(new Fuzziness(2))));

        // Modelde özet varsa, özete göre bir filtre ekliyoruz.
        // Fuziness:2 dememizin sebebi 2 harfte hata yapmasına müsade etmek.
        if (!string.IsNullOrWhiteSpace(model.Abstract))
            listQuery.Add(
                q => q.MatchBoolPrefix(
                        m => m.Field(f => f.Abstract)
                              .Query(model.Abstract)
                              .Fuzziness(new Fuzziness(2))));

        // Modelde minimum fiyat belirtilmişse, fiyata göre bir filtre ekliyoruz.
        if (model.MinPrice is not null & model.MinPrice is not default(double))
            listQuery.Add(
                q => q.Bool(
                        b => b.Filter(
                                f => f.Range(
                                        r => r.NumberRange(
                                                nr => nr.Field(f => f.Price)
                                                        .Gte(model.MinPrice))))));

        // Modelde minimum stok belirtilmişse, stoğa göre bir filtre ekliyoruz.
        if (model.MinStock is not null & model.MinStock is not default(uint))
            listQuery.Add(
                q => q.Bool(
                        b => b.Filter(
                                f => f.Range(
                                        r => r.NumberRange(
                                                nr => nr.Field(f => f.Stock)
                                                        .Gte(model.MinStock))))));

        // Modelde yayın tarihi başlangıcı belirtilmişse, tarihe göre bir filtre ekliyoruz.
        if (model.PublishDateStart is not null & model.PublishDateStart != DateTime.MinValue)
            listQuery.Add(
                q => q.Range(
                        r => r.DateRange(
                                dr => dr.Field(f => f.PublishDate)
                                        .Lte(model.PublishDateStart))));

        // Filtre uygulama fonksiyonumuzu çağırıyoruz.
        return await ApplyFilter(indexName, listQuery, model.Page, model.PageSize);
    }

    /// <summary>Belirtilen sorgu filtrelerini uygulayarak kitapları getirir.</summary>
    /// <param name="indexName">Elasticsearch index adı.</param>
    /// <param name="listQuery">Uygulanacak sorgu filtrelerinin listesi.</param>
    /// <param name="page">Sayfa numarası.</param>
    /// <param name="pageSize">Bir sayfada gösterilecek öğe sayısı.</param>
    /// <returns>Filtrelenmiş kitapların listesi ve işlem mesajı içeren bir tuple döner.</returns>
    private async Task<(IEnumerable<Book> data, string message)> ApplyFilter(string indexName, List<Action<QueryDescriptor<Book>>> listQuery, int page = 1, int pageSize = 10)
    {
        // Sayfalama için başlangıç noktasını hesaplıyoruz.
        var pageFrom = (page - 1) * pageSize;

        // Elasticsearch istemcisini kullanarak asenkron bir arama işlemi gerçekleştiriyoruz.
        var result = await _client.SearchAsync<Book>(
            s => s.Index(indexName)
                  .Size(pageSize)
                  .From(pageFrom)
                  .Query(
                    q => q.Bool(
                            b => b.Must(listQuery.ToArray()))));

        // Eğer sonuç null ise veya geçerli bir yanıt değilse, hata mesajı ile boş bir liste dönüyoruz.
        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<Book>().ToList(), exception.Message);
        }

        // Her bir sonucun ID'sini kaynağa ekliyoruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        // Sonuçları ve boş bir mesajı döndürüyoruz.
        return (data: result.Documents.AsEnumerable(), message: string.Empty);
    }
}