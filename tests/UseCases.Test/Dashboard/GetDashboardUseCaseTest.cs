using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Dashboard;

namespace UseCases.Test.Dashboard;
public class GetDashboardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        //builder e coollection para receber um usuário e uma lista de receitas
        (var user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute();

        //verificação de que o resultado não é nulo e que a lista de receitas não é nula, tem mais de 0 itens,
        //todos os itens são únicos e todos os itens não são nulos
        result.Should().NotBeNull();
        result.Recipes.Should()
            .HaveCountGreaterThan(0)
            .And.OnlyHaveUniqueItems(recipe => recipe.Id)
            .And.AllSatisfy(recipe =>
            {
                recipe.Id.Should().NotBeNullOrWhiteSpace();
                recipe.Title.Should().NotBeNullOrWhiteSpace();
                recipe.AmountIngredients.Should().BeGreaterThan(0);
                recipe.ImageUrl.Should().NotBeNullOrWhiteSpace();
            });
    }

    private static GetDashboardUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = new RecipeReadOnlyRepositoryBuilder().GetForDashboard(user, recipes).Build();
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipes).Build();

        return new GetDashboardUseCase(repository, mapper, loggedUser, blobStorage);
    }
}
