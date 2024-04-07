namespace Elasticsearch.Core.Dtos;

public record BookDto : BaseDto
{
    public string Title { get; set; } = null!;
    public string Abstract { get; set; } = null!;
    public decimal Price { get; set; }
    public uint Stock { get; set; }
    public DateTime PublishDate { get; set; }
    public List<string> Categories { get; set; }

    /* NESTED ENTITIES */
    public AuthorDto Author { get; set; } = null!;
}