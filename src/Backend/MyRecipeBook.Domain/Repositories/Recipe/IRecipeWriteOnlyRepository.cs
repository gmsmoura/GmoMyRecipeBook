using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repositories.Recipe;
public interface IRecipeWriteOnlyRepository
{
    Task Add(Entities.Recipe recipe);
    Task<IList<Entities.Recipe>> Filter(Entities.User user, FilterRecipesDto filters);
    Task Delete(long recipeId);
}
