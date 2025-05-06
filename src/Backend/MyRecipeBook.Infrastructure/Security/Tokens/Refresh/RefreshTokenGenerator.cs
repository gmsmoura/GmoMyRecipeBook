using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Refresh;
// Classe responsável por gerar um novo Refresh Token
public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    // Método que gera um refresh token único em formato Base64
    public string Generate() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());

    /*
     Explicação:
     - Guid.NewGuid() gera um identificador único global.
     - .ToByteArray() converte o GUID em um array de bytes.
     - Convert.ToBase64String(...) transforma esses bytes em uma string segura para ser usada como token.
     
     Isso garante que cada token seja único, difícil de prever e seguro para uso em URLs e headers.
    */
}
