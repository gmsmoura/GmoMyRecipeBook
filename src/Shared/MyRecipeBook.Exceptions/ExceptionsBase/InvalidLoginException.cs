using MyRecipeBook.Exceptions.ExceptionsBase;
using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase
{
    public class InvalidLoginException : MyRecipeBookException
    {
        //tratando o retorno customizado da mensagem de erro com o retorno da string dentro de ResourceMessagesExceptions
        public InvalidLoginException() : base(ResourceMessagesExceptions.EMAIL_OR_PASSWORD_INVALID)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
