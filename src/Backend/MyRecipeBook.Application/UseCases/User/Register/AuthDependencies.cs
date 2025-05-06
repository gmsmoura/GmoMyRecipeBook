using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class AuthDependencies
    {
        public  IAccessTokenGenerator AccessTokenGenerator { get; set; }
        public  IRefreshTokenGenerator RefreshTokenGenerator { get; set; }
        public  ITokenRepository TokenRepository { get; set; }

        public AuthDependencies(
            IAccessTokenGenerator accessTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            ITokenRepository tokenRepository)
        {
            AccessTokenGenerator = accessTokenGenerator;
            RefreshTokenGenerator = refreshTokenGenerator;
            TokenRepository = tokenRepository;
        }
    }
}
