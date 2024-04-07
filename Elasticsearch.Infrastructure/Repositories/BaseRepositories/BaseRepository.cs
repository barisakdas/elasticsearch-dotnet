namespace Elasticsearch.Infrastructure.Repositories.BaseRepositories;

/// <summary>BaseRepository ifadesi, .NET’te genel bir depo deseni uygulamasının bir parçasıdır.
/// Bu yapı, veritabanı işlemlerini soyutlayarak, farklı veri türleri için ayrı depolar oluşturma ihtiyacını ortadan kaldırır.
/// BaseRepository<T> sınıfı, belirli bir veri türü (T) için CRUD (Oluştur, Oku, Güncelle, Sil) işlemlerini gerçekleştiren yöntemler sağlar.
/// Bu, kod tekrarını azaltır ve veri erişim mantığını merkezileştirir.</summary>
/// <typeparam name="T">Generic olarak alınan sınıf. Bu sınıf mutlaka BaseEntity'den kalıtım almak zoruda olmasa da BaseRepository tanımında o şekilde işaretlenmiştir.</typeparam>
public class BaseRepository<T> : RepositoryInterfaces.BaseRepositoryInterfaces.IRepository<T>
     where T : BaseEntity
{
    // ElasticsearchClient, Elasticsearch veritabanıyla etkileşim kurmak için kullanılan .NET tabanlı bir kütüphanedir.
    // Elasticsearch, büyük veri setlerini hızlı bir şekilde aramak ve analiz etmek için kullanılan popüler bir açık kaynak arama motorudur.
    // ElasticsearchClient, bu işlevselliği .NET uygulamalarınıza entegre etmenizi sağlar. Örneğin, belgeleri indeksleme, arama sorguları oluşturma,
    // veri güncelleme ve silme gibi işlemleri kolaylaştırır.
    // .NET 8 ile ilgili olarak, Elasticsearch .NET Client’ın 8.x sürümleri, .NET Core, .NET 5+ ve .NET Framework (4.6.1 ve üzeri) uygulamalarında
    // kullanılmak üzere NuGet paketi olarak mevcuttur. Bu kütüphane, REST API ile birebir eşleme, Elasticsearch API’leri için güçlü tip denetimi,
    // sorgu oluşturma için Fluent API ve belge indeksleme gibi ortak görevler için yardımcılar sunar1.
    // .NET 8’in kendine has özellikleri arasında performans iyileştirmeleri, çöp toplama ve çekirdek ile uzantı kitaplıklarına yönelik geliştirmeler bulunmaktadır.
    // Ayrıca, mobil uygulamalar ve yeni kaynak oluşturucular için com birlikte çalışma ve yapılandırma bağlaması gibi yeni özellikler içerir
    private readonly ElasticsearchClient _client;

    public BaseRepository(ElasticsearchClient client)
        => _client = client;

    #region [STRUCTURED_QUERYS]

    /// <summary>Elasticsearch üzerinde arama yaparken veriyi id değeri ile çekmemizi sağlayan metot.</summary>
    /// <param name="id">Tek bir veri almamız için gereken tekil bilgi.</param>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    public async Task<(T? data, string? message)> GetAsync(string id, string indexName)
    {
        // ElasticClient üzerinden tek bir veri alacağımız zaman GetAsync metodunu kullanmamız yeterli olacaktır.
        var result = await _client.GetAsync<T>(indexName, id);

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception? exception);
            return (default(T), exception?.Message);
        }

        result.Source.Id = result.Id;

        // Gelen verinin içerisindeki Source alanı bize istediğimiz modeli verecektir.
        return (result.Source, string.Empty);
    }

    /// <summary>TermQuery, Elasticsearch’de belirli bir alanda kesin bir terimi içeren belgeleri döndüren bir sorgu türüdür.
    /// Bu sorgu, kesin bir değer aramak için kullanılır, örneğin bir fiyat, bir ürün kimliği veya bir kullanıcı adı gibi.
    /// TermQuery, metin alanları için değil, kesin değerler gerektiren alanlar için idealdir.
    /// Metin alanlarının değerleri analiz sürecinde değiştiği için, bu tür alanlarda kesin eşleşmeler bulmak zor olabilir.
    /// Metin alanlarında arama yapmak için match query kullanılması önerilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> TermQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1)
    {
        // Elasticsearch üzerinde arayacağımız veri için term query'mizi oluşturuyoruz.
        var termQuery = new TermQuery(fieldName.Suffix(".keyword").ToString())
        {
            Value = value,

            // Büyük/küçük harf duyarlılığı olacak mı?
            // Bunu true olarak atama yaparsak duyarlılığı kaldırmış oluruz.
            CaseInsensitive = true
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(termQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>TermsQuery, Elasticsearch’de belirli bir alanda bir veya daha fazla kesin terimi içeren belgeleri döndüren bir sorgu türüdür.
    /// TermQuery ile benzerdir, ancak birden fazla değer arayabilirsiniz. Bir belge, en az bir terimi içeriyorsa eşleşir.
    /// Birden fazla eşleşen terim içeren belgeleri aramak için terms_set sorgusunu kullanabilirsiniz</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> TermsQueryAsync(string indexName, string fieldName, List<string> values, int pageSize = 10, int page = 1)
    {
        // TermsQuery içerisine ekleyeceğimiz filtre değerleri için bir collection oluşturuyoruz.

        var terms = new List<FieldValue>();

        // Parametre olarak alınan değerleri oluşturduğumuz collection içerisine ekliyoruz.
        values.ForEach(value => { terms.Add(value); });

        var termsQuery = new TermsQuery()
        {
            Field = fieldName.Suffix(".keyword").ToString(),
            Terms = new(terms.AsReadOnly())
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(termsQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>PrefixQuery, Elasticsearch’de belirli bir alanda belirli bir ön ek (prefix) ile başlayan terimleri içeren belgeleri döndüren bir sorgu türüdür.
    /// Bu sorgu, özellikle kullanıcıların bir metin alanında belirli bir karakter serisiyle başlayan belgeleri bulmaları gerektiğinde kullanışlıdır.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> PrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1)
    {
        var prefixQuery = new PrefixQuery(
            new Field(fieldName.Suffix(".keyword").ToString()))
        {
            Field = fieldName,
            Value = value
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(prefixQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>DateRangeQuery, Elasticsearch’de belirli bir tarih aralığında olan belgeleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, belirli bir alan için belirlenen başlangıç ve bitiş tarihleri arasındaki değerleri içeren belgeleri döndürür.
    /// rneğin, son bir yıl içinde oluşturulan belgeleri veya belirli bir tarih aralığındaki olayları bulmak için kullanılabilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="startValue">Aranacak özellikteki başlangıç tarih değeri.</param>
    /// <param name="endValue">Aranacak özellikteki bitiş tarih değeri.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> DateRangeQueryAsync(string indexName, string fieldName, DateTime startValue, DateTime endValue, int pageSize = 10, int page = 1)
    {
        var dateRangeQuery = new DateRangeQuery(
                new Field(fieldName))
        {
            Gte = startValue,
            Lt = endValue,
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(dateRangeQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>NumberRangeQuery, Elasticsearch’de sayısal bir alan üzerinde belirli bir aralıkta olan değerleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, belirli bir sayısal alan için belirlenen minimum ve maksimum değerler arasındaki değerleri içeren belgeleri döndürür.
    /// Örneğin, belirli bir fiyat aralığındaki ürünleri veya belirli bir puan aralığındaki değerlendirmeleri bulmak için kullanılabilir</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="startValue">Aranacak özellikteki başlangıç sayı değeri.</param>
    /// <param name="endValue">Aranacak özellikteki bitiş sayı değeri.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> NumberRangeQueryAsync(string indexName, string fieldName, double startValue, double endValue, int pageSize = 10, int page = 1)
    {
        var numberRangeQuery = new NumberRangeQuery(
                new Field(fieldName))
        {
            From = startValue,
            To = endValue
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(numberRangeQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>WildcardQuery, Elasticsearch’de belirli bir desene uyan terimleri içeren belgeleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, joker karakterler (* ve ?) kullanarak, belirli bir alanda arama yapmanızı sağlar* karakteri sıfır veya daha fazla karakterle
    /// eşleşirken, ? karakteri yalnızca tek bir karakterle eşleşir.
    /// Örneğin: App?e => 5 karakter ve geriye Apple döndürebilir.
    /// Örneğin: App*e => en az 5 karakter ve geriye Apple, Appilacate gibi değerleri döndürebilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> WildcardQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1)
    {
        var wildcardQuery = new WildcardQuery(
            new Field(fieldName.Suffix(".keyword").ToString()))
        {
            Field = fieldName,
            Value = value,
            CaseInsensitive = true
        };

        var result = await _client
           .SearchAsync<T>(
           s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                 .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                 .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                 .Query(wildcardQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>FuzzyQuery, Elasticsearch’de benzer terimleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, arama terimi ile belirli bir düzenleme mesafesi içinde tüm olası varyasyonları veya genişlemeleri oluşturur ve her genişleme için kesin eşleşmeleri döndürür.
    /// Düzenleme mesafesi, bir terimi diğerine dönüştürmek için gerekli olan tek karakter değişikliklerinin sayısını ifade eder.
    /// Bu değişiklikler, bir karakteri değiştirme, bir karakteri çıkarma, bir karakter eklemek veya iki bitişik karakteri yer değiştirmek gibi işlemleri içerebilir
    /// Örneğin: Google verisini ararken hata payını 1 karakter verirsek Googla, Poogle gibi parametreler ile veri dönüşü sağlanırken Goopla gibi birden fazla hata alınan filtrelerde veri alınmaz.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="fuzinessCharCount">Parametre olarak alınan veride hata yapılmasına(eşleşmemesine) müsade edilecek karakter sayısı.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> FuzzyQueryAsync(string indexName, string fieldName, string value, int fuzinessCharCount, int pageSize = 10, int page = 1)
    {
        var fuzzyQuery = new FuzzyQuery(
            new Field(fieldName.Suffix(".keyword").ToString()))
        {
            Field = fieldName,
            Value = value,
            Fuzziness = new(fuzinessCharCount)
        };

        var result = await _client
           .SearchAsync<T>(
           s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                 .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                 .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                 .Query(fuzzyQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>Elasticsearch üzerinde bir verinin indexleme işlemini yapmasını sağlayan metot.
    /// Index işlemi yapılırken id yi kendimiz vermezsek Elasticsearch bir id üretir.
    /// Elasticsearc doğasında insert işlemi yoktur. Bunun yerine indexleme dediğimiz ve birden fazla işlemden oluşan bir boru hattı vardır.
    /// Bu hat içerisinde öncelikle alınan veri RAM'e yazılır. Daha sonrasında tokenizer işlemi başlar. Bu işlemde ek ayar verilmedikçe elasticsearch
    /// alınan veri içerisindeki karakterleri boşluk karakterine göre parçalar. Ve her bir karakteri kaydeder.
    /// Hemen ardından normalize işlemi başlar. Burada veri büyük/küçükk karakter uyumuna uysun diye tek bir formata çekilir.
    /// Eğer ayarlardan eklenmek istenirse kelimeler için aynı anlamlı kelimeler de eklenebilir.</summary>
    /// <param name="entity">Eklenecek olan veri.</param>
    /// <param name="indexName">Eklenecek index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    public async Task<(T data, string message)> IndexAsync(T entity, string indexName)
    {
        // Entity üzerine eklenme ile ilgili özellikleri ekliyoruz.
        // Best practice'lerde bunların bir interceptor yardımıyla eklenmesi daha doğrudur.
        entity.CreatedDate = DateTime.Now;
        entity.CreatedBy = 1111;

        // Elasticsearch üzerine elimizdeki veriyi indexleme işlemi yapıyoruz. Elasticsearch içerisinde insert işlemi yoktur.
        // Elasticsearc text olarak gönderilen datayı önce tokenizer işlemine daha sonra da normalizasyon işlemine tabi tutar.
        // Bu işlemler sonrasında ilgili indexlere kayıtlarını atar.
        // İndexleme metodu iki parametre alır.
        // 1- Gönderilecek model,
        // 2- Verinin yazılacağı index(diğer db ler için tabloya karşılık gelir.) adı.
        // İndex adı belirtirken id oluşturmasını Elasticsearch'un kafasına göre yapmasını istemiyorsak .Id() metodu ile bir atama yapabiliyoruz.
        var result = await _client.IndexAsync<T>(entity, indexName);

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception? exception);
            return (default(T), exception?.Message);
        }

        // Cevap olarak dönen verinin id'sini entity id olarak atıyoruz.
        entity.Id = result.Id;

        return (entity, string.Empty);
    }

    /// <summary>Elasticsearch üzerinde id yardımıyla bulunan bir verinin güncelleme işlemini yapmasını sağlayan metot.
    /// Bu metot çalışırken aynı index metodunda olduğu için Token ve Normalize işlemlerini yürütür.
    /// <param name="entity">Güncellenecek olan veri.</param>
    /// <param name="indexName">Güncellenecek index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    public async Task<(T data, string message)> UpdateAsync(T entity, string indexName)
    {
        // Entity üzerine eklenme ile ilgili özellikleri ekliyoruz.
        // Best practice'lerde bunların bir interceptor yardımıyla eklenmesi daha doğrudur.
        entity.UpdatedDate = DateTime.Now;
        entity.UpdatedBy = 2222;

        // Elasticsearch üzerine elimizdeki veriyi güncelleme işlemi yapıyoruz.
        // Elasticsearc text olarak gönderilen datayı önce tokenizer işlemine daha sonra da normalizasyon işlemine tabi tutar.
        // Bu işlemler sonrasında ilgili indexlere kayıtlarını atar.
        // İndexleme metodu iki parametre alır.
        // 1- Gönderilecek model,
        // 2- Verinin yazılacağı index(diğer db ler için tabloya karşılık gelir.) adı.
        var result = await _client.UpdateAsync<T, T>(indexName, entity.Id, x => x.Doc(entity));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception? exception);
            return (default(T), exception?.Message);
        }

        return (entity, string.Empty);
    }

    /// <summary>Elasticsearch üzerinde id yardımıyla bulunan bir verinin silinme işlemini yapmasını sağlayan metot.</summary>
    /// <param name="id">Silinecek veri için gereken tekil bilgi.</param>
    /// <param name="indexName">Güncellenecek index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    public async Task<(bool isSuccess, string message)> DeleteAsync(string id, string indexName)
    {
        // Elasticsearch üzerine elimizdeki veriyi silme işlemi yapıyoruz.
        var result = await _client.DeleteAsync(indexName, id);

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception? exception);
            return (false, exception?.Message);
        }

        return (true, string.Empty);
    }

    #endregion [STRUCTURED_QUERYS]

    #region [UNSTRUCTURED_QUERYS]

    /// <summary>MatchAllQuery, Elasticsearch’de en basit sorgu türlerinden biridir ve tüm belgeleri eşleştirerek, her birine varsayılan olarak
    /// 1.0 _score verir. Bu sorgu, özellikle bir indeksteki veya birden fazla indeksteki tüm belgeleri almak istediğinizde kullanışlıdır.
    /// Örneğin, filtreler uygulamak, toplulaştırmalar yapmak veya sıralama işlemleri gerçekleştirmek gibi operasyonlar için bu sorgu tercih edilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> MatchAllQueryAsync(string indexName, int pageSize = 10, int page = 1)
    {
        var matchAllQuery = new MatchAllQuery()
        {
        };

        // ElasticClient üzerinden ilgili index içerisine istek yapıyoruz.
        var result = await _client.SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(matchAllQuery));           // Yapacağımız işlem Query olacak ve içerisine filtre eklemeden tüm datayı almak için MatchAll kullanıyoruz.

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>MultiMatchAllQuery terimi, Elasticsearch’de doğrudan bir sorgu türü olarak tanımlanmamıştır.
    /// Ancak, kullanıcı muhtemelen multi_match sorgusunu ve match_all sorgusunu birleştirmiş olabilir.
    /// multi_match sorgusu, birden fazla alanda eş zamanlı arama yapmak için kullanılır ve match sorgusunun çoklu alan versiyonudurmatch_all sorgusu ise,
    /// bir indeksteki tüm belgeleri döndürmek için kullanılır ve herhangi bir arama kriteri gerektirmez.
    /// Eğer kullanıcı, tüm alanlarda belirli bir terimi arayan bir sorgu oluşturmak istiyorsa, multi_match sorgusunu * joker karakteri ile kullanarak
    /// tüm alanlarda arama yapabilir. </summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName1">Aramanın yapılacağı ilk özellik.</param>
    /// <param name="fieldName2">Aramanın yapılacağı ikinci özellik.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> MultiMatchAllQueryAsync(string indexName, string fieldName1, string fieldName2, string value, int pageSize = 10, int page = 1)
    {
        var matchAllQuery = new MultiMatchQuery()
        {
            Fields = new Field(fieldName1)
                    .And(new Field(fieldName2)),
            Query = value
        };

        // ElasticClient üzerinden ilgili index içerisine istek yapıyoruz.
        var result = await _client.SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(matchAllQuery));           // Yapacağımız işlem Query olacak ve içerisine filtre eklemeden tüm datayı almak için MatchAll kullanıyoruz.

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);

            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>match_bool_prefix sorgusu, Elasticsearch’de kullanılan bir sorgu türüdür.
    /// Bu sorgu, girdiyi analiz eder ve terimlerden bir bool sorgusu oluşturur. Son terim hariç her terim bir term sorgusunda kullanılırken,
    /// son terim bir prefix sorgusunda kullanılır1. Örneğin, “hızlı kahverengi t” girdisi için, match_bool_prefix sorgusu, “hızlı”, “kahverengi”
    /// ve “t” terimlerini üretir ve bu terimlerden “hızlı” ve “kahverengi” için term sorguları, “t” için ise prefix sorgusu oluşturur.
    /// match_bool_prefix sorgusunun önemli bir özelliği, match_phrase_prefix sorgusundan farklı olarak, terimlerin bir ifade olarak değil,
    /// herhangi bir pozisyonda eşleşebilmesidir.Yani, “hızlı kahverengi tilki” ifadesiyle eşleşebileceği gibi, “kahverengi tilki hızlı” veya
    /// “hızlı” ve “kahverengi” terimlerini ve “f” ile başlayan bir terimi içeren herhangi bir pozisyondaki belgelerle de eşleşebilir.
    /// Bu sorgu türü, kullanıcıların metin girişini otomatik olarak tokenlere ayırmasını sağlar ve minimum_should_match ve operator parametrelerini
    /// destekler.Bu parametreler, oluşturulan bool sorgusuna uygulanır.fuzziness, prefix_length, max_expansions, fuzzy_transpositions ve
    /// fuzzy_rewrite parametreleri, son terim hariç tüm terimler için oluşturulan term alt sorgularına uygulanabilir.
    /// Ancak, son terim için oluşturulan prefix sorgusuna etki etmezler</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> MatchBoolPrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1)
    {
        var matchAllQuery = new MatchBoolPrefixQuery(
            new Field(fieldName))
        {
            Query = value
        };

        // ElasticClient üzerinden ilgili index içerisine istek yapıyoruz.
        var result = await _client.SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(matchAllQuery));           // Yapacağımız işlem Query olacak ve içerisine filtre eklemeden tüm datayı almak için MatchAll kullanıyoruz.

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);

            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    public async Task<(List<T> data, string message)> MatchPhraseQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1)
    {
        // Elasticsearch üzerinde arayacağımız veri için term query'mizi oluşturuyoruz.
        var matchPhrasePrefixQuery = new MatchPhraseQuery(
            fieldName.Suffix(".keyword").ToString())
        {
            Query = value
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(matchPhrasePrefixQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>MatchPhrasePrefixQuery, Elasticsearch’de kullanılan bir sorgu türüdür.
    /// Bu sorgu, verilen metnin kelimelerini sağlanan sırayla içeren belgeleri döndürür. Sağlanan metnin son terimi bir ön ek (prefix) olarak ele alınır
    /// ve bu terimle başlayan herhangi bir kelimeyle eşleşir. Örneğin, “hızlı kahverengi t” metni için bu sorgu, “hızlı kahverengi tilki” veya
    /// “iki hızlı kahverengi fırça” değerine sahip bir mesajla eşleşir, ancak “tilki hızlı ve kahverengidir” ile eşleşmez1.
    /// MatchPhrasePrefixQuery kullanımının temel parametreleri şunlardır:
    ///     * query: Aranacak metni içerir.
    ///     * analyzer: Metni tokenlere dönüştürmek için kullanılan analizörü belirtir.Varsayılan olarak, alan için haritalanan indeks zamanı analizörü kullanılır.
    ///     * max_expansions: Sorgu değerinin son teriminin genişleyebileceği maksimum terim sayısını belirtir.Varsayılan olarak 50’dir.
    ///     * slop: Eşleşen tokenlar arasında izin verilen maksimum pozisyon sayısını belirtir.Varsayılan olarak 0’dır.
    /// Bu sorgu türü, özellikle arama otomasyonu için kullanıldığında bazen kafa karıştırıcı sonuçlar üretebilir. Daha iyi çözümler için completion suggester
    /// ve search_as_you_type alan tipi gibi diğer araçlar da mevcuttur</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> MatchPhrasePrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1)
    {
        // Elasticsearch üzerinde arayacağımız veri için term query'mizi oluşturuyoruz.
        var matchPhrasePrefixQuery = new MatchPhrasePrefixQuery(fieldName.Suffix(".keyword").ToString())
        {
            Query = value
        };

        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(matchPhrasePrefixQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /// <summary>Elasticsearch üzerinde bir field yardımıyla full text search kullanarak verilerin çekilmesi işlemini yapmasını sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="fuzinessCharCount">Parametre olarak alınan veride hata yapılmasına(eşleşmemesine) müsade edilecek karakter sayısı.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    public async Task<(List<T> data, string message)> FullTextMatchQueryAsync(string indexName, string fieldName, string value, int fuzinessCharCount, int pageSize = 10, int page = 1)
    {
        var matchQuery = new MatchQuery(
            new Field(fieldName))
        {
            Query = value,
            Fuzziness = new(fuzinessCharCount),
            Operator = Operator.Or,
        };

        var result = await _client
           .SearchAsync<T>(
           s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                 .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                 .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                 .Query(matchQuery));

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    #endregion [UNSTRUCTURED_QUERYS]

    #region [COMPOUND_QUERYS]

    /// <summary>CompoundQuery, Elasticsearch’de diğer bileşik veya yaprak (leaf) sorguları sarmalayarak, sonuçlarını ve skorlarını birleştirmek,
    /// davranışlarını değiştirmek veya sorgu ile filtre bağlamı arasında geçiş yapmak için kullanılan bir sorgu grubudur1.
    /// Bu sorgular, birden fazla sorgu koşulunu mantıksal operatörlerle (must, should, must_not, filter) birleştiren bool query, belirli bir
    /// sorguya uyan belgeleri döndürürken başka bir sorguya uyan belgelerin skorunu düşüren boosting query, başka bir sorguyu sarmalayıp filtre
    /// bağlamında çalıştıran ve tüm eşleşen belgelere sabit bir _score veren constant_score query, birden fazla sorguyu kabul edip herhangi bir
    /// sorgu koşuluna uyan belgeleri döndüren dis_max query ve ana sorgunun döndürdüğü skorları popülerlik, güncellik, mesafe veya özel algoritmalar
    /// gibi faktörleri hesaba katarak değiştiren function_score query gibi çeşitli sorguları içerir</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    /// <param name="mustFieldName">Karmaşık query içerisinde kesinlikle olması istenen MUST özelliği için kullanılacak özellik ismi.</param>
    /// <param name="mustFieldValue">Karmaşık query içerisinde kesinlikle olması istenen MUST özelliği için kullanılacak özellik değeri.</param>
    /// <param name="mustnotFieldName">Karmaşık query içerisinde kesinlikle olmaması istenen MUST_NOT özelliği için kullanılacak özellik ismi.</param>
    /// <param name="mustnotFieldValue">Karmaşık query içerisinde kesinlikle olmaması istenen MUST_NOT özelliği için kullanılacak özellik değeri.</param>
    /// <param name="shouldFieldName">Karmaşık query içerisinde olması durumunda önceliğe alınması istenen SHOULD özelliği için kullanılacak özellik ismi.</param>
    /// <param name="shouldFieldValue">Karmaşık query içerisinde olması durumunda önceliğe alınması istenen SHOULD özelliği için kullanılacak özellik değeri.</param>
    /// <param name="filterFieldName">Karmaşık query içerisinde verinin filtrelenmesi istenen FILTER özelliği için kullanılacak özellik ismi.</param>
    /// <param name="filterFieldValue">Karmaşık query içerisinde verinin filtrelenmesi istenen FILTER özelliği için kullanılacak özellik değeri.</param>
    public async Task<(List<T> data, string message)> CompoundQueryAsync(
        string indexName,
        string mustFieldName, string mustFieldValue,
        string mustnotFieldName, double mustnotFieldValue,
        string shouldFieldName, DateTime shouldFieldValue,
        string filterFieldName, string filterFieldValue,
        int pageSize = 10,
        int page = 1)
    {
        var result = await _client
            .SearchAsync<T>(
            s => s.Index(indexName)                 // Index bilgisi veriyoruz.
                  .Size(pageSize)                   // Gelmesini istediğimiz data miktarını veriyoruz. Default olarak Elasticsearch bize 10 tane data verir
                  .From((page - 1) * pageSize)      // Atlanmasını istediğimiz data miktarsını veriyoruz. Genelde sayfalama yapısı için kullanabiliriz.
                  .Query(q =>
                            q.Bool(b =>

                            #region Must

                                       b.Must(m => m
                                            .Term(t => t
                                                    .Field(mustFieldName
                                                        .Suffix(".keyword")
                                                        .ToString()!)
                                                    .Value(mustFieldValue)))

                            #endregion Must

                            #region Sould

                                       .Should(s => s
                                                .Range(t => t
                                                        .DateRange(dr => dr
                                                                    .Field(shouldFieldName)
                                                                    .From(shouldFieldValue))))

                            #endregion Sould

                            #region MustNot

                                       .MustNot(mn => mn
                                                  .Range(rn => rn
                                                            .NumberRange(nr => nr
                                                                            .Field(mustnotFieldName)
                                                                            .Lte(mustnotFieldValue))))

                            #endregion MustNot

                            #region Filter

                                       .Filter(f => f
                                                .Term(t => t
                                                        .Field(filterFieldName)
                                                        .Value(filterFieldValue))))));

        #endregion Filter

        if (result is null || !result.IsValidResponse)
        {
            result.TryGetOriginalException(out Exception exception);
            return (Enumerable.Empty<T>().ToList(), exception.Message);
        }

        // Not: Gelen veri kendi içerisinde id özelliğini taşımıyor. Bu id verinin `Hits` alanının içerisinde.
        // Hist alanının içerisindeki veriyi source içerisine taşırsak documents alanı da source içerisinden beslendiği için verilerimizin id özelliğini almış oluruz.
        foreach (var hit in result.Hits)
            hit.Source.Id = hit.Id;

        return (result.Documents.ToList(), string.Empty);
    }

    /* Elasticsearch Compound Query Nedir?

    Elasticsearch compound queries, birden fazla sorguyu birleştirmek, sonuçlarını ve skorlarını değiştirmek veya sorgu ile filtre bağlamı arasında
    geçiş yapmak için kullanılan sorgulardır. Bu tür sorgular, diğer bileşik veya yaprak sorguları kapsar ve genellikle daha karmaşık arama
    senaryolarında kullanılır.

    Elasticsearch compound queries’in temel tipleri şunlardır:
    * Bool Query:   Birden fazla yaprak veya bileşik sorgu ifadesini `must`, `should`, `must_not`, ya da `filter` klausülleri olarak birleştiren varsayılan sorgudur.
                    must ve should klausülleri skorları birleştirirken, must_not ve filter klausülleri filtre bağlamında çalıştırılır.

        - Must:     Bu klausül, belirtilen koşulların hepsinin karşılanması gerektiğini belirtir. Yani, bir must klausülü içeren sorgu, tüm must koşullarını
                    sağlayan belgeleri döndürür. Bu, mantıksal “VE” operatörüne benzer.
        - MustNot:  Bu klausül, belirtilen koşulların hiçbirinin karşılanmaması gerektiğini belirtir. Bir must_not klausülü içeren sorgu,
                    bu koşulları sağlamayan belgeleri döndürür. Bu, mantıksal “DEĞİL” operatörüne benzer.
        - Should:   Bu klausül, belirtilen koşullardan en az birinin karşılanmasını önerir. Ancak, should klausülü tek başına kullanıldığında,
                    koşullardan en az birini karşılayan belgeleri döndürür. Eğer should klausülü must ile birlikte kullanılırsa, should koşullarından
                    birini karşılayan belgeler ekstra skor alır, yani daha yüksek sıralamada yer alır.
        - Filter:   Bu klausül, belirtilen koşulları karşılayan belgeleri döndürür, ancak skorlama yapmaz. Yani, filter klausülü içeren sorgu,
                    koşulları sağlayan belgeleri döndürür, ancak bu belgelerin skorları hesaplanmaz veya değiştirilmez. Bu, genellikle performansı
                    artırmak için kullanılır çünkü skor hesaplaması yapılmadığından sorgular daha hızlı çalışır.

    Compound queries kullanımı, genellikle JSON tabanlı sorgu dili olan Query DSL (Domain Specific Language) ile yapılır.
    Bu sorgular, Elasticsearch’in RESTful API’si üzerinden gönderilir ve karmaşık arama ihtiyaçlarını karşılamak için genişletilebilirlik sağlar.
    Örneğin, bir e-ticaret sitesinde hem ürün adına hem de açıklamasına göre arama yapmak istediğinizde, bool sorgusu kullanarak her iki alanı da
    sorgulayabilir ve sonuçları birleştirebilirsiniz1.
    Elasticsearch compound queries, büyük veri setlerinde hızlı ve etkili arama, sıralama ve analiz işlemleri için tercih edilen güçlü araçlardır
    ve özellikle büyük ve karmaşık veri yapılarında değerli bilgiler elde etmek için kullanılır
     */

    #endregion [COMPOUND_QUERYS]
}