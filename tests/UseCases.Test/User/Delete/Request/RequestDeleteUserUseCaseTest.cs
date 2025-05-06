using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.ServiceBus;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Delete.Request;
using Xunit;

namespace UseCases.Test.User.Delete.Request;
public class RequestDeleteUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Cria um usuário fictício para o teste (builder padrão usado em testes para gerar dados)
        (var user, _) = UserBuilder.Build();

        // Cria a instância do caso de uso com dependências injetadas simuladas (fakes/mocks)
        var useCase = CreateUseCase(user);

        // Define uma ação assíncrona que executa o caso de uso de requisição de exclusão de usuário
        var act = async () => await useCase.Execute();

        // Verifica se a execução da ação não lança nenhuma exceção
        await act.Should().NotThrowAsync();

        // Verifica se, após a execução, o usuário foi desativado (Active = false)
        user.Active.Should().BeFalse();
    }

    private static RequestDeleteUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        // Cria uma instância simulada da fila que envia a mensagem para exclusão real
        var queue = DeleteUserQueueBuilder.Build();

        // Cria uma instância simulada da UnitOfWork (controle de transações)
        var unitOfWork = UnitOfWorkBuilder.Build();

        // Cria o contexto do usuário logado, com base no usuário gerado
        var loggedUser = LoggedUserBuilder.Build(user);

        // Cria um repositório fake para simular atualização do usuário no banco (por ID)
        var repository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();

        // Retorna a instância do caso de uso que orquestra a solicitação de exclusão de conta
        return new RequestDeleteUserUseCase(queue, repository, loggedUser, unitOfWork);
    }
}
