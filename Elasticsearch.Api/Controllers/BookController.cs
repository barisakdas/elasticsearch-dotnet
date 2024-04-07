namespace Elasticsearch.Api.Controllers;

/// <summary>Endpoinleri oluşturduğumuz yapı.</summary>
[Produces("application/json")]
[Route("v1/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    // İlgili servisi içeriye alıyoruz.
    private readonly IBookService _service;

    // Servisi DI container içerisinde dahil ediyoruz.
    public BookController(IBookService service)
       => _service = service;

    [HttpGet("getall")] // Yapılan işlemin endpointinin nasıl görüneceğini burada `getall` diyerek belirtiyoruz.
    public async Task<IActionResult> GetAllAsync()
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetAllAsync();

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetByIdAsync(id);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpGet("getbyname")]
    public async Task<IActionResult> GetByTitleAsync(string title)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetByTitleAsync(title);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpPost("getbynamelist")]
    public async Task<IActionResult> GetByTitleListAsync(List<string> titles)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetByTitleListAsync(titles);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpGet("getbypublishdate")]
    public async Task<IActionResult> GetByNameAsync(DateTime publishDate)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetByPublishDateAsync(publishDate);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpPost("insert")]
    public async Task<IActionResult> InsertAsync(CreateBookModel model)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.InsertAsync(model);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync(UpdateBookModel model)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.UpdateAsync(model);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.DeleteAsync(id);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }
}