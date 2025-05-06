using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Login.DoLogin
{
    //teste unitário
    public class DoLoginUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            //armazenando resultados dos retornos dos parâmetros de UserBuilder para ser utilizado no teste Success
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(new RequestLoginJson
            {
                Email = user.Email,
                Password = password,
            });

            result.Should().NotBeNull();
            result.Tokens.AccessToken.Should().NotBeNullOrEmpty();
            result.Tokens.Should().NotBeNull();
            result.Name.Should().NotBeNullOrWhiteSpace().And.Be(user.Name);
        }

        [Fact]
        public async Task Error_Invalid_User()
        {
            var request = RequestLoginJsonBuilder.Build();
            var useCase = CreateUseCase();

            Func<Task> act = async () => { await useCase.Execute(request); };

            await act.Should().ThrowAsync<InvalidLoginException>()
                .Where(e => e.Message.Equals(ResourceMessagesExceptions.EMAIL_OR_PASSWORD_INVALID));
        }

        //utilizando a convenção de nullable para sinalizar que o objeto poderá retornar null
        private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var passwordEncrypter = PasswordEncripterBuilder.Build();
            var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
            var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var tokenRepository = new TokenRepositoryBuilder().Build(); // instanciando o TokenRepositoryBuilder

            if (user is not null)
                userReadOnlyRepositoryBuilder.GetByEmail(user);

            return new DoLoginUseCase(userReadOnlyRepositoryBuilder.Build(), passwordEncrypter, accessTokenGenerator, refreshTokenGenerator, unitOfWork, tokenRepository);
        }
    }
}
