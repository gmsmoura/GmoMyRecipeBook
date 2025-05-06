using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class RefreshTokenExpiredException : MyRecipeBookException
{
    
    public RefreshTokenExpiredException()
        : base(ResourceMessagesExceptions.INVALID_SESSION) 
    {
    }

    
    public override IList<string> GetErrorMessages() => [Message];

    
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}

