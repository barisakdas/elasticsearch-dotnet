namespace Elasticsearch.Core.Models;

/// <summary>Dışarıdan bir entity oluşturulacağı zaman gönderilmesini istediğimiz zorunlu alanları içeren modeli oluşturuyoruz.</summary>
public record CreateAuthorModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
}

/// <summary>Dışarıdan bir entity güncelleneceği zaman gönderilmesini istediğimiz zorunlu alanları içeren modeli oluşturuyoruz.</summary>
public record UpdateAuthorModel
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
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