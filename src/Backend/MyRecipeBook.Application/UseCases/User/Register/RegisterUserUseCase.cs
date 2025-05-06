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
    
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        
        private readonly IUserWriteOnlyRepository _writeOnlyRepository;
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ITokenRepository _tokenRepository;

        
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

        
        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            
            await Validate(request);

            
            
            
            var user = _mapper.Map<Domain.Entities.User>(request);

            
            user.Password = _passwordEncripter.Encrypt(request.Password);

            
            await _writeOnlyRepository.Add(user);

            
            await _unitOfWork.Commit();

            
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

        
        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            
            var refreshToken = _refreshTokenGenerator.Generate();

            
            await _tokenRepository.SaveNewRefreshToken(new RefreshToken
            {
                Value = refreshToken,     
                UserId = user.Id          
            });

            
            await _unitOfWork.Commit();

            
            return refreshToken;
        }

        
        private async Task Validate(RequestRegisterUserJson request)
        {
            #region ValidateErrors

            
            var validator = new RegisterUserValidator();

            
            var result = validator.Validate(request);

            
            var emailExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);
           
            
            if (emailExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));

            
            if (!result.IsValid)
            {
                
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorMessages);

            }

            #endregion
        }
    }
}
