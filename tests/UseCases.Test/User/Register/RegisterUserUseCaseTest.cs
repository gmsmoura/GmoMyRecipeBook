using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.Register
{
    public class RegisterUserUseCaseTest
    {
        
        [Fact]
        public async Task Success()
        {
            
            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase();

            
            var result = await useCase.Execute(request);

            
            result.Should().NotBeNull();

            
            result.Tokens.Should().NotBeNull();
            result.Tokens.AccessToken.Should().NotBeNullOrEmpty();

            
            result.Name.Should().Be(request.Name);
        }

        
        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase(request.Email);

            
            Func<Task> act = async () => await useCase.Execute(request);

            
            
            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                 .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));
        }

        
        [Fact]
        public async Task Error_Name_Empty()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            
            request.Name = string.Empty;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.Execute(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                 .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.NAME_EMPTY));
        }

        
        private static RegisterUserUseCase CreateUseCase(string? email = null, string? name = null)
        {
            
            var mapper = MapperBuilder.Build();
            var passwordEncripter = PasswordEncripterBuilder.Build();
            var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
            
            
            var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
            var tokenRepository = new TokenRepositoryBuilder().Build(); 

            
            var authDependencies = new AuthDependencies(accessTokenGenerator, refreshTokenGenerator, tokenRepository);

            
            if (string.IsNullOrEmpty(email) == false)
                readRepositoryBuilder.ExistActiveUserWithEmail(email);     

            
            return new RegisterUserUseCase(writeRepository, readRepositoryBuilder.Build(), unitOfWork, passwordEncripter, mapper, authDependencies);
        }
    }
}
