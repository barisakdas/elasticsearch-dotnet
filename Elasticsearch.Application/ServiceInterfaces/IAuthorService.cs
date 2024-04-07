namespace Elasticsearch.Application.ServiceInterfaces;

public interface IAuthorService
{
    /// <summary>Veri tabanında aktif olarak bulunan verilerin gerekli işlemleri yapılarak ön yüze toplu şekilde gönderilmesini sağlayan metot.</summary>
    Task<BaseResult<List<AuthorDto>>> GetAllAsync();

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili id ye ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<AuthorDto>> GetByIdAsync(string id);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isme ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<List<AuthorDto>>> GetByFirstNameAsync(string firstName);

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isimlere ait veriyi getiren sağlayan metot.</summary>
    Task<BaseResult<List<AuthorDto>>> GetByFirstNameListAsync(List<string> firstNames);

    /// <summary>Son kullanıcı tarafından veri tabanına eklenmesini istediği metodun eklenme işlemlerini yapan metot.</summary>
    Task<BaseResult<AuthorDto>> InsertAsync(CreateAuthorModel model);

    /// <summary>Son kullanıcı tarafından veri tabanında güncellenmesini istediği metodun işlemlerini yapan metot.</summary>
    Task<BaseResult<AuthorDto>> UpdateAsync(UpdateAuthorModel model);

    /// <summary>Son kullancı tarafından veri tabanından silinmesini istediği metodun işlemlerini yapan metot. Burada veri kaybını önlemek için veriyi silmiyoruz. Onun yerine aktif olmayacak şekilde işliyoruz.</summary>
    Task<BaseResult<bool>> DeleteAsync(string id);
}