using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests;
public class RequestGenerateRecipeJsonBuilder
{
    public static RequestGenerateRecipeJson Build(int count = 5)
    {
        // Gera um objeto falso de RequestGenerateRecipeJson
        return new Faker<RequestGenerateRecipeJson>()

            // Preenche a propriedade 'Ingredients' com uma lista de 'count' nomes de produtos (ingredientes aleatórios)
            .RuleFor(user => user.Ingredients, faker => faker.Make(count, () => faker.Commerce.ProductName()));
    }
}
