# .NET 8 .0 & ELASTICSEARCH

## Projenin içeriği
Bu proje `.Net 8.0` frameworkü ve `Elastic.Clients.Elasticsearch` kütüphaneleri yardımıyla Elasticsearch veritabanında yapılacak tüm işlemleri gösteren yapılar üzerine kurulmuştur. Proje DDD design pattern ve Repository pattern kullanılarak oluşturulmuştur.

Proje içerisinde 4 katman bulunur.
* Core Katmanı: Projenin ihtiyaç duyduğu merkezi yapı taşlarını bulunduran katmandır. Birden fazla katmanda kullanılacak paketler bu katmana eklenir. Böylece olası bir versiyon değişikliğinde tek bir katmanda değişiklik yeterli olacaktır. Core katmanında projenin temellerini oluşturacak olan `BaseEntity`, geri dönüşlerde api'ların ortak bir modelde toplanmasını sağlayan `ResultModel`, Enitity ve Dto'lar arasında otomatik dönüşümleri yapmamıza yardımcı olacak `Mapper` sınıfları ve client tarafından gelen istekleri yönetecek `Model` yapılarını barındırır.

Core katmanında aşağıdaki kütüphaneler kullanılmıştır.
```csharp
	<ItemGroup>
		<PackageReference Include="AutoMapper" Version="12.0.1" />
		<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
		<PackageReference Include="Elastic.Clients.Elasticsearch" Version="8.13.3" />
		<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
	</ItemGroup>
```

* Infrastructure Katmanı: Infrastructure katmanı projenin veritabanları ile ilgili işlemlerinden sorumlu olan katmandır. `Context` yapıları, `Repository` sınıf ve arayüzleri bu katmanda bulunur. Bu proje özelinde bu katmanda `Repository Pattern` mantığıyla tasarlanmış `BaseRepository` sınıf ve arayüzleri inşa edilmiştir. Bu sınıflar jenerik olarak tasarlanmış ve yazılımcıların her bir entity için kullanacağı ortak metotları barındırmaktadır. Böylelikle kod tekrarından kurtulmuş olmakla beraber bir merkezi yapı da kurulmuş olur. Bu katman merkezinde Core katmanını barındırdığından başka bir kütüphane eklenmeden kullanılmaktadır.

* Application Katmanı: Application katmanı proje içerisinde Api katmanından gelen istekleri Infrastructure katmanına iletmek ve aradaki işlemleri yapmaktan sorumludur. Proje içerisindeki tüm mantıksal işlemler, modeller arasındaki dönüşler, yardımcı servisler gibi ara komponentler bu katmanda bulunur. Application katmanıdanki metotlar geri dönüş için ortak bir model(ResultModel) kullanır. Böylece Api katmanında bir dönüş modeli oluşturma işlemlerinden kaçınılmış olur.

* Api Katmanı: Api katmanı projenin ön yüzü niteliğindedir. Projenin ayağa kalkmasını sağlayacak ayarları ve yapılanmaların bulunduğu ve her ortam için gereken ihtiyaçların yönetildiği katmandır. Bu katman controller sınıfları aracılığıyla Application katmanıyla haberleşir.

## Projenin Amacı
Proje Elasticsearch kullanımına yeni başlayanların client üzerinden gelen istekleri Elasticsearch üzerinde işlemeyi göstermek ve örnek yapılar oluşturmaktır. Bu metotların her birinin örneklemeleri olmasa bile `BaseRepository` içerisinde detaylı açıklamalar barındırmaktadır. Bu sayede hangi metot ne için kullanılacak, hangi parametreler ne amaca hizmet etmektedir detaylo dökümanları kod üzerinde mevcuttur.

## Projenin kullanımı
Projenin kullanımı için bir elasticsearch veritabanına ihtiyacınız bulunmaktadır. Localhost üzerine kurulum yapmanızı sağlayan `docker-compose.yaml` dosyası proje içerisinde tüm ayarları ile mevcuttur. Bir elasticsearch container oluştururken en çok dikkat etmemiz gereken key bağlantıyı sağlayacak credential bilgileridir.
Yaml dosyası üzerinde bu bilgilerin değiştirilmesi durumunda uygulamanın api katmanıdan bulunan `appsettings.Development.json` dosyasında ilgili alanda da değişiklik yapılmalıdır.

```json
// Elasticsearch üzerine bağlanmamızı sağlayacak ayarlar.
"Elasticsearch": {
  "Url": "http://localhost:9200", // Ulaşacağımız Elasticsearch adresi.
  "Client": "elastic", // Kullanıcı adı.
  "Secret": "changeme" // Parola.
}
```

Docker compose dosyasının bulunduğu dizinde açılacak bir terminalde `docker-compose up` komutunu çalıştırarak Elasticsearch ve Kibana konteynırlarının ayağa kalkmasını sağlayabilirsiniz. Bu işlemden sonra projeyi localhost üzerinde çalıştırabilir ve kullanabilirsiniz.