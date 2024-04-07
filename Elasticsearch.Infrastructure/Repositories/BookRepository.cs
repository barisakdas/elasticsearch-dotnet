namespace Elasticsearch.Infrastructure.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(ElasticsearchClient client) : base(client)
    {
    }
}