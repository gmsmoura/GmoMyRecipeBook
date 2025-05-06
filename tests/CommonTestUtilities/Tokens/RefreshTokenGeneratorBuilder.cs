using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;

namespace CommonTestUtilities.Tokens;
public class RefreshTokenGeneratorBuilder
{
    // Método que cria uma instância de IRefreshTokenGenerator
    public static IRefreshTokenGenerator Build() => new RefreshTokenGenerator();
}
