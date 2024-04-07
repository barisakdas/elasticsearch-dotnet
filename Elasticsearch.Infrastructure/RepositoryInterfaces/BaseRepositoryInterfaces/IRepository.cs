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

    /// <summary>Elasticsearch üzerinde arama yaparken veriyi id değeri ile çekmemizi sağlayan metot.</summary>
    /// <param name="id">Tek bir veri almamız için gereken tekil bilgi.</param>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    Task<(T data, string message)> GetAsync(string id, string indexName);

    /// <summary>TermQuery, Elasticsearch’de belirli bir alanda kesin bir terimi içeren belgeleri döndüren bir sorgu türüdür.
    /// Bu sorgu, kesin bir değer aramak için kullanılır, örneğin bir fiyat, bir ürün kimliği veya bir kullanıcı adı gibi.
    /// TermQuery, metin alanları için değil, kesin değerler gerektiren alanlar için idealdir.
    /// Metin alanlarının değerleri analiz sürecinde değiştiği için, bu tür alanlarda kesin eşleşmeler bulmak zor olabilir.
    /// Metin alanlarında arama yapmak için match query kullanılması önerilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> TermQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>TermsQuery, Elasticsearch’de belirli bir alanda bir veya daha fazla kesin terimi içeren belgeleri döndüren bir sorgu türüdür.
    /// TermQuery ile benzerdir, ancak birden fazla değer arayabilirsiniz. Bir belge, en az bir terimi içeriyorsa eşleşir.
    /// Birden fazla eşleşen terim içeren belgeleri aramak için terms_set sorgusunu kullanabilirsiniz</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> TermsQueryAsync(string indexName, string fieldName, List<string> values, int pageSize = 10, int page = 1);

    /// <summary>PrefixQuery, Elasticsearch’de belirli bir alanda belirli bir ön ek (prefix) ile başlayan terimleri içeren belgeleri döndüren bir sorgu türüdür.
    /// Bu sorgu, özellikle kullanıcıların bir metin alanında belirli bir karakter serisiyle başlayan belgeleri bulmaları gerektiğinde kullanışlıdır.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> PrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>DateRangeQuery, Elasticsearch’de belirli bir tarih aralığında olan belgeleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, belirli bir alan için belirlenen başlangıç ve bitiş tarihleri arasındaki değerleri içeren belgeleri döndürür.
    /// rneğin, son bir yıl içinde oluşturulan belgeleri veya belirli bir tarih aralığındaki olayları bulmak için kullanılabilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="startValue">Aranacak özellikteki başlangıç tarih değeri.</param>
    /// <param name="endValue">Aranacak özellikteki bitiş tarih değeri.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> DateRangeQueryAsync(string indexName, string fieldName, DateTime startValue, DateTime endValue, int pageSize = 10, int page = 1);

    /// <summary>NumberRangeQuery, Elasticsearch’de sayısal bir alan üzerinde belirli bir aralıkta olan değerleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, belirli bir sayısal alan için belirlenen minimum ve maksimum değerler arasındaki değerleri içeren belgeleri döndürür.
    /// Örneğin, belirli bir fiyat aralığındaki ürünleri veya belirli bir puan aralığındaki değerlendirmeleri bulmak için kullanılabilir</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="startValue">Aranacak özellikteki başlangıç sayı değeri.</param>
    /// <param name="endValue">Aranacak özellikteki bitiş sayı değeri.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> NumberRangeQueryAsync(string indexName, string fieldName, double startValue, double endValue, int pageSize = 10, int page = 1);

    /// <summary>WildcardQuery, Elasticsearch’de belirli bir desene uyan terimleri içeren belgeleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, joker karakterler (* ve ?) kullanarak, belirli bir alanda arama yapmanızı sağlar* karakteri sıfır veya daha fazla karakterle
    /// eşleşirken, ? karakteri yalnızca tek bir karakterle eşleşir.
    /// Örneğin: App?e => 5 karakter ve geriye Apple döndürebilir.
    /// Örneğin: App*e => en az 5 karakter ve geriye Apple, Appilacate gibi değerleri döndürebilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> WildcardQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>FuzzyQuery, Elasticsearch’de benzer terimleri bulmak için kullanılan bir sorgu türüdür.
    /// Bu sorgu, arama terimi ile belirli bir düzenleme mesafesi içinde tüm olası varyasyonları veya genişlemeleri oluşturur ve her genişleme için kesin eşleşmeleri döndürür.
    /// Düzenleme mesafesi, bir terimi diğerine dönüştürmek için gerekli olan tek karakter değişikliklerinin sayısını ifade eder.
    /// Bu değişiklikler, bir karakteri değiştirme, bir karakteri çıkarma, bir karakter eklemek veya iki bitişik karakteri yer değiştirmek gibi işlemleri içerebilir
    /// Örneğin: Google verisini ararken hata payını 1 karakter verirsek Googla, Poogle gibi parametreler ile veri dönüşü sağlanırken Goopla gibi birden fazla hata alınan filtrelerde veri alınmaz.</summary>
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

    /// <summary>MatchAllQuery, Elasticsearch’de en basit sorgu türlerinden biridir ve tüm belgeleri eşleştirerek, her birine varsayılan olarak
    /// 1.0 _score verir. Bu sorgu, özellikle bir indeksteki veya birden fazla indeksteki tüm belgeleri almak istediğinizde kullanışlıdır.
    /// Örneğin, filtreler uygulamak, toplulaştırmalar yapmak veya sıralama işlemleri gerçekleştirmek gibi operasyonlar için bu sorgu tercih edilir.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> MatchAllQueryAsync(string indexName, int pageSize = 10, int page = 1);

    /// <summary>MultiMatchAllQuery terimi, Elasticsearch’de doğrudan bir sorgu türü olarak tanımlanmamıştır.
    /// Ancak, kullanıcı muhtemelen multi_match sorgusunu ve match_all sorgusunu birleştirmiş olabilir.
    /// multi_match sorgusu, birden fazla alanda eş zamanlı arama yapmak için kullanılır ve match sorgusunun çoklu alan versiyonudurmatch_all sorgusu ise,
    /// bir indeksteki tüm belgeleri döndürmek için kullanılır ve herhangi bir arama kriteri gerektirmez.
    /// Eğer kullanıcı, tüm alanlarda belirli bir terimi arayan bir sorgu oluşturmak istiyorsa, multi_match sorgusunu * joker karakteri ile kullanarak
    /// tüm alanlarda arama yapabilir. </summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName1">Aramanın yapılacağı ilk özellik.</param>
    /// <param name="fieldName2">Aramanın yapılacağı ikinci özellik.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> MultiMatchAllQueryAsync(string indexName, string fieldName1, string fieldName2, string value, int pageSize = 10, int page = 1);

    /// <summary>match_bool_prefix sorgusu, Elasticsearch’de kullanılan bir sorgu türüdür.
    /// Bu sorgu, girdiyi analiz eder ve terimlerden bir bool sorgusu oluşturur. Son terim hariç her terim bir term sorgusunda kullanılırken,
    /// son terim bir prefix sorgusunda kullanılır1. Örneğin, “hızlı kahverengi t” girdisi için, match_bool_prefix sorgusu, “hızlı”, “kahverengi”
    /// ve “t” terimlerini üretir ve bu terimlerden “hızlı” ve “kahverengi” için term sorguları, “t” için ise prefix sorgusu oluşturur.
    /// match_bool_prefix sorgusunun önemli bir özelliği, match_phrase_prefix sorgusundan farklı olarak, terimlerin bir ifade olarak değil,
    /// herhangi bir pozisyonda eşleşebilmesidir.Yani, “hızlı kahverengi tilki” ifadesiyle eşleşebileceği gibi, “kahverengi tilki hızlı” veya
    /// “hızlı” ve “kahverengi” terimlerini ve “f” ile başlayan bir terimi içeren herhangi bir pozisyondaki belgelerle de eşleşebilir.
    /// Bu sorgu türü, kullanıcıların metin girişini otomatik olarak tokenlere ayırmasını sağlar ve minimum_should_match ve operator parametrelerini
    /// destekler.Bu parametreler, oluşturulan bool sorgusuna uygulanır.fuzziness, prefix_length, max_expansions, fuzzy_transpositions ve
    /// fuzzy_rewrite parametreleri, son terim hariç tüm terimler için oluşturulan term alt sorgularına uygulanabilir.
    /// Ancak, son terim için oluşturulan prefix sorgusuna etki etmezler.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> MatchBoolPrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>MatchPhraseQuery, Elasticsearch’de tam metin aramaları için kullanılan bir sorgu türüdür.
    /// Bu sorgu, analiz edilen metni kullanarak bir ifade sorgusu oluşturur ve belirli bir sırayla yan yana gelen terimleri içeren belgeleri bulur.
    /// Örneğin, “bu bir testtir” ifadesini aramak için kullanılan bir MatchPhraseQuery şu şekilde olabilir</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> MatchPhraseQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    /// <summary>Elasticsearch üzerinde bir field yardımıyla full text search kullanarak verilerin çekilmesi işlemini yapmasını sağlayan metot.</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="fuzinessCharCount">Parametre olarak alınan veride hata yapılmasına(eşleşmemesine) müsade edilecek karakter sayısı.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> FullTextMatchQueryAsync(string indexName, string fieldName, string value, int fuzinessCharCount, int pageSize = 10, int page = 1);

    /// <summary>MatchPhrasePrefixQuery, Elasticsearch’de kullanılan bir sorgu türüdür.
    /// Bu sorgu, verilen metnin kelimelerini sağlanan sırayla içeren belgeleri döndürür. Sağlanan metnin son terimi bir ön ek (prefix) olarak ele alınır
    /// ve bu terimle başlayan herhangi bir kelimeyle eşleşir. Örneğin, “hızlı kahverengi t” metni için bu sorgu, “hızlı kahverengi tilki” veya
    /// “iki hızlı kahverengi fırça” değerine sahip bir mesajla eşleşir, ancak “tilki hızlı ve kahverengidir” ile eşleşmez1.
    /// MatchPhrasePrefixQuery kullanımının temel parametreleri şunlardır:
    ///     * query: Aranacak metni içerir.
    ///     * analyzer: Metni tokenlere dönüştürmek için kullanılan analizörü belirtir.Varsayılan olarak, alan için haritalanan indeks zamanı analizörü kullanılır.
    ///     * max_expansions: Sorgu değerinin son teriminin genişleyebileceği maksimum terim sayısını belirtir.Varsayılan olarak 50’dir.
    ///     * slop: Eşleşen tokenlar arasında izin verilen maksimum pozisyon sayısını belirtir.Varsayılan olarak 0’dır.
    /// Bu sorgu türü, özellikle arama otomasyonu için kullanıldığında bazen kafa karıştırıcı sonuçlar üretebilir. Daha iyi çözümler için completion suggester
    /// ve search_as_you_type alan tipi gibi diğer araçlar da mevcuttur</summary>
    /// <param name="indexName">Aranacak index ismi(ilişkisel veri tabanlarındaki tabloya karşılık gelir).</param>
    /// <param name="fieldName">Aramanın yapılacağı özelliğin isim değeri.</param>
    /// <param name="value">İlgili özellikte arama yapılacak değer.</param>
    /// <param name="pageSize">Sayfalandırma yapısında bir sayfada kaç veri gösterileceğini belirten özellik.</param>
    /// <param name="page">Sayfalandırma yapısında hangi sayfa için veri alınacağını belirten özellik.</param>
    Task<(List<T> data, string message)> MatchPhrasePrefixQueryAsync(string indexName, string fieldName, string value, int pageSize = 10, int page = 1);

    #endregion [UNSTRUCTURED_QUERYS]

    #region [COMPOUND_QUERYS]

    /// <summary>CompoundQuery, Elasticsearch’de diğer bileşik veya yaprak (leaf) sorguları sarmalayarak, sonuçlarını ve skorlarını birleştirmek,
    /// davranışlarını değiştirmek veya sorgu ile filtre bağlamı arasında geçiş yapmak için kullanılan bir sorgu grubudur1.
    /// Bu sorgular, birden fazla sorgu koşulunu mantıksal operatörlerle (must, should, must_not, filter) birleştiren bool query, belirli bir
    /// sorguya uyan belgeleri döndürürken başka bir sorguya uyan belgelerin skorunu düşüren boosting query, başka bir sorguyu sarmalayıp filtre
    /// bağlamında çalıştıran ve tüm eşleşen belgelere sabit bir _score veren constant_score query, birden fazla sorguyu kabul edip herhangi bir
    /// sorgu koşuluna uyan belgeleri döndüren dis_max query ve ana sorgunun döndürdüğü skorları popülerlik, güncellik, mesafe veya özel algoritmalar
    /// gibi faktörleri hesaba katarak değiştiren function_score query gibi çeşitli sorguları içerir</summary>
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