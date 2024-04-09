# Book Modeli İçin Kibana Üzerinde Örnek İndex İşlemleri

İlgili Book modeli için öncelikle Elasticsearch üzerinde bir şema tanımlaması yapılması gereklidir.
Şema işlemini Elasticsearch ilk index işlemimizle birlikte kendisi oluşturabilir ama bu oluşturulan şema gönderdiğimiz verinin Elasticsearch tarafındaki yorumlanması ile oluşur. Bu durumda istenen şema tam olarak bizim veri tiplerimiz ile eşleşmiyor olabilir.
Örneğin şema oluşturmadan şöyle bir istek göndermiş olalım:

```json
{
  "title": "Yapay Zeka ve Geleceği",
  "abstract": "Bu kitap, yapay zekanın temellerini, tarihçesini ve gelecekteki potansiyelini derinlemesine inceler. Yapay zeka teknolojilerinin nasıl geliştiğini, günümüzdeki uygulamalarını ve gelecekte insan hayatını nasıl etkileyeceğini detaylı bir şekilde anlatır. Yapay zeka, algoritmaların ve veri işlemenin sınırlarını zorlayarak, öğrenme, algılama ve karar verme yeteneklerini makineler aracılığıyla gerçekleştirmeyi amaçlar. Bu kitapta, yapay zekanın farklı disiplinlerle olan ilişkisi, etik konular, ve toplumsal etkileri üzerinde durulur. Ayrıca, yapay zekanın sağlık, eğitim, sanayi, ve finans gibi çeşitli sektörlerde nasıl devrim yarattığı ve bu teknolojilerin gelecekteki iş ve yaşam biçimlerimizi nasıl dönüştürebileceği tartışılır. Yapay zekanın gelişimi, tarihsel bir perspektiften ele alınarak, geçmişten günümüze önemli kilometre taşları ve bu alandaki öncü isimlerin katkıları irdelenir. Kitap, yapay zekanın temel kavramlarını ve teorilerini anlaşılır bir dille sunarken, aynı zamanda derin öğrenme, makine öğrenimi, sinir ağları, ve doğal dil işleme gibi konulara da detaylı bir giriş yapar. Okuyucular, yapay zekanın karmaşık dünyasına dair kapsamlı bir bakış açısı kazanırken, bu teknolojinin insanlık için barındırdığı umutları ve riskleri de değerlendirme fırsatı bulacaklar.",
  "price": 29,
  "stock": 150,
  "publishdate": "2024-01-15",
  "categories": ["Teknoloji", "Bilim"],
  "author": {
    "firstname": "Ahmet",
    "lastname": "Kaya"
  }
}
```

Yukarıdaki isteği kod üzerinden veya Kibana üzerinden Elasticsearch üzerine eklediğimizi varsayalım. Elasticsearch bu isteği yorumlar ve kendisi bir şema oluşturur.
Bu şema oluşturma sırasında modeldeki `price` alanının değerine bakarak int bir değer şeklinde oluşturabilir. Ama biz bu alanda bir flout veri tipi kullanmak isteyebilir ve sonraki verilerde int olmayan ve double verisi olan bir istek attığımızda hatalı bir işlem olarak algılanacaktır.
Veya diğer verilerin tiplerinde ilk isteğe göre Elasticsearch'ın yorumlayarak bir şema oluşturmasını istemeyiz. Bu yüzden aşağıdaki gibi şemamızı kendimiz oluşturacağız.

## Şema Yapısı
Book modeli için aşağıdaki gibi bir şema oluşturabiliriz. Bu isteği Kibana üzerinden `PUT /books` komutu ile kullanabiliriz.

```json
{
  "mappings": {
    "properties": {
      "title": {
        "type": "text",
        "fields": {
          "keyword": {
            "type": "keyword"
          }
        }
      },
      "abstract": {
        "type": "text"
      },
      "price": {
        "type": "scaled_float",
        "scaling_factor": 100
      },
      "stock": {
        "type": "integer"
      },
      "publishdate": {
        "type": "date"
      },
      "categories": {
        "type": "keyword"
      },
      "authors": {
        "properties": {
          "firstname": {
            "type": "text",
            "fields": {
              "keyword": {
                "type": "keyword"
              }
            }
          },
          "lastname": {
            "type": "text",
            "fields": {
              "keyword": {
                "type": "keyword"
              }
            }
          }
        }
      }
    }
  }
}
```

Elasticsearch şeması oluştururken, veri tipleri ve yapılarını seçerken dikkat edilen bazı önemli noktalar vardır. İşte yukarıdaki şemada alınan veri tiplerinin seçimine dair açıklamalar:

* Text ve Keyword: title, abstract, firstname ve lastname alanları için “text” tipi kullanılmıştır çünkü bu alanlar genellikle tam metin aramalarında kullanılır. “text” tipi, veriyi analiz ederek arama sırasında kelime bazında eşleştirme yapar. Ayrıca, bu alanların her biri için “keyword” tipinde bir alt alan tanımlanmıştır. “keyword” tipi, tam eşleşme aramaları ve sıralama işlemleri için kullanılır ve metni analiz etmeden, tam metin olarak saklar.
* Scaled Float: price alanı için “scaled_float” tipi seçilmiştir. Bu tip, ondalık sayıları sabit bir çarpanla ölçeklendirerek saklar. Örneğin, “scaling_factor”: 100 ayarı, fiyatları tam sayı olarak saklamak için fiyat değerlerini 100 ile çarpar. Bu, kayan nokta sayılarının hassasiyet kaybını önler ve sorgulama sırasında daha kesin sonuçlar elde edilmesini sağlar.
* Integer: stock alanı için “integer” tipi kullanılmıştır çünkü stok miktarı tam sayı değerlerini ifade eder ve bu tip, tam sayı aralığındaki değerleri saklamak için uygundur.
* Date: publishdate alanı için “date” tipi seçilmiştir. Bu tip, tarih ve zaman bilgilerini ISO 8601 standardına uygun olarak saklar ve sorgulamalarda tarih aralıkları gibi işlemleri kolaylaştırır.
* Keyword for Categories: categories alanı için “keyword” tipi kullanılmıştır. Kategoriler genellikle filtreleme ve sıralama işlemlerinde kullanıldığı için, bu tipin seçilmesi uygun olmuştur.

Bu şema yapısında, veri tiplerinin seçimi, alanların kullanım amacına ve arama/filtreleme gereksinimlerine göre yapılmıştır. Ayrıca, performans ve sorgu doğruluğunu optimize etmek için de bu tipler tercih edilmiştir. Her bir alanın kullanım senaryosuna uygun en verimli veri tipinin seçilmesi, Elasticsearch üzerinde hızlı ve etkili sorgulama yapılabilmesini sağlar.

## Örnek Index İşlemi
Elasticsearch’te bir index işlemi gerçekleştirmek, verilerinizi saklamak ve sorgulamak için bir yapı oluşturmanızı sağlar. Yukarıda belirtilen şema yapısı, Book modelinizin veri tiplerini ve yapılarını tanımlar. Bu şemayı kullanarak, verilerinizi Elasticsearch’e ekleyebilir ve daha sonra bu veriler üzerinde arama, filtreleme ve sıralama gibi işlemler yapabilirsiniz.

İşte Book modeli için birkaç örnek index işlemi:

* Index Oluşturma ve Şema Tanımlama: İlk adım olarak, books adında bir index oluşturur ve yukarıda tanımlanan şemayı bu index’e uygularız. Bu işlemi Kibana üzerinden PUT /books komutu ile yapabiliriz.
* Veri Ekleme: Şemayı tanımladıktan sonra, Book modeline ait verileri index’e ekleyebiliriz. Her bir kitap için ayrı bir doküman oluştururuz ve bu dokümanları POST /books/_doc komutu ile ekleriz.
* Veri Güncelleme: Mevcut bir dokümanı güncellemek için POST /books/_doc/{id} komutunu kullanırız. Burada {id} güncellemek istediğimiz dokümanın ID’sidir.
* Veri Silme: Bir dokümanı silmek için DELETE /books/_doc/{id} komutunu kullanırız. Yine {id} silmek istediğimiz dokümanın ID’sidir.
* Sorgulama: Verileri sorgulamak için GET /books/_search komutu ile çeşitli sorgular yapabiliriz. Bu sorgular, belirli kriterlere göre verileri filtrelememizi ve sonuçları döndürmemizi sağlar.
* Sıralama ve Agregasyon: Verileri belirli bir özelliğe göre sıralamak veya veriler üzerinde istatistiksel işlemler yapmak için sıralama (sort) ve agregasyon (aggs) özelliklerini kullanabiliriz.

Bu işlemler, Elasticsearch’te veri yönetimi ve analizi için temel operasyonlardır ve Book modelinizin verilerini etkili bir şekilde yönetmenize olanak tanır. Her bir işlem, Kibana üzerinden veya kod aracılığıyla HTTP istekleri olarak gerçekleştirilebilir. Önemli olan, veri tiplerinizi ve iş akışınızı doğru şekilde planlamak ve Elasticsearch’in güçlü özelliklerinden tam olarak yararlanmaktır.