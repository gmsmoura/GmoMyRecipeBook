using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenRepository _tokenRepository;

        public DoLoginUseCase(
            IUserReadOnlyRepository repository
            , IPasswordEncripter passwordEncripter
            , IAccessTokenGenerator accessTokenGenerator
            , IRefreshTokenGenerator refreshTokenGenerator
            , IUnitOfWork unitOfWork
            , ITokenRepository tokenRepository)
        {
            _repository = repository;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _unitOfWork = unitOfWork;
            _tokenRepository = tokenRepository;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            // Busca o usuário no repositório pelo e-mail informado na requisição
            var user = await _repository.GetByEmail(request.Email);

            // Verifica se o usuário não foi encontrado OU se a senha está inválida (se não der match da senha informada com a senha criptografada)
            if (user is null || _passwordEncripter.IsValid(request.Password, user.Password).IsFalse())
                throw new InvalidLoginException();

            // Cria e salva um novo refresh token para o usuário
            var refreshToken = await CreateAndSaveRefreshToken(user);

            // Retorna os dados do usuário autenticado, incluindo o nome, e-mail e tokens
            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Email = user.Email,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier), // Gera e retorna o token de acesso (JWT)
                    RefreshToken = refreshToken // Retorna o refresh token recém-criado
                }
            };
        }

        // Método responsável por criar e salvar um novo refresh token para um usuário autenticado
        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            // Gera um novo token usando o gerador de refresh tokens
            var refreshToken = new Domain.Entities.RefreshToken
            {
                Value = _refreshTokenGenerator.Generate(),
                UserId = user.Id
            };

            // Salva o token no repositório vinculado ao usuário
            await _tokenRepository.SaveNewRefreshToken(refreshToken);

            // Confirma as alterações no banco de dados
            await _unitOfWork.Commit();

            // Retorna o token recém-criado
            return refreshToken.Value;
        }
    }
}
