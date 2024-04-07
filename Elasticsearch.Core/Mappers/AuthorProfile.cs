namespace Elasticsearch.Core.Mappers;

/// <summary>Profile sınıfı oluşturulan classlar arasında eşleme işlemi yapar.</summary>
public class AuthorProfile : AutoMapper.Profile   // AutoMapper dan gelen Profile sınıfını miras alıyoruz.
{
    // Yapıcı metot içerisinde hangi sınıflardan hangi sınıflara eşleme işlemi yapacağımızı belirtiyoruz.
    public AuthorProfile()
    {
        // Author entity'si ile AuthorDto geri dönüş modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<Author, AuthorDto>().ReverseMap();

        // Author entity'si ile CreateAuthorModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<CreateAuthorModel, Author>().ReverseMap();

        // AuthorDto geri dönüş modeli ile CreateAuthorModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<CreateAuthorModel, AuthorDto>().ReverseMap();

        // Author entity'si ile CreateAuthorModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<UpdateAuthorModel, Author>().ReverseMap();

        // AuthorDto geri dönüş modeli ile CreateAuthorModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<UpdateAuthorModel, AuthorDto>().ReverseMap();
    }
}