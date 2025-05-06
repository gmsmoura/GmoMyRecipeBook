using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.ChangePassword
{
    public class ChangePasswordUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            //utilizando a entidade user com UserBuilder
            (var user, var password) = UserBuilder.Build();

            var request = RequestChangePasswordJsonBuilder.Build();
            request.Password = password;

            var useCase = CreateUseCase(user);

            //armazenando a função Execute() na variável act 
            Func<Task> act = async () => await useCase.Execute(request);

            //executando a função da variável act sem executar nenhum exception, isso para funções que não devolvem valor
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Error_NewPassword_Empty()
        {
            (var user, var password) = UserBuilder.Build();

            //instaciando a request manualmente para forçar o newPassword empty
            var request = new RequestChangePasswordJson
            {
                Password = password,
                NewPassword = string.Empty
            };

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(request); };

            //ErrorsMessages.Count == 1 para informar que será somente uma mensagem de erro a ser retornado
            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.PASSWORD_EMPTY));
        }

        [Fact]
        public async Task Error_CurrentPassword_Different()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestChangePasswordJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => { await useCase.Execute(request); };

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesExceptions.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
        }

        private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var unitOfWork = UnitOfWorkBuilder.Build();

            //instaciando UserUpdateOnlyRepositoryBuilder para captura do user por Id
            var userUpdateRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var passwordEncripter = PasswordEncripterBuilder.Build();

            //parametros sendo recebidosa com: user logado, userUpdateRepository, userReadOnlyRepositoryBuilder e unidade de trabalho
            return new ChangePasswordUseCase(loggedUser, passwordEncripter, userUpdateRepository, unitOfWork);
        }
    }
}
