using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    //herança com a interface IRegisterUseCase
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        //underline para sinalizar que é uma prop privada
        private readonly IUserWriteOnlyRepository _writeOnlyRepository;
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ITokenRepository _tokenRepository;

        //construtor para receber as classes de injeção de dependências do banco de dados
        public RegisterUserUseCase(
            IUserWriteOnlyRepository writeOnlyRepository,
            IUserReadOnlyRepository readOnlyRepository,
            IUnitOfWork unitOfWork,
            IPasswordEncripter passwordEncripter,
            IMapper mapper,
            AuthDependencies authDependencies)
        {
            _writeOnlyRepository = writeOnlyRepository;
            _readOnlyRepository = readOnlyRepository;
            _unitOfWork = unitOfWork;
            _passwordEncripter = passwordEncripter;
            _mapper = mapper;

            _accessTokenGenerator = authDependencies.AccessTokenGenerator;
            _refreshTokenGenerator = authDependencies.RefreshTokenGenerator;
            _tokenRepository = authDependencies.TokenRepository;
        }

        //retornando 'classe ResponseRegisteredUserJson' e como parâmetro será necessário chamar a regra de negócio 'classe controller RequestRegisterUserJson request'
        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            //validando a request
            await Validate(request);

            //mapeando a request em uma entidade (representação do banco de dados),
            //parâmetros dentro de <> se trata do destino de onde este objeto irá: <Domain.Entities.User> e
            //no parâmetro dos parenteses se trata da origem de onde está vindo o objeto: request
            var user = _mapper.Map<Domain.Entities.User>(request);

            //criptografando a senha chamando o método Encrypt() que foi instanciado pela classe PasswordEncripter e passando como parâmetro o Password da request e utilizando via injeção de depência _passwordEncripter
            user.Password = _passwordEncripter.Encrypt(request.Password);

            //salvando no banco de dados, utilizando o await devido ser uma Task e ser necessário aguardar a finalização da mesma para rodar o Add
            await _writeOnlyRepository.Add(user);

            //após a chamada da adição do user está sendo chamado o Commit() para por fim persistir os dados no banco de dados
            await _unitOfWork.Commit();

            //chamando o método CreateAndSaveRefreshToken() para criar e salvar um novo refresh token para o usuário recém-criado
            var refreshToken = await CreateAndSaveRefreshToken(user);

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Email = user.Email,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                    RefreshToken = refreshToken
                }
            };
        }

        // Método responsável por criar e salvar um novo refresh token para um usuário
        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            // Gera um novo token usando o gerador de refresh tokens
            var refreshToken = _refreshTokenGenerator.Generate();

            // Salva o token no repositório vinculado ao usuário
            await _tokenRepository.SaveNewRefreshToken(new RefreshToken
            {
                Value = refreshToken,     // Token gerado
                UserId = user.Id          // ID do usuário associado
            });

            // Confirma as alterações no banco de dados
            await _unitOfWork.Commit();

            // Retorna o token recém-criado
            return refreshToken;
        }

        //método privado para validação da requisição recebida
        private async Task Validate(RequestRegisterUserJson request)
        {
            #region ValidateErrors

            //instanciando a classe RegisterUserValidator
            var validator = new RegisterUserValidator();

            //retornando o resultado validado
            var result = validator.Validate(request);

            //validando se user já existe nos registros
            var emailExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);
           
            //Add() para adicionar o erro de emailExist a listagem de exceções caso exista outras com o ValidationFailure e os dois parâmetros para validação
            if (emailExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));

            //se for IsValid == false envia o retorno dos erros
            if (!result.IsValid)
            {
                //guardando errors para retornar uma string da mensagem do erro, método ToList() para receber uma lista caso contrário o parâmetro do ErrorOnValidationException indicará erro que estará null
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorMessages);

            }

            #endregion
        }
    }
}
