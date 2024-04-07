namespace Elasticsearch.Core.ResultModels.BaseResultModel;

/// <summary>Metotların geri dönüşlerinde kullanacağı ortak model tipi. Bu modeli base alan diğer modeller dönüşler için kullanılacaktır.<summary>
/// <summary>The common model type that methods will use in their return. Other models based on this model will be used for turns.<summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class BaseResult<T>
{
    public BaseResult() => Messages = new List<string>();

    /// <summary>Gelen isteğe karşılık dönülecek olan cevaptaki durum kodu.</summary>
    /// <summary>Status code in the response to be returned in response to the incoming request.</summary>
    public virtual HttpStatusCode ResultType { get; set; }

    /// <summary>İsteğe verilen cevabın başarı durumu.</summary>
    /// <summary>The success status of the response to the request.</summary>
    public virtual bool Success { get; set; }

    /// <summary>Geri dönüşte verilecek cevaptaki mesaj listesi.</summary>
    /// <summary>The list of messages in the reply to be given in the return.</summary>
    public virtual List<string> Messages { get; set; }

    /// <summary>Başarılı bir işlem sonrasında kullanıcıya dönülmesi beklenen veri modeli.</summary>
    /// <summary>The data model that is expected to be returned to the user after a successful transaction.</summary>
    [Required(AllowEmptyStrings = true)]
    public virtual T? Data { get; set; }
}