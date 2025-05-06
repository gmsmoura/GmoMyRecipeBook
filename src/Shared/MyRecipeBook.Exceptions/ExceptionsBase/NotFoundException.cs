using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;
public class NotFoundException : MyRecipeBookException
{
    //classe responsável para receber mensagem de erro e repassar para a classe 
    public NotFoundException(string message) : base(message)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
}