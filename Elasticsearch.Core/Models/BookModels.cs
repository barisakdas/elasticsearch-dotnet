namespace Elasticsearch.Core.Models;

/// <summary>Dışarıdan bir entity oluşturulacağı zaman gönderilmesini istediğimiz zorunlu alanları içeren modeli oluşturuyoruz.</summary>
public record CreateBookModel
{
    public string Title { get; set; } = null!;
    public string Abstract { get; set; } = null!;
    public decimal Price { get; set; }
    public uint Stock { get; set; }
    public DateTime PublishDate { get; set; }
    public List<string> Categories { get; set; } = new();
}

/// <summary>Dışarıdan bir entity güncelleneceği zaman gönderilmesini istediğimiz zorunlu alanları içeren modeli oluşturuyoruz.</summary>
public record UpdateBookModel
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Abstract { get; set; } = null!;
    public decimal Price { get; set; }
    public uint Stock { get; set; }
    public DateTime PublishDate { get; set; }
    public List<string> Categories { get; set; } = new();
}

// <note-tr>
// Burada bir sınıfın oluşturulma ve güncellenmesi dışında kullanılacak tüm talep modellerinin tek bir .cs dosyasının içerisinde
// oluşturulması yöntemini kullanıyoruz. Böylece .cs fazlalaşmamış oluyor ve klasör/dosya kalabalığının da önüne geçmiş oluyoruz.
// Bu kullanıma `Vertical Slice Design Pattern` deniyor.
// </note-tr>

// <note-en>
// Here, we use the method of creating all demand models to be used in a single .cs file, except for creating and updating a class.
// In this way, .cs files do not become excessive and we avoid folder/file crowding. This usage is called 'Vertical Slice Design Pattern'.
// </note-en>