namespace Elasticsearch.Core.Dtos;

public record AuthorDto : BaseDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }

    /* NESTED ENTITIES */
    public List<BookDto> Books { get; set; } = new List<BookDto>();
}