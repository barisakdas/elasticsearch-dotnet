namespace Elasticsearch.Core.Entities;

public class Author : BaseEntity
{
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; } = null!;

    [JsonPropertyName("lastname")]
    public string LastName { get; set; } = null!;

    [JsonPropertyName("birdthdata")]
    public DateTime BirthDate { get; set; }

    /* NESTED ENTITIES */

    /// <summary>Yazara ait kitapların eklenebilmesi için çoka çok yapıyı kuruyoruz. Bir kitabın birden fazla yazarı olabilir.</summary>
    [JsonPropertyName("books")]
    public List<Book> Books { get; set; } = new List<Book>();

    /// <summary>İstenilen değererli alan ve kalanını kullanmayan bir genel yapıcı metot.</summary>
    public Author(DateTime birthDate, string firstName = null, string lastName = null)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }
}