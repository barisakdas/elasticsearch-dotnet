namespace Elasticsearch.Infrastructure.RepositoryInterfaces;

public interface IBookRepository : BaseRepositoryInterfaces.IRepository<Book>
{
    /// <summary>Ekrandan gelen parametre ile Makalelerin `Title` ve `Abstract` alanlarında full text search araması yapmak istiyoruz.
    /// Bunun için repositorymize özel olarak bir sorgu hazırladık. Bu sorgu makaleleri ararken elasticsearch veritabanının Full Text Search özelliğini kullanacak.</summary>
    Task<(IEnumerable<Book> data, string message)> GetFilterAsync(string indexName, string searchText);

    /// <summary>Belirtilen arama modeline göre kitapları filtreleyerek getirir.</summary>
    /// <param name="indexName">Elasticsearch index adı.</param>
    /// <param name="model">Arama kriterlerini içeren model.</param>
    /// <returns>Filtrelenmiş kitapların listesi ve işlem mesajı içeren bir tuple döner.</returns>
    Task<(IEnumerable<Book> data, string message)> GetFilterAsync(string indexName, SearchBookModel model);
}