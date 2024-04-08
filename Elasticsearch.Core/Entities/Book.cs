namespace Elasticsearch.Core.Entities;

public class Book : BaseEntity
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("abstract")]
    public string Abstract { get; set; } = null!;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("stock")]
    public uint Stock { get; set; }

    [JsonPropertyName("publishdate")]
    public DateTime PublishDate { get; set; }

    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; }

    /* NESTED ENTITIES */

    /// <summary>Kitabın yazar bilgisi.</summary>
    [JsonPropertyName("author")]
    public Author Author { get; set; } = null!;

    /// <summary>İstenilen değererli alan ve kalanını kullanmayan bir genel yapıcı metot.</summary>
    public Book(string title, string @abstract, decimal price, uint stock)
    {
        Title = title;
        Abstract = @abstract;
        Price = price;
        Stock = stock;
    }
}