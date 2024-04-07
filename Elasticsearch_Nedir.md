# .NET 8 .0 & ELASTICSEARCH

## Elasticsearch Nedir?
Elasticsearch, Apache Lucene tabanlı, dağıtılmış, RESTful arama ve analiz motorudur. Büyük veri setleri üzerinde hızlı ve gerçek zamanlı arama ve analiz yapabilme yeteneğine sahiptir. JSON formatındaki belgeleri indeksleyerek, bu belgeler üzerinde karmaşık sorgulamalar yapmanıza olanak tanır.

Elasticsearch’in temel özellikleri şunlardır:
* Hızlı ve Ölçeklenebilir: Elasticsearch, verileri paralel olarak işleyerek hızlı arama sonuçları sunar ve büyüyen veri hacimlerine kolayca uyum sağlar.
* Esnek Sorgulama Dili: Çeşitli arama sorgularını destekler ve karmaşık veri analizleri yapabilir.
* Yüksek Erişilebilirlik: Dağıtılmış yapısı sayesinde, bir sunucu arızalandığında bile veriye erişim sağlamaya devam eder.
* Görselleştirme Araçları: Kibana gibi araçlarla entegre çalışarak verilerin görselleştirilmesini ve panolar oluşturulmasını sağlar.

Elasticsearch, genellikle log ve olay izleme, güvenlik istihbaratı, tam metin arama ve operasyonel zeka gibi alanlarda kullanılır. Ayrıca, büyük veri analitiği ve karmaşık aramalar için de tercih edilen bir çözümdür.

Elasticsearch’in avantajları arasında yüksek ölçeklenebilirlik, hızlı arama sonuçları, esnek yapı, RESTful API desteği ve açık kaynak olması bulunur. Diğer NoSQL veritabanlarından farkı, özellikle arama ve analiz işlemlerine odaklanması, dağıtılmış mimarisi ve gerçek zamanlı veri analizi yapabilmesidir. Arama işlemi, kullanıcıların tanımladığı sorgularla eşleşen sonuçları döndüren bir API üzerinden gerçekleştirilir. Elasticsearch, verileri indekslerken belirli alanları indeksler ve arama yapılırken bu indeksleri kullanarak hızlı sonuçlar sağlar

Elasticsearch’in full text search (tam metin arama) altyapısı, Apache Lucene üzerine kuruludur ve metin tabanlı verilerde karmaşık aramalar yapabilmenizi sağlar. İşte temel adımlar:

* İndeksleme: Veri, Elasticsearch’e yüklenir ve bir ‘indeks’ oluşturulur. Bu süreçte, belirli bir ‘analyzer’ kullanılarak metin parçalanır ve indekslenir.
* Sorgulama: Kullanıcılar, Elasticsearch’in anlayabileceği sorguları oluşturur. Bu sorgular, metin alanlarında arama yapmak için kullanılır.
* Eşleştirme: Sorgu, indekslenmiş verilerle eşleştirilir ve ilgili sonuçlar döndürülür.

Arama türleri arasında ‘match query’ (eşleşme sorgusu), ‘match_phrase query’ (ifade eşleşme sorgusu) ve ‘multi_match query’ (çoklu alan eşleşme sorgusu) gibi çeşitli seçenekler bulunur. Bu sorgular, metin alanlarının nasıl analiz edildiğini anlar ve sorgu dizesini yürütmeden önce uygulanacak analizörü kullanır.

Elasticsearch, kullanıcıların doğrudan maruz kalabileceği daha basit ve sağlam bir sorgu dili olan ‘simple_query_string query’ (basit sorgu dizesi sorgusu) gibi araçlar da sunar. Bu, kullanıcıların AND, OR, NOT koşullarını ve tek bir sorgu dizesi içinde çoklu alan aramasını belirtmelerine olanak tanır.

## Indexleme
Elasticsearch’de indeksleme, verilerin arama motoru tarafından hızlı bir şekilde erişilebilir hale getirilmesi sürecidir. İşte bu sürecin detayları:

Veri Yükleme: Veriler, Elasticsearch’e JSON formatında yüklenir.
Analiz: Yüklenen veri, ‘analyzer’ tarafından işlenir. Bu aşamada, metin parçalanır ve terimlere (tokens) ayrılır.
İndeks Oluşturma: Parçalanan terimler, bir indeks içinde saklanır. Bu indeks, daha sonra arama sırasında kullanılır.
İndeksleme süreci, verinin arama motorunda nasıl saklandığını ve sorgulandığını belirler. Elasticsearch, her kelimeyi ayrı ayrı indeksleyerek arama performansını artırır. Ayrıca, Elasticsearch’in indeksleme işlemi, verileri belleğe alarak ve ardından shard içerisindeki segmentlere aktararak gerçekleşir. Bu segmentlerdeki veri, arama için hazırdır ve son aşamada file sisteme geçer.

Elasticsearch, yapılandırılmış ve yapılandırılmamış veriler için güçlü ve esnek arama yetenekleri sunar. Bu, özellikle büyük veri setleri üzerinde etkili bir şekilde arama yapabilmek için önemlidir.
Elasticsearch’de indeksleme için bir örnek senaryo şu şekilde olabilir:

Diyelim ki bir kitapçı için bir arama motoru geliştiriyorsunuz ve kitapların detaylarını içeren bir veri setine sahipsiniz. Bu veri setinde her kitap için başlık, yazar, açıklama ve yayın tarihi gibi alanlar bulunuyor.

Veri Yapısını Belirleme: İlk olarak, kitap verilerinizin yapısını belirleyerek bir ‘mapping’ (haritalama) oluşturursunuz. Bu mapping, Elasticsearch’e hangi alanların nasıl indeksleneceğini söyler. Örneğin, ‘title’ ve ‘author’ alanları için ‘text’ tipinde, ‘publish_date’ için ise ‘date’ tipinde mapping tanımlayabilirsiniz.
İndeks Oluşturma: Ardından, ‘books’ adında bir indeks oluşturursunuz. Bu indeks, kitap verilerinizin saklanacağı yerdir.
Veri İndeksleme: Kitap verilerinizi JSON formatında Elasticsearch’e yüklersiniz. Her bir kitap için ayrı bir JSON belgesi oluşturulur ve ‘books’ indeksine eklenir.
Sorgulama: İndeksleme işlemi tamamlandıktan sonra, kullanıcılar kitap başlıklarına, yazar isimlerine veya açıklamalara göre arama yapabilir. Örneğin, bir kullanıcı “Elasticsearch” kelimesini içeren tüm kitap başlıklarını bulmak için bir sorgu gönderebilir.
Bu senaryoda, indeksleme süreci, kitap verilerinin hızlı ve etkili bir şekilde aranabilmesi için kritik bir rol oynar. Elasticsearch’in güçlü arama yetenekleri sayesinde, kullanıcılar aradıkları kitaplara kolayca ulaşabilir

## Sorgulama
Elasticsearch’de sorgulama işlemi, veri setleri üzerinde arama yapmak için kullanılan çeşitli sorgu türlerini içerir. İşte bu sorgulama işleminin detayları:

Sorgu Türleri: Elasticsearch, farklı sorgu türlerini destekler. Bunlar arasında match, term, range, bool ve daha pek çok sorgu türü bulunur. Her bir sorgu türü, belirli bir arama senaryosuna göre optimize edilmiştir.
* Match Sorguları: Tam metin aramaları için kullanılır. Kullanıcıların girdiği metin, analiz edilerek indekslenmiş terimlerle eşleştirilir.
* Term Sorguları: Belirli bir terimi veya terimleri aramak için kullanılır. Bu sorgular, analiz sürecinden geçmeyen kesin değer aramalarıdır.
* Bool Sorguları: Birden fazla sorgu türünü AND, OR, NOT gibi mantıksal operatörlerle birleştirmek için kullanılır.
* Range Sorguları: Belirli bir aralıktaki değerleri bulmak için kullanılır. Örneğin, belirli tarihler arasında yayınlanmış kitapları bulmak için kullanılabilir.
* Aggregation: Veri üzerinde istatistiksel analizler yapmak için kullanılır. Örneğin, belirli bir yazarın kitaplarının ortalama sayfa sayısını hesaplamak için kullanılabilir.
* Sorgu Yapısı: Elasticsearch sorguları, JSON formatında yazılır. Bir sorgu, query anahtar kelimesi ile başlar ve arama kriterlerini içeren JSON nesneleri içerir.
* Analiz Süreci: Full-text sorgularında, arama terimleri önce analiz edilir. Bu analiz süreci, tokenization ve normalization gibi işlemleri içerir.
* Performans: Elasticsearch, sorguları paralel olarak işleyerek yüksek performanslı arama sonuçları sunar.

## Sorgu Örnekleri
Elasticsearch’de kullanılan bazı temel sorgu türleri ve bunlara ait örnekler aşağıda yer almaktadır:

1. Match Sorgusu:
Amaç: Belirli bir alan üzerinde tam metin araması yapmak.
```json
{
  "query": {
    "match": {
      "alan_adi": "aranacak_metin"
    }
  }
}
```

2. Term Sorgusu:
Amaç: Belirli bir alan üzerinde kesin değer araması yapmak.
```json
{
  "query": {
    "term": {
      "alan_adi": {
        "value": "kesin_deger"
      }
    }
  }
}
```

3. Range Sorgusu:
Amaç: Belirli bir alan üzerinde bir değer aralığı sorgulamak.
```json
{
  "query": {
    "range": {
      "alan_adi": {
        "gte": "baslangic_degeri",
        "lte": "bitis_degeri"
      }
    }
  }
}
```

4. Bool Sorgusu:
Amaç: Birden fazla sorgu koşulunu mantıksal operatörlerle birleştirmek.
```json
{
  "query": {
    "bool": {
      "must": [
        { "match": { "alan_adi1": "metin1" } },
        { "match": { "alan_adi2": "metin2" } }
      ],
      "must_not": [
        { "range": { "alan_adi3": { "lte": "deger" } } }
      ],
      "should": [
        { "term": { "alan_adi4": "deger" } }
      ],
      "filter": [
        { "term": { "alan_adi5": "deger" } }
      ]
    }
  }
}
```

5. Aggregation Sorgusu:
Amaç: Veri üzerinde istatistiksel analizler yapmak.
```json
{
  "size": 0,
  "aggs": {
    "populer_kategoriler": {
      "terms": {
        "field": "kategori_alani"
      },
      "aggs": {
        "ortalama_fiyat": {
          "avg": {
            "field": "fiyat_alani"
          }
        }
      }
    }
  }
}
```

Bu örnekler, Elasticsearch’de sorgulama yaparken kullanabileceğiniz temel yapıları göstermektedir. Her bir sorgu türü, farklı senaryolarda kullanılmak üzere tasarlanmıştır ve bu sorguları kendi veri setlerinize göre özelleştirebilirsiniz.

## Eşleştirme
Elasticsearch’de eşleştirme (matching), kullanıcı sorgularının indekslenmiş verilerle karşılaştırılması sürecidir. Bu süreç, arama motorunun temel işlevlerinden biridir ve şu adımları içerir:

* Analiz: Sorgu metni, belirlenen analizör tarafından işlenir. Bu işlem sırasında metin, terimlere (tokens) ayrılır ve normalleştirilir.
* Arama: Analiz edilen terimler, indekslenmiş veri seti içinde aranır. Elasticsearch, bu terimleri belgelerdeki indekslerle karşılaştırarak eşleşmeleri bulur.
* Skorlama: Eşleşen belgeler, ne kadar iyi eşleştiklerine bağlı olarak bir skor alır. Bu skor, sorgu terimlerinin belgedeki önemi ve sıklığı gibi faktörlere dayanır.
* Sıralama: Belgeler, skorlarına göre sıralanır ve en yüksek skora sahip olanlar arama sonuçlarının başında yer alır.

Elasticsearch, Apache Lucene kütüphanesini kullanarak bu eşleştirme işlemini gerçekleştirir. Lucene, metin tabanlı arama süreçlerinde bir indeks oluşturarak belgelerin içeriğini parçalara ayırır ve bu parçaları anahtar kelimelere dönüştürerek indeksler. Bu indeksler, metin tabanlı sorgulama süreçlerinde etkili bir şekilde arama yapılmasını ve hızlı bir şekilde sonuçlara ulaşılmasını sağlar.

Elasticsearch’in eşleştirme süreci, metin işleme çeşitliliği açısından oldukça zengindir ve kelime köklerini bulma (stemming), eş anlamlı kelimelerin tanınması (synonym recognition), büyük/küçük harf duyarlılığı (case sensitivity), dil analizi (language analysis) gibi özelliklerle metin işleme süreçlerinde işlevsellik gösterir. Bu özellikler, arama süreçlerinde daha doğru ve kapsamlı sonuçlar elde edilmesini sağlar.