using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repositories.Recipe;
public interface IRecipeReadOnlyRepository
{
    //filtrando pelos parâmetros do DTO e usuário
    Task<IList<Entities.Recipe>> Filter(Entities.User user, FilterRecipesDto filters);

    //devolvendo a receita pelo id e usuário
    Task<Entities.Recipe?> GetById(Entities.User user, long recipeId);
    Task<IList<Entities.Recipe>> GetForDashboard(Entities.User user);
}
