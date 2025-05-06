using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Recipe.Delete;
public class DeleteRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var act = async () => { await useCase.Execute(recipe.Id); };

        //utilizando NotThrowAsync para verificar se não houve exceção e para o caso de NoContent (que não tem um retorno de outro status, como 200)
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_Recipe_NotFound()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(recipeId: 1000); };

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.Message.Equals(ResourceMessagesExceptions.RECIPE_NOT_FOUND));
    }

    private static DeleteRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repositoryRead = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var repositoryWrite = RecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

        return new DeleteRecipeUseCase(loggedUser, repositoryRead, repositoryWrite, unitOfWork, blobStorage);
    }
}
