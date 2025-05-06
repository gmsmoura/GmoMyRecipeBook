using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class RefreshTokenNotFoundException : MyRecipeBookException
{
    
    public RefreshTokenNotFoundException()
        : base(ResourceMessagesExceptions.EXPIRED_SESSION) 
    {
    }

    
    public override IList<string> GetErrorMessages() => [Message];

    
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized; 
}

