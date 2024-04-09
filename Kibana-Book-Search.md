# Book Modeli İçin Kibana Üzerinde Search İşlemleri Nasıl Yapılır?
Kibana üzerinde Book modeli için arama işlemleri yapmak, Elasticsearch’de saklanan kitap verilerini sorgulamanıza olanak tanır. İşte bu işlemleri nasıl yapabileceğinize dair bir rehber:

1. Basit Arama
Basit bir arama yapmak için, Kibana’nın Dev Tools sekmesini kullanabilirsiniz. Örneğin, başlığı “Büyük Veri Analitiği” olan kitapları aramak için aşağıdaki sorguyu kullanabilirsiniz: 

`GET /books/_search`
```json
{
  "query": {
    "match": {
      "title": "Büyük Veri Analitiği"
    }
  }
}
```

2. Filtreleme
Fiyat, stok miktarı veya yayın tarihi gibi belirli alanlara göre filtreleme yapmak istiyorsanız, range sorgusunu kullanabilirsiniz. Örneğin, fiyatı 50 TL’den fazla olan kitapları bulmak için:

`GET /books/_search`
```json
{
  "query": {
    "range": {
      "price": {
        "gte": 50
      }
    }
  }
}
```

3. Kategorilere Göre Arama
Belirli kategorilerdeki kitapları aramak için terms sorgusunu kullanabilirsiniz. Örneğin, “Teknoloji” ve “Bilim” kategorilerindeki kitapları bulmak için:

`GET /books/_search`
```json
{
  "query": {
    "terms": {
      "categories": ["Teknoloji", "Bilim"]
    }
  }
}
```

4. Karmaşık Sorgular
Birden fazla kriteri birleştirmek için bool sorgusunu kullanabilirsiniz. Örneğin, “Teknoloji” kategorisindeki ve stok miktarı 100’den fazla olan kitapları bulmak için:

`GET /books/_search`
```json
{
  "query": {
    "bool": {
      "must": [
        { "term": { "categories": "Teknoloji" }},
        { "range": { "stock": { "gt": 100 }}}
      ]
    }
  }
}
```

5. Sayfalama
Sonuçları sayfalamak için from ve size parametrelerini kullanabilirsiniz. Örneğin, ilk 10 sonucu almak için:

`GET /books/_search`
```json
{
  "from": 0,
  "size": 10,
  "query": {
    "match_all": {}
  }
}
```

6. Sıralama
Sonuçları belirli bir alan göre sıralamak için sort parametresini kullanabilirsiniz. Örneğin, fiyata göre artan sıralama yapmak için:

`GET /books/_search`
```json
{
  "sort": [
    { "price": { "order": "asc" }}
  ],
  "query": {
    "match_all": {}
  }
}
```

7. Fuziness
Bir arama sırasında kelimeler içerisindeki bazı karakterler yanlış olarak yazılsa bile bunu anlayıp getirebilmek için:

`GET /books/_search`
```json
{
  "query": {
    "match": {
      "title": {
        "query": "Büyük Veri Analitiki",
        "fuzziness": 2
      }
    }
  }
}
```
Bu sorgu, “title” alanında “Büyük Veri Analitiği” ifadesine benzer kelimeleri arar ve en fazla iki karakter farklılığı olan sonuçları döndürür. Örneğin, “Büyük Veri Analitigi” veya “Buyuk Veri Analitiği” gibi yazım hataları olan başlıkları da bulabilirsiniz. fuzziness değeri olarak 2 kullanmak, arama terimi ile eşleşen terimler arasında en fazla iki karakter değişikliği (ekleme, çıkarma veya değiştirme) olabileceği anlamına gelir. Bu, kullanıcıların hatalı yazımlarını düzeltebilir ve arama sonuçlarını genişletebilir.

Bu rehber, Book modeli için Kibana üzerinde temel arama işlemlerini nasıl yapabileceğinizi göstermektedir. İhtiyacınıza göre bu sorguları özelleştirebilir ve genişletebilirsiniz. Her bir sorgu, farklı senaryolarda kullanılmak üzere tasarlanmıştır.