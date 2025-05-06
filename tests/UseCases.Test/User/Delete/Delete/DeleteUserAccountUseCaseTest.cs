using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using MyRecipeBook.Domain.Repositories.User;
using Xunit;

namespace UseCases.Test.User.Delete.Delete;
public class DeleteUserAccountUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Cria um usuário de teste utilizando o UserBuilder
        (var user, _) = UserBuilder.Build();

        // Cria a instância do use case (caso de uso de exclusão de usuário)
        var useCase = CreateUseCase();

        // Define uma ação assíncrona que executa o método de exclusão passando o identificador do usuário
        var act = async () => await useCase.Execute(user.UserIdentifier);

        // Verifica se a execução do método não lança nenhuma exceção
        await act.Should().NotThrowAsync();
    }

    private static DeleteUserAccountUseCase CreateUseCase()
    {
        // Cria a instância do UnitOfWork para controle de transações
        var unitOfWork = UnitOfWorkBuilder.Build();

        // Cria a instância do serviço de armazenamento (ex: Azure Blob Storage)
        var blobStorageService = new BlobStorageServiceBuilder().Build();

        // Cria o repositório responsável apenas pela exclusão do usuário
        var repository = UserDeleteOnlyRepository.Build();

        // Retorna a instância do caso de uso de exclusão de conta, injetando as dependências necessárias
        return new DeleteUserAccountUseCase(repository, blobStorageService, unitOfWork);
    }
}
