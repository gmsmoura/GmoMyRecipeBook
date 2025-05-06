using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Image;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using UseCases.Test.Recipe.InlineDatas;
using Xunit;

namespace UseCases.Test.Recipe.Image;
public class AddUpdateImageCoverUseCaseTest
{
    // Teste que garante o sucesso ao adicionar ou atualizar imagem em uma receita válida com imagem
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))] // Fornece arquivos de imagem válidos
    public async Task Success(IFormFile file)
    {
        (var user, _) = UserBuilder.Build(); // Cria um usuário fake
        var recipe = RecipeBuilder.Build(user); // Cria uma receita associada ao usuário

        var useCase = CreateUseCase(user, recipe); // Instancia o caso de uso com mocks

        Func<Task> act = async () => await useCase.Execute(recipe.Id, file); // Ação a ser testada

        await act.Should().NotThrowAsync(); // Verifica que nenhuma exceção foi lançada
    }

    // Teste que valida o comportamento ao adicionar uma imagem em uma receita que ainda não tinha imagem
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success_Recipe_Did_Not_Have_Image(IFormFile file)
    {
        (var user, _) = UserBuilder.Build(); // Cria usuário fake
        var recipe = RecipeBuilder.Build(user); // Cria receita fake
        recipe.ImageIdentifier = null; // Simula que a receita não tinha imagem antes

        var useCase = CreateUseCase(user, recipe); // Instancia o caso de uso

        Func<Task> act = async () => await useCase.Execute(recipe.Id, file); // Executa o uso com imagem

        await act.Should().NotThrowAsync(); // Verifica que a execução foi bem-sucedida

        recipe.ImageIdentifier.Should().NotBeNullOrWhiteSpace(); // Verifica que a imagem foi atribuída corretamente
    }

    // Teste que verifica o comportamento quando a receita não é encontrada (ID inválido)
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Error_Recipe_NotFound(IFormFile file)
    {
        (var user, _) = UserBuilder.Build(); // Cria usuário

        var useCase = CreateUseCase(user); // Cria o use case sem nenhuma receita cadastrada

        var act = async () => await useCase.Execute(1, file); // Tenta executar com ID inexistente

        (await act.Should().ThrowAsync<NotFoundException>()) // Deve lançar exceção de "não encontrado"
            .Where(e => e.Message.Equals(ResourceMessagesExceptions.RECIPE_NOT_FOUND)); // Confirma a mensagem de erro
    }

    // Teste que garante que arquivos não permitidos (ex: .txt) são rejeitados corretamente
    [Fact]
    public async Task Error_File_Is_Txt()
    {
        (var user, _) = UserBuilder.Build(); // Cria usuário
        var recipe = RecipeBuilder.Build(user); // Cria receita

        var useCase = CreateUseCase(user, recipe); // Instancia o caso de uso

        var file = FormFileBuilder.Txt(); // Gera um arquivo .txt (inválido)

        var act = async () => await useCase.Execute(recipe.Id, file); // Tenta usar imagem inválida

        (await act.Should().ThrowAsync<ErrorOnValidationException>()) // Espera exceção de validação
            .Where(e => e.GetErrorMessages().Count == 1 &&
                e.GetErrorMessages().Contains(ResourceMessagesExceptions.ONLY_IMAGES_ACCEPTED)); // Verifica a mensagem de erro
    }

    private static AddUpdateImageCoverUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var blobStorage = new BlobStorageServiceBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new AddUpdateImageCoverUseCase(loggedUser, repository, unitOfWork, blobStorage);
    }
}
