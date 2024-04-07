namespace Elasticsearch.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    private const string IndexName = "authors";

    public AuthorService(IAuthorRepository repository, IMapper mapper)
       => (_repository, _mapper) = (repository, mapper);

    /// <summary>Veri tabanında aktif olarak bulunan verilerin gerekli işlemleri yapılarak ön yüze toplu şekilde gönderilmesini sağlayan metot.</summary>
    public async Task<BaseResult<List<AuthorDto>>> GetAllAsync()
    {
        // ElasticClient üzerinden ilgili index içerisine istek yapıyoruz.
        var (result, message) = await _repository.MatchAllQueryAsync(IndexName);

        if (result is null)
            return new NoContentResult<List<AuthorDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<AuthorDto>>(result);

        return new SuccessfullResult<List<AuthorDto>>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili id ye ait veriyi getiren sağlayan metot.</summary>
    public async Task<BaseResult<AuthorDto>> GetByIdAsync(string id)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (string.IsNullOrWhiteSpace(id))
            return new BadRequestResult<AuthorDto>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.GetAsync(id, IndexName);

        if (result is null)
            return new NoContentResult<AuthorDto>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<AuthorDto>(result);

        return new SuccessfullResult<AuthorDto>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan verilerin gerekli işlemleri yapılarak ön yüze gönderilmesini sağlayan metot.</summary>
    public async Task<BaseResult<List<AuthorDto>>> GetByFirstNameAsync(string firstName)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (string.IsNullOrWhiteSpace(firstName))
            return new BadRequestResult<List<AuthorDto>>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.TermQueryAsync(IndexName, "firstname", firstName);

        if (result is null)
            return new NoContentResult<List<AuthorDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<AuthorDto>>(result);

        return new SuccessfullResult<List<AuthorDto>>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isimlere ait veriyi getiren sağlayan metot.</summary>
    public async Task<BaseResult<List<AuthorDto>>> GetByFirstNameListAsync(List<string> firstNames)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (firstNames is null)
            return new BadRequestResult<List<AuthorDto>>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.TermsQueryAsync(IndexName, "firstname", firstNames);

        if (result is null)
            return new NoContentResult<List<AuthorDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<AuthorDto>>(result);

        return new SuccessfullResult<List<AuthorDto>>(response);
    }

    /// <summary>Son kullanıcı tarafından veri tabanına eklenmesini istediği metodun eklenme işlemlerini yapan metot.</summary>
    public async Task<BaseResult<AuthorDto>> InsertAsync(CreateAuthorModel model)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (model is null)
            return new BadRequestResult<AuthorDto>("Gelen model boş geçilemez!");

        // Dışarıdan alınan modeli AutoMapper yardımıyla veritabanına ekleyeceğimiz modele çeviriyoruz.
        var entity = _mapper.Map<Author>(model);

        // Elasticsearch üzerine elimizdeki veriyi indexleme işlemi yapıyoruz.
        var (result, message) = await _repository.IndexAsync(entity, IndexName);

        // ElasticClient tarafından gönderilen veride başarısızlık olmuşsa bunu IsValid özelliğinden okuyarak geri dönüş yapıyoruz.
        if (result is default(Author))
            return new BadRequestResult<AuthorDto>($"Indexleme işlemi başarısız oldu. Mesaj: {message}");

        // Başarılı işlem sonrasında oluşan veriyi dönüş modeline çeviriyoruz.
        var response = _mapper.Map<AuthorDto>(entity);

        // Client tarafına başarılı modelimizi dönüyoruz.
        return new SuccessfullResult<AuthorDto>(response);
    }

    /// <summary>Son kullanıcı tarafından veri tabanında güncellenmesini istediği metodun işlemlerini yapan metot.</summary>
    public async Task<BaseResult<AuthorDto>> UpdateAsync(UpdateAuthorModel model)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (model is null)
            return new BadRequestResult<AuthorDto>("Gelen model boş geçilemez!");

        // Dışarıdan alınan modeli AutoMapper yardımıyla veritabanına ekleyeceğimiz modele çeviriyoruz.
        var entity = _mapper.Map<Author>(model);

        // Elasticsearch üzerine elimizdeki veriyi güncelleme işlemi yapıyoruz.
        var (result, message) = await _repository.UpdateAsync(entity, IndexName);

        // ElasticClient tarafından gönderilen veride başarısızlık olmuşsa bunu IsValid özelliğinden okuyarak geri dönüş yapıyoruz.
        if (result is default(Author))
            return new BadRequestResult<AuthorDto>($"Güncelleme işlemi başarısız oldu. Mesaj: {message}");

        // Başarılı işlem sonrasında oluşan veriyi dönüş modeline çeviriyoruz.
        var response = _mapper.Map<AuthorDto>(entity);

        // Client tarafına başarılı modelimizi dönüyoruz.
        return new SuccessfullResult<AuthorDto>(response);
    }

    /// <summary>Son kullancı tarafından veri tabanından silinmesini istediği metodun işlemlerini yapan metot.
    /// Burada veri kaybını önlemek için veriyi silmiyoruz. Onun yerine aktif olmayacak şekilde işliyoruz.</summary>
    public async Task<BaseResult<bool>> DeleteAsync(string id)
    {
        // Elasticsearch üzerine elimizdeki veriyi id yardımıyla silme işlemi yapıyoruz.
        var (isSuccess, message) = await _repository.DeleteAsync(id, IndexName);

        if (!isSuccess)
            return new BadRequestResult<bool>("Silme işlemi başarısız oldu.");

        return new SuccessfullResult<bool>(true);
    }
}