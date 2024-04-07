namespace Elasticsearch.Api.Extensions;

public static class ControllerExtensions
{
    /// <summary>Api içindeki controllerdaki gelen veriyi kontrol ederek dönüş tipini ayarlayan yardımcı metot.</summary>
    /// <summary>Helper method that checks the incoming data in the API controller and sets the return type.</summary>
    public static ActionResult FromResult<T>(this ControllerBase controller, BaseResult<T> result)
    {
        switch (result.ResultType)
        {
            case HttpStatusCode.OK:
                if (result.Data == null)
                    return controller.NoContent();
                else
                    return controller.Ok(result);

            case HttpStatusCode.NoContent:
                return controller.Ok(result);

            case HttpStatusCode.BadRequest:
                return controller.BadRequest(result);

            case HttpStatusCode.NotFound:
                return controller.NotFound(result);

            case HttpStatusCode.Unauthorized:
                return controller.Unauthorized(result);

            default:
                throw new Exception("An unhandled result has occurred as a result of a service call.");
        }
    }
}