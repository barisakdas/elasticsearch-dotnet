namespace Elasticsearch.Infrastructure.RepositoryInterfaces.BaseRepositoryInterfaces;

/// <summary>IRepository ifadesi, .NET’te genel bir depo deseni uygulamasının bir parçasıdır.
/// Bu yapı, veritabanı işlemlerini soyutlayarak, farklı veri türleri için ayrı depolar oluşturma ihtiyacını ortadan kaldırır.
/// IRepository<T> sınıfı, belirli bir veri türü (T) için CRUD (Oluştur, Oku, Güncelle, Sil) işlemlerini gerçekleştiren yöntemler sağlar.
/// Bu, kod tekrarını azaltır ve veri erişim mantığını merkezileştirir.</summary>
/// <typeparam name="T">Generic olarak alınan sınıf. Bu sınıf mutlaka BaseEntity'den kalıtım almak zoruda olmasa da IRepository tanımında o şekilde işaretlenmiştir.</typeparam>
public interface IRepository<T>
    where T : class
{
    #region [STRUCTURED_QUERYS]

    /// <summary>Elasticsearch üzerinde arama yaparken herhangi bir filtre eklemeden tüm verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> MatchAllQueryAsync(string indexName, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken veriyi id değeri ile çekmemizi sağlayan metot.</summary>
    /// <param name="id">Tek bir veri almamız için gereken tekil bilgi.</param>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    Task<(T data, string message)> GetAsync(string id, string indexName);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir field ismi ve onda arayacağımız değeri ekleyerek verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> TermQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir field ismi ve onda arayacağımız değerleri ekleyerek verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> TermsQueryAsync(string indexName, string fieldName, List<string> values, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir field ismi ve onda arayacağımız değeri ekleyerek ve sonuna veya başına belli bir değer ekleyerek verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> PrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir tarih field ismi ve başlangıç-bitiş aralık değerleri ekleyerek verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="startValue">Aranacak özellikteki başlangıç tarih değeri.</param>
    /// <param name="endValue">Aranacak özellikteki bitiş tarih değeri.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> DateRangeQueryAsync(string indexName, string fieldName, DateTime startValue, DateTime endValue, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir sayı field ismi ve başlangıç-bitiş aralık değerleri ekleyerek verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="startValue">Aranacak özellikteki başlangıç sayı değeri.</param>
    /// <param name="endValue">Aranacak özellikteki bitiş sayı değeri.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> NumberRangeQueryAsync(string indexName, string fieldName, double startValue, double endValue, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir field ismi ve içerisinde saklı(bilinmeyen) karakter değerleri ekleyerek verileri çekmemizi sağlayan metot.
    /// Bu değerleri eklerken `?` sadece bir karakterin yerine geçerken `*` ise çoklu karakterin yerine kullanılır.
    /// Örneğin: App?e => 5 karakter ve geriye Apple döndürebilir.
    /// Örneğin: App*e => en az 5 karakter ve geriye Apple, Appilacate gibi değerleri döndürebilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> WildcardQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde arama yaparken filtre olarak bir field ismi ve içerisindeki değerde belli miktarda karakter değerleri için hata payı ekleyerek verileri çekmemizi sağlayan metot.
    /// Bu değerleri eklerken kelimede kaç karakter için yanlış yazılırsa veri geleceğini belirtmemiz gerekiyor.
    /// Örneğin: Google verisini ararken hata payını 1 karakter verirsek Googla, Poogle gibi parametreler ile veri dönüşü sağlanırken Goopla gibi birden fazla hata alınan filtrelerde veri alınmaz.
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="fuzinessCharCount">Parametre olarak alınan veride hata yapılmasına(eşleşmemesine) müsade edilecek karakter sayısı.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> FuzzyQueryAsync(string indexName, string fieldName, string value, int fuzinessCharCount, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde bir verinin indexleme işlemini yapmasını sağlayan metot.
    /// Index işlemi yapılırken id yi kendimiz vermezsek Elasticsearch bir id üretir.
    /// Elasticsearc doğasında insert işlemi yoktur. Bunun yerine indexleme dediğimiz ve birden fazla işlemden oluşan bir boru hattı vardır.
    /// Bu hat içerisinde öncelikle alınan veri RAM'e yazılır. Daha sonrasında tokenizer işlemi başlar. Bu işlemde ek ayar verilmedikçe elasticsearch alınan veri içerisindeki karakterleri boşluk karakterine göre parçalar. Ve her bir karakteri kaydeder.
    /// Hemen ardından normalize işlemi başlar. Burada veri büyük/küçükk karakter uyumuna uysun diye tek bir formata çekilir. Eğer ayarlardan eklenmek istenirse kelimeler için aynı anlamlı kelimeler de eklenebilir.</summary>
    /// <param name="entity">Eklenecek olan veri.</param>
    /// <param name="indexName">Eklenecek index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    Task<(T data, string message)> IndexAsync(T entity, string indexName);

    /// <summary>Elasticsearch üzerinde id yardımıyla bulunan bir verinin güncelleme işlemini yapmasını sağlayan metot.
    /// Bu metot çalışırken aynı index metodunda olduğu için Token ve Normalize işlemlerini yürütür.
    /// <param name="entity">Güncellenecek olan veri.</param>
    /// <param name="indexName">Güncellenecek index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    Task<(T data, string message)> UpdateAsync(T entity, string indexName);

    /// <summary>Elasticsearch üzerinde id yardımıyla bulunan bir verinin silinme işlemini yapmasını sağlayan metot.</summary>
    /// <param name="id">Silinecek veri için gereken tekil bilgi.</param>
    /// <param name="indexName">Güncellenecek index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    Task<(bool isSuccess, string message)> DeleteAsync(string id, string indexName);

    #endregion [STRUCTURED_QUERYS]

    #region [UNSTRUCTURED_QUERYS]

    /// <summary>Elasticsearch üzerinde bir field yardımıyla full text search kullanarak verilerin çekilmesi işlemini yapmasını sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="fuzinessCharCount">Parametre olarak alınan veride hata yapılmasına(eşleşmemesine) müsade edilecek karakter sayısı.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> FullTextMatchQueryAsync(string indexName, string fieldName, string value, int fuzinessCharCount, int pageSize = 10, int page = 1);

    #endregion [UNSTRUCTURED_QUERYS]

    #region [COMPOUND_QUERYS]

    /// <summary>Elasticsearch üzerinde arama yaparken çoklu özelliklere sahip bir filtre yapısı ekleyerek verileri çekmemizi sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    /// <param name="mustFieldName">Karmaşık query içerisinde kesinlikle olması istenen MUST özelliği için kullanılacak özellik ismi.</param>
    /// <param name="mustFieldValue">Karmaşık query içerisinde kesinlikle olması istenen MUST özelliği için kullanılacak özellik değeri.</param>
    /// <param name="mustnotFieldName">Karmaşık query içerisinde kesinlikle olmaması istenen MUST_NOT özelliği için kullanılacak özellik ismi.</param>
    /// <param name="mustnotFieldValue">Karmaşık query içerisinde kesinlikle olmaması istenen MUST_NOT özelliği için kullanılacak özellik değeri.</param>
    /// <param name="shouldFieldName">Karmaşık query içerisinde olması durumunda önceliğe alınması istenen SHOULD özelliği için kullanılacak özellik ismi.</param>
    /// <param name="shouldFieldValue">Karmaşık query içerisinde olması durumunda önceliğe alınması istenen SHOULD özelliği için kullanılacak özellik değeri.</param>
    /// <param name="filterFieldName">Karmaşık query içerisinde verinin filtrelenmesi istenen FILTER özelliği için kullanılacak özellik ismi.</param>
    /// <param name="filterFieldValue">Karmaşık query içerisinde verinin filtrelenmesi istenen FILTER özelliği için kullanılacak özellik değeri.</param>
    Task<(List<T> data, string message)> CompoundQueryAsync(
        string indexName,
        string mustFieldName, string mustFieldValue,
        string mustnotFieldName, double mustnotFieldValue,
        string shouldFieldName, DateTime shouldFieldValue,
        string filterFieldName, string filterFieldValue,
        int pageSize = 10,
        int page = 1);

    #endregion [COMPOUND_QUERYS]
}