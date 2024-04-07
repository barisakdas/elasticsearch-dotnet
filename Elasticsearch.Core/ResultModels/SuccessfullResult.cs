namespace Elasticsearch.Core.ResultModels;

/// <summary>Metotların geri dönüşlerinde kullanacağı başarılı model tipi.<summary>
/// <summary>The type of successful model that the methods will use in their return.<summary>
public class SuccessfullResult<T> : BaseResult<T>
{
    private readonly T _data;

    public SuccessfullResult(T data) : base() => _data = data;

    /// <summary>Gelen isteğe karşılık dönülecek olan cevaptaki durum kodu.</summary>
    /// <summary>Status code in the response to be returned in response to the incoming request.</summary>
    public override HttpStatusCode ResultType => HttpStatusCode.OK;

    /// <summary>İsteğe verilen cevabın başarı durumu.</summary>
    /// <summary>The success status of the response to the request.</summary>
    public override bool Success => true;

    /// <summary>Geri dönüşte verilecek cevaptaki mesaj listesi.</summary>
    /// <summary>The list of messages in the reply to be given in the return.</summary>
    public override List<string> Messages { get; set; } = new List<string>();

    /// <summary>Başarılı bir işlem sonrasında kullanıcıya dönülmesi beklenen veri modeli.</summary>
    /// <summary>The data model that is expected to be returned to the user after a successful transaction.</summary>
    public override T Data => _data;
}