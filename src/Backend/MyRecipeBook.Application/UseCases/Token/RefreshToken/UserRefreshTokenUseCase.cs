using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Token.RefreshToken;
public class UseRefreshTokenUseCase : IUseRefreshTokenUseCase
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public UseRefreshTokenUseCase(
        IUnitOfWork unitOfWork, // Unidade de trabalho para gerenciar transações
        ITokenRepository tokenRepository, // Repositório de tokens para acessar e manipular tokens no banco de dados
        IRefreshTokenGenerator refreshTokenGenerator, // Gerador de refresh tokens para criar novos tokens
        IAccessTokenGenerator accessTokenGenerator) // Gerador de access tokens para criar novos tokens
    {
        _unitOfWork = unitOfWork;
        _tokenRepository = tokenRepository;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<ResponseTokensJson> Execute(RequestNewTokenJson request)
    {
        // Busca o refresh token enviado no repositório
        var refreshToken = await _tokenRepository.Get(request.RefreshToken);

        // Se não encontrar o token, lança exceção
        if (refreshToken is null)
            throw new RefreshTokenNotFoundException();

        // Calcula a data de expiração do token com base na data de criação, neste caso são 7 dias
        var refreshTokenValidUntil = refreshToken.CreatedOn.AddDays(MyRecipeBookRuleConstants.REFRESH_TOKEN_EXPIRATION_DAYS);

        // Se o token já expirou, lança exceção
        if (DateTime.Compare(refreshTokenValidUntil, DateTime.UtcNow) < 0)
            throw new RefreshTokenExpiredException();

        // Gera um novo refresh token para substituir o anterior
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Value = _refreshTokenGenerator.Generate(),       // Gera um novo valor seguro
            UserId = refreshToken.UserId                     // Associa ao mesmo usuário do token antigo
        };

        // Salva o novo token no repositório
        await _tokenRepository.SaveNewRefreshToken(newRefreshToken);

        // Realiza o commit das alterações na base de dados
        await _unitOfWork.Commit();

        // Retorna um novo access token e o novo refresh token
        return new ResponseTokensJson
        {
            AccessToken = _accessTokenGenerator.Generate(refreshToken.User.UserIdentifier),
            RefreshToken = newRefreshToken.Value
        };
    }
}
