namespace Elasticsearch.Core.ResultModels;

public class BadRequestResult<T> : BaseResult<T>
{
    public BadRequestResult() : base()
    {
    }

    public BadRequestResult(string message) => Messages.Add(message);

    /// <summary>Gelen isteğe karşılık dönülecek olan cevaptaki durum kodu.</summary>
    /// <summary>Status code in the response to be returned in response to the incoming request.</summary>
    public override HttpStatusCode ResultType => HttpStatusCode.BadRequest;

    /// <summary>İsteğe verilen cevabın başarı durumu.</summary>
    /// <summary>The success status of the response to the request.</summary>
    public override bool Success => true;

    /// <summary>Geri dönüşte verilecek cevaptaki mesaj listesi.</summary>
    /// <summary>The list of messages in the reply to be given in the return.</summary>
    public override List<string> Messages { get; set; } = new List<string>();

    /// <summary>Başarılı bir işlem sonrasında kullanıcıya dönülmesi beklenen veri modeli.</summary>
    /// <summary>The data model that is expected to be returned to the user after a successful transaction.</summary>
    public override T Data => default(T);
}