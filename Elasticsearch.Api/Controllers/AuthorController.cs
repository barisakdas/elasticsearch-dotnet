namespace Elasticsearch.Api.Controllers;

/// <summary>Endpoinleri oluşturduğumuz yapı.
/// İlgili servisi içeriye alıyoruz.
/// Servisi DI container içerisinde dahil ediyoruz.</summary>
[Produces("application/json")]
[Route("v1/[controller]")]
[ApiController]
public class AuthorController(IAuthorService _service) : ControllerBase
{
    [HttpGet("getall")]
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

    [HttpGet("getbyfirstname")]
    public async Task<IActionResult> GetByFirstNameAsync(string firstName)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetByFirstNameAsync(firstName);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpPost("getbyfirstnamelist")]
    public async Task<IActionResult> GetByFirstNameListAsync(List<string> firstNames)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.GetByFirstNameListAsync(firstNames);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpPost("insert")]
    public async Task<IActionResult> InsertAsync(CreateAuthorModel model)
    {
        // Servis üzerinden veriyi alıyoruz. Alınan bu veri bize Result<T> şeklinde döneceği için bunun yapılandırmasına ihtiyacımız var.
        var result = await _service.InsertAsync(model);

        // Gelen verideki Result yapısının durumunu kontrol ederek ve ona uygun geri dönüş tipini (IActionResult) seçerek işlemi sonlandırıyoruz.
        return this.FromResult(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync(UpdateAuthorModel model)
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