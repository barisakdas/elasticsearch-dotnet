namespace Elasticsearch.Api.Extensions;

/// <summary>Elasticsearch için oluşturulacak Client'ı DI container'a geçmek için oluşturduğumuz yardımcı sınıf.</summary>
public static class ElasticsearchExtensions
{
    /// <summary>
    /// Elasticsearch'ün yeni geliştirdiği ve Nest yerine destek verdiği bir kütüphane var.
    /// Elastic.Clients.Elasticsearch
    /// Bu kütüphane sadece Elasticsearch'ün 8 ve üstü versiyonlarında destekli. Alt versiyonlar için Nest kullanmaya devam edebiliriz.
    /// </summary>
    public static void AddElasticsearchClient_ElasticClients(this IServiceCollection services, IConfiguration configuration)
    {
        // ElasticOptions sınıfından bir örnek oluştur ve configuration'dan URL'i ata.
        var elasticOptions = new ElasticOptions();
        configuration.GetSection(elasticOptions.SectionName).Bind(elasticOptions);

        var settings = new ElasticsearchClientSettings(new Uri(elasticOptions.Url));
        settings.Authentication(new BasicAuthentication(elasticOptions.Client, elasticOptions.Secret));

        // Not: Bu ElasticClient `ThreadSafe` bir sınıftır. Yani başka bir thread içerisinden bu sınıfa erişilebilir.
        // Yani Multi Thread çalışılabilir.
        var client = new ElasticsearchClient(settings);

        // ElasticsearchClient nesnesini services koleksiyonuna singleton olarak ekliyoruz.
        // Bunu Singleton olarak eklememizin sebebi Elasticsearch resmi dökümantasyonlarında bu şekilde kullanılmasının önerilmesidir.
        services.AddSingleton(client);
    }
}

/*
NOTE: WHAT IS THEREAD SAFE CLASS IN C#

C# dilinde “thread safe” bir sınıf, birden fazla iş parçacığı tarafından eş zamanlı olarak güvenli bir şekilde erişilebilen ve
kullanılabilecek şekilde tasarlanmış bir sınıftır. Bir sınıfın veya kod parçasının thread safe olduğu söylenirken,
bu kodun birden fazla iş parçacığı tarafından aynı anda çağrıldığında işlevselliğinin bozulmadan doğru bir şekilde çalıştığı anlamına gelir.

Thread safe bir sınıf tasarlarken dikkate alınması gereken bazı önemli noktalar şunlardır:
* Paylaşılan Verilerin Korunması: Sınıf içindeki paylaşılan verilerin (örneğin, sınıf değişkenleri) uygun şekilde korunması gerekir.
  Bu genellikle lock anahtar kelimesi kullanılarak yapılır.
* Durum Yönetimi: Sınıfın durumunu yönetirken, iş parçacıkları arasında yarış koşullarını (race conditions) önlemek için senkronizasyon
  mekanizmaları kullanılmalıdır.
* İş Parçacığı Yerel Depolama: Bazı durumlarda, her iş parçacığı için ayrı veri depolama alanları kullanmak, iş parçacığı güvenliği sağlamak
  için etkili bir yöntem olabilir.
* Koleksiyonlar: .NET’te, System.Collections.Concurrent ad alanı altında bulunan iş parçacığı güvenli koleksiyonlar kullanılabilir3.

Özetle, thread safe bir sınıf, birden fazla iş parçacığı tarafından aynı anda erişildiğinde doğru çalışmaya devam eden ve iş parçacıkları
arasında veri bütünlüğünü koruyan bir sınıftır. Bu tür sınıflar, özellikle çok iş parçacıklı uygulamalarda önemlidir ve veri tutarlılığını
sağlamak için dikkatli bir şekilde tasarlanmalıdır.
*/