namespace Elasticsearch.Core.Configs;

/// <summary>Elasticsearch üzerine bağlanmamızı sağlayacak ayarları barındıran sınıf.</summary>
public class ElasticOptions
{
    /// <summary>Appsettings altında bulunan ve bilgilerin alındığı alan adı.</summary>
    public string SectionName { get; } = "Elasticsearch";

    /// <summary>Mevcutta kullanılan veri tabanının adresi.</summary>
    public string Url { get; set; } = null!;

    /// <summary>Bağlantı için gerekli kullanıcı adı.</summary>
    public string Client { get; set; } = null!;

    /// <summary>Bağlantı için gerekli kullanıcı parola.</summary>
    public string Secret { get; set; } = null!;
}