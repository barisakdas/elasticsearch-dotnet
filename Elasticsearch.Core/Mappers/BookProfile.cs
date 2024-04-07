namespace Elasticsearch.Core.Mappers;

/// <summary>Profile sınıfı oluşturulan classlar arasında eşleme işlemi yapar.</summary>
public class BookProfile : AutoMapper.Profile   // AutoMapper dan gelen Profile sınıfını miras alıyoruz.
{
    // Yapıcı metot içerisinde hangi sınıflardan hangi sınıflara eşleme işlemi yapacağımızı belirtiyoruz.
    public BookProfile()
    {
        // Book entity'si ile BookDto geri dönüş modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<Book, BookDto>().ReverseMap();

        // Book entity'si ile CreateBookModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<CreateBookModel, Book>().ReverseMap();

        // BookDto geri dönüş modeli ile CreateBookModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<CreateBookModel, BookDto>().ReverseMap();

        // Book entity'si ile CreateBookModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<UpdateBookModel, Book>().ReverseMap();

        // BookDto geri dönüş modeli ile CreateBookModel oluşturma modeli arasında ve tam tersi yöndede olmak üzere bir eşleme yapıyoruz.
        CreateMap<UpdateBookModel, BookDto>().ReverseMap();
    }
}