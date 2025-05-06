using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtilities.Entities;
public class RecipeBuilder
{
    public static IList<Recipe> Collection(User user, uint count = 2)
    {
        var list = new List<Recipe>(); // Cria uma lista vazia para armazenar as receitas

        if (count == 0)                // Garante que sempre tenha pelo menos 1 receita
            count = 1;

        var recipeId = 1;              // Inicializa o contador de ID das receitas

        for (int i = 0; i < count; i++) // Loop que gera 'count' receitas
        {
            var fakeRecipe = Build(user); // Cria uma receita fake usando o método Build
            fakeRecipe.Id = recipeId++;   // Atribui um ID incremental para cada receita

            list.Add(fakeRecipe);         // Adiciona a receita à lista
        }

        return list; // Retorna a lista completa
    }

    public static Recipe Build(User user)
    {
        return new Faker<Recipe>() // Inicia o builder de objeto fake para a entidade Recipe
            .RuleFor(r => r.Id, () => 1) // Define o ID como 1 (será sobrescrito na Collection)
            .RuleFor(r => r.Title, (f) => f.Lorem.Word()) // Título aleatório
            .RuleFor(r => r.CookingTime, (f) => f.PickRandom<CookingTime>()) // Tempo de preparo aleatório
            .RuleFor(r => r.Difficulty, (f) => f.PickRandom<Difficulty>())   // Dificuldade aleatória
            .RuleFor(r => r.ImageIdentifier, _ => $"{Guid.NewGuid()}.png")   // Identificador único de imagem (simula nome do arquivo)
            .RuleFor(r => r.Ingredients, (f) => f.Make(1, () => new Ingredient // Cria 1 ingrediente fake
            {
                Id = 1,
                Item = f.Commerce.ProductName() // Nome de produto aleatório como ingrediente
            }))
            .RuleFor(r => r.Instructions, (f) => f.Make(1, () => new Instruction // Cria 1 instrução fake
            {
                Id = 1,
                Step = 1,
                Text = f.Lorem.Paragraph() // Texto aleatório como instrução
            }))
            .RuleFor(u => u.DishTypes, (f) => f.Make(1, () => new MyRecipeBook.Domain.Entities.DishType // Cria 1 tipo de prato
            {
                Id = 1,
                Type = f.PickRandom<MyRecipeBook.Domain.Enums.DishType>() // Enum aleatório de tipo de prato
            }))
            .RuleFor(r => r.UserId, _ => user.Id); // Atribui o ID do usuário passado
    }

}
