namespace Elasticsearch.Infrastructure.Repositories;

public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
{
    public AuthorRepository(ElasticsearchClient client) : base(client)
    {
    }
}