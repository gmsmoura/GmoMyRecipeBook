using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;
// Exceção personalizada que representa o caso em que o refresh token expirou
public class RefreshTokenExpiredException : MyRecipeBookException
{
    // Construtor da exceção que define uma mensagem de erro utilizando um recurso localizado
    public RefreshTokenExpiredException()
        : base(ResourceMessagesExceptions.INVALID_SESSION) // Usa uma mensagem localizada indicando sessão inválida
    {
    }

    // Retorna a mensagem de erro como uma lista (padrão utilizado nas exceções do projeto)
    public override IList<string> GetErrorMessages() => [Message];

    // Define o status HTTP a ser retornado quando essa exceção ocorre (403 - Forbidden)
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}

