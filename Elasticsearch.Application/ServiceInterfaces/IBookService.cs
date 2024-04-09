namespace Elasticsearch.Application.ServiceInterfaces;

public interface IBookService
{
    /// <summary>Veri tabanında aktif olarak bulunan verilerin gerekli işlemleri yapılarak ön yüze toplu şekilde gönderilmesini sağlayan metot.</summary>
    Task<BaseResult<List<BookDto>>> GetAllAsync();

    /// <summary>Ekrandan gelen parametre ile Makalelerin `Title` ve `Abstract` alanlarında full text search araması yapmak istiyoruz.
    /// Bunun için repositorymize özel olarak bir sorgu hazırladık. Bu sorgu makaleleri ararken elasticsearch veritabanının Full Text Search özelliğini kullanacak.
    /// Arka planda(repository içinde) komplex ve birleşik bir sorgu yazacağız.</summary>
    Task<BaseResult<List<BookDto>>> GetFilterAsync(string searchText);

    /// <summary>Ekrandan gelen parametreler ile Makalelerin ilgili tüm alanlarında filtreleme yapmamızı sağlayacak olan metot..</summary>
    Task<BaseResult<List<BookDto>>> GetFilterAsync(SearchBookModel model);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili id ye ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<BookDto>> GetByIdAsync(string id);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isme ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<List<BookDto>>> GetByTitleAsync(string title);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isimlere ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<List<BookDto>>> GetByTitleListAsync(List<string> titles);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili basım tarihine ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<List<BookDto>>> GetByPublishDateAsync(DateTime publishDate);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili içeriğe sahip veriyi getiren sağlayan metot.
    /// Bu metot Elasticsearch üzerinde Full Text Search özelliğini kullanır.</summary>
    Task<BaseResult<List<BookDto>>> GetByAbstractContetAsync(string abstractContent);

    /// <summary>Son kullanıcı tarafından veri tabanına eklenmesini istediği metodun eklenme işlemlerini yapan metot.</summary>
    Task<BaseResult<BookDto>> InsertAsync(CreateBookModel model);

    /// <summary>Son kullanıcı tarafından veri tabanında güncellenmesini istediği metodun işlemlerini yapan metot.</summary>
    Task<BaseResult<BookDto>> UpdateAsync(UpdateBookModel model);

    /// <summary>Son kullancı tarafından veri tabanından silinmesini istediği metodun işlemlerini yapan metot. Burada veri kaybını önlemek için veriyi silmiyoruz. Onun yerine aktif olmayacak şekilde işliyoruz.</summary>
    Task<BaseResult<bool>> DeleteAsync(string id);
}