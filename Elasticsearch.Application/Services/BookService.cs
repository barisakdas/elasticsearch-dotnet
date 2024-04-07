﻿using Elastic.Clients.Elasticsearch;

namespace Elasticsearch.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;

    private const string IndexName = "books";

    public BookService(IBookRepository repository, IMapper mapper)
       => (_repository, _mapper) = (repository, mapper);

    /// <summary>Veri tabanında aktif olarak bulunan verilerin gerekli işlemleri yapılarak ön yüze toplu şekilde gönderilmesini sağlayan metot.</summary>
    public async Task<BaseResult<List<BookDto>>> GetAllAsync()
    {
        // ElasticClient üzerinden ilgili index içerisine istek yapıyoruz.
        var (result, message) = await _repository.MatchAllQueryAsync(IndexName);

        if (result is null)
            return new NoContentResult<List<BookDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<BookDto>>(result);

        return new SuccessfullResult<List<BookDto>>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan verilerin gerekli işlemleri yapılarak ön yüze gönderilmesini sağlayan metot.</summary>
    public async Task<BaseResult<BookDto>> GetByIdAsync(string id)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (string.IsNullOrWhiteSpace(id))
            return new BadRequestResult<BookDto>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.GetAsync(id, IndexName);

        if (result is null)
            return new NoContentResult<BookDto>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<BookDto>(result);

        return new SuccessfullResult<BookDto>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isme ait veriyi getiren sağlayan metot.</summary>
    public async Task<BaseResult<List<BookDto>>> GetByNameAsync(string name)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (string.IsNullOrWhiteSpace(name))
            return new BadRequestResult<List<BookDto>>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.TermQueryAsync(IndexName, "name", name);

        if (result is null)
            return new NoContentResult<List<BookDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<BookDto>>(result);

        return new SuccessfullResult<List<BookDto>>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili isimlere ait veriyi getiren sağlayan metot.</summary>
    public async Task<BaseResult<List<BookDto>>> GetByNameListAsync(List<string> names)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (names is null)
            return new BadRequestResult<List<BookDto>>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.TermsQueryAsync(IndexName, "name", names);

        if (result is null)
            return new NoContentResult<List<BookDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<BookDto>>(result);

        return new SuccessfullResult<List<BookDto>>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili basım tarihine ait veriyi getiren sağlayan metot.</summary>
    public async Task<BaseResult<List<BookDto>>> GetByPublishDateAsync(DateTime publishDate)
    {
        // Elasticsearch üzerindeki veriyi alıyoruz.
        var (result, message) = await _repository.TermQueryAsync(IndexName, "publishDate", publishDate.ToString());

        if (result is null)
            return new NoContentResult<List<BookDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<BookDto>>(result);

        return new SuccessfullResult<List<BookDto>>(response);
    }

    /// <summary>Veri tabanında aktif olarak bulunan ve ilgili içeriğe sahip veriyi getiren sağlayan metot.
    /// Bu metot Elasticsearch üzerinde Full Text Search özelliğini kullanır.</summary>
    public async Task<BaseResult<List<BookDto>>> GetByAbstractContetAsync(string abstractContent)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (string.IsNullOrWhiteSpace(abstractContent))
            return new BadRequestResult<List<BookDto>>("Gelen id boş geçilemez!");

        // Elasticsearch üzerindeki veriyi alıyoruz. fuzinessCharCount özelliği kaç karakterde hata payı olmasını istediğimizi gösterir.
        var (result, message) = await _repository.FullTextMatchQueryAsync(
            indexName: IndexName,
            fieldName: "abstract",
            value: abstractContent,
            fuzinessCharCount: 1);

        if (result is null)
            return new NoContentResult<List<BookDto>>($"Veri alınamadı. Mesaj: {message}");

        var response = _mapper.Map<List<BookDto>>(result);

        return new SuccessfullResult<List<BookDto>>(response);
    }

    /// <summary>Son kullanıcı tarafından veri tabanına eklenmesini istediği metodun eklenme işlemlerini yapan metot.</summary>
    public async Task<BaseResult<BookDto>> InsertAsync(CreateBookModel model)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (model is null)
            return new BadRequestResult<BookDto>("Gelen model boş geçilemez!");

        // Dışarıdan alınan modeli AutoMapper yardımıyla veritabanına ekleyeceğimiz modele çeviriyoruz.
        var entity = _mapper.Map<Book>(model);

        // Elasticsearch üzerine elimizdeki veriyi indexleme işlemi yapıyoruz.
        var (result, message) = await _repository.IndexAsync(entity, IndexName);

        // ElasticClient tarafından gönderilen veride başarısızlık olmuşsa bunu IsValid özelliğinden okuyarak geri dönüş yapıyoruz.
        if (result is default(Book))
            return new BadRequestResult<BookDto>($"Indexleme işlemi başarısız oldu. Mesaj: {message}");

        // Başarılı işlem sonrasında oluşan veriyi dönüş modeline çeviriyoruz.
        var response = _mapper.Map<BookDto>(entity);

        // Client tarafına başarılı modelimizi dönüyoruz.
        return new SuccessfullResult<BookDto>(response);
    }

    /// <summary>Son kullanıcı tarafından veri tabanında güncellenmesini istediği metodun işlemlerini yapan metot.</summary>
    public async Task<BaseResult<BookDto>> UpdateAsync(UpdateBookModel model)
    {
        // İlk olarak dışarıdan gelen modelin boş olup olmadığına bakıyoruz.
        // Böylece gelen model boşşsa herhangi bir işlem yapmadan hızlıca metodu kırabiliriz.
        if (model is null)
            return new BadRequestResult<BookDto>("Gelen model boş geçilemez!");

        // Dışarıdan alınan modeli AutoMapper yardımıyla veritabanına ekleyeceğimiz modele çeviriyoruz.
        var entity = _mapper.Map<Book>(model);

        // Elasticsearch üzerine elimizdeki veriyi güncelleme işlemi yapıyoruz.
        var (result, message) = await _repository.UpdateAsync(entity, IndexName);

        // ElasticClient tarafından gönderilen veride başarısızlık olmuşsa bunu IsValid özelliğinden okuyarak geri dönüş yapıyoruz.
        if (result is default(Book))
            return new BadRequestResult<BookDto>($"Güncelleme işlemi başarısız oldu. Mesaj: {message}");

        // Başarılı işlem sonrasında oluşan veriyi dönüş modeline çeviriyoruz.
        var response = _mapper.Map<BookDto>(entity);

        // Client tarafına başarılı modelimizi dönüyoruz.
        return new SuccessfullResult<BookDto>(response);
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