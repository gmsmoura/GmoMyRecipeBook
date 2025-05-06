using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using System.Linq;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeUpdateOnlyRepository
    {
        private readonly MyRecipeBookDbContext _dbContext;
        public RecipeRepository(MyRecipeBookDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Recipe recipe) => await _dbContext.Recipes.AddAsync(recipe);

        public async Task Delete(long recipeId)
        {
            //buscando a receita pelo id e removendo com Remove()
            var recipe = await _dbContext.Recipes.FindAsync(recipeId);

            _dbContext.Recipes.Remove(recipe!);
        }

        public async Task<IList<Recipe>> Filter(User user, FilterRecipesDto filters)
        {
            //utilizando AsNoTracking para não permitir atualização de dados na entidade e aplicar melhoria na performance
            var query = _dbContext
                .Recipes
                .AsNoTracking()
                //utilização do include para incluir o join na consulta e trazer o relacionamento de outras tabelas
                .Include(recipe => recipe.Ingredients)
                .Include(recipe => recipe.DishTypes)
                .Where(recipe => recipe.Active && recipe.UserId == user.Id);

            //condicional para incluir novas verificações e verificar se existe nível de difficulty em alguma receita
            if (filters.Difficulties.Any()) 
            {
                //utilizando recipe.Difficulty.HasValue para eliminar as possibilidades de receitas que não tenham o nível de difficulty preenchido
                //e em seguida valida se contém o value especificado nos itens de receitas para incluir no retorno da busca
                query = query.Where(recipe => recipe.Difficulty.HasValue && filters.Difficulties.Contains(recipe.Difficulty.Value));
            }

            if (filters.CookingTimes.Any())
            {
                //verificação semelhante a de cima para filtrar o cookingTime
                query = query.Where(recipe => recipe.CookingTime.HasValue && filters.CookingTimes.Contains(recipe.CookingTime.Value));
            }

            if (filters.DishTypes.Any()) 
            {
                //utilizando com Any() devido dishTypes ser uma lista e retornar apenas receitas que existem um dishType
                //e em seguida valida se contém o value(Type) especificado nos itens de receitas para incluir no retorno da busca
                query = query.Where(recipe => recipe.DishTypes.Any(dishType => filters.DishTypes.Contains(dishType.Type)));
            }

            //utilizando a sobreposição de resultados com query = query... devido existir uma consulta pós a outra e no fim mesclar os resultados

            //se o RecipeTitle_Ingredient do DTO não for vazio
            if (filters.RecipeTitle_Ingredient.NotEmpty())
            {
                //valida se o Title contém o value de RecipeTitle_Ingredient ou se via Any() validando se a lista de Ingredients existe algum valor e contém algum item de ingredient
                query = query.Where(
                    recipe => recipe.Title.Contains(filters.RecipeTitle_Ingredient)
                    || recipe.Ingredients.Any(ingredient => ingredient.Item.Contains(filters.RecipeTitle_Ingredient)));
            }

            //acima está sendo preparado a consulta, a abaixo com await query.ToListAsync() a busca está sendo efetivada de fato no banco de dados
            return await query.ToListAsync();
        }

        //funçao GetById específica para uso da interface IRecipeReadOnlyRepository com uso de AsNoTracking para não permitir atualização de dados na entidade e aplicar melhoria na performance
        async Task<Recipe?> IRecipeReadOnlyRepository.GetById(User user, long recipeId)
        {
            return await GetFullRecipe()
                .AsNoTracking()
                .FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == user.Id);
        }

        //função GetById específica para uso da interface IRecipeUpdateOnlyRepository
        async Task<Recipe?> IRecipeUpdateOnlyRepository.GetById(User user, long recipeId)
        {
            return await GetFullRecipe()
                .FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == user.Id);
        }

        public void Update(Recipe recipe) => _dbContext.Recipes.Update(recipe);

        //função para retornar as cinco primeiras receitas com o método Take() para o dashboard com base na data de criação
        public async Task<IList<Recipe>> GetForDashboard(User user)
        {
            return await _dbContext
                .Recipes
                .AsNoTracking()
                .Include(recipe => recipe.Ingredients)
                .Where(recipe => recipe.Active && recipe.UserId == user.Id)
                .OrderByDescending(r => r.CreatedOn)
                .Take(5)
                .ToListAsync();
        }

        //função auxiliar para retornar a receita completa com todos os relacionamentos nas funções de GetById acima para não haver código duplicado
        private IIncludableQueryable<Recipe, IList<DishType>> GetFullRecipe()
        {
            return _dbContext
                .Recipes
                .Include(recipe => recipe.Ingredients)
                .Include(recipe => recipe.Instructions)
                .Include(recipe => recipe.DishTypes);
        }
    }
}
