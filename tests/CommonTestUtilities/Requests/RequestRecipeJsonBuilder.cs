using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestRecipeJsonBuilder
    {
        public static RequestRecipeJson Build()
        {
            var step = 1;

            return new Faker<RequestRecipeJson>()
                //utilizando Lorem.Word() para sinalizar ao Bogus que preencha somente com uma palavra do texto Lorem
                .RuleFor(r => r.Title, f => f.Lorem.Word())
                //PickRandom para selecionar um item de enum aleatoriamente
                .RuleFor(r => r.CookingTime, f => f.PickRandom<CookingTime>())
                .RuleFor(r => r.Difficulty, f => f.PickRandom<Difficulty>())
                //Make para informar quantos elementos trará na lista com Commerce.ProductName para retornar nomes de produtos aleatórios
                .RuleFor(r => r.Ingredients, f => f.Make(3, () => f.Commerce.ProductName()))
                .RuleFor(r => r.DishTypes, f => f.Make(3, () => f.PickRandom<DishType>()))
                //para Instructions será instanciado a request RequestInstructionJson para inputar os textos e step para os 3 elementos
                .RuleFor(r => r.Instructions, f => f.Make(3, () => new RequestInstructionJson
                {
                    Text = f.Lorem.Paragraph(),
                    Step = step++,
                }));
        }
    }
}
