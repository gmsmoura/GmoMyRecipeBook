using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;
// Exceção personalizada que representa o caso em que o refresh token não foi encontrado
public class RefreshTokenNotFoundException : MyRecipeBookException
{
    // Construtor da exceção que define a mensagem de erro com base em um recurso localizado
    public RefreshTokenNotFoundException()
        : base(ResourceMessagesExceptions.EXPIRED_SESSION) // Usa uma mensagem localizada para sessão expirada
    {
    }

    // Método sobrescrito que retorna uma lista contendo a mensagem de erro
    public override IList<string> GetErrorMessages() => [Message];

    // Método sobrescrito que define o código HTTP a ser retornado pela API nesse tipo de erro
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized; // HTTP 401 - Não autorizado
}

