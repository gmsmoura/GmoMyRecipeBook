using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate;
public class GenerateRecipeValidator : AbstractValidator<RequestGenerateRecipeJson>
{
    public GenerateRecipeValidator()
    {
        //variável para definir o número máximo de ingredientes
        var maximum_number_ingredients = MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE;

        //regra para validar o número máximo de ingrediente e exibir mensagem caso ultrapasse e seja inválido
        RuleFor(request => request.Ingredients.Count).InclusiveBetween(1, maximum_number_ingredients).WithMessage(ResourceMessagesExceptions.INVALID_NUMBER_INGREDIENTS);

        //regra para validar ingredientes duplicados, utilização do Distinct() + Count() para verificar, não há necessidade de uso do Select(), é possível utilizar o Distinct() diretamente
        RuleFor(request => request.Ingredients).Must(ingredients => ingredients.Count == ingredients.Distinct().Count()).WithMessage(ResourceMessagesExceptions.DUPLICATED_INGREDIENTS_IN_LIST);

        //regra para validar ingredientes em branco e ingredientes que não seguem o padrão
        RuleFor(request => request.Ingredients).ForEach(rule =>
        {
            rule.Custom((value, context) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    context.AddFailure("Ingredient", ResourceMessagesExceptions.INGREDIENT_EMPTY);
                }
                //para evitar que o usuário preencha com os valores separados por espaços ou barras
                else if (value.Count(c => c == ' ') > 3 || value.Count(c => c == '/') > 1)
                {
                    context.AddFailure("Ingredient", ResourceMessagesExceptions.INGREDIENT_NOT_FOLLOWING_PATTERN);
                }
            });
        });
    }
}
