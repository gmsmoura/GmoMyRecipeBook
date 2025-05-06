using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe
{
    public class RecipeValidator : AbstractValidator<RequestRecipeJson>
    {
        public RecipeValidator() 
        {
            RuleFor(recipe => recipe.Title).NotEmpty().WithMessage(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY);

            //utilização de IsEnum() para sinalizar que a propriedade se trata de um enum e assim validá-lo, ou seja, só será permitido entrada de valores constantes no enum
            RuleFor(recipe => recipe.CookingTime).IsInEnum().WithMessage(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED);
            RuleFor(recipe => recipe.Difficulty).IsInEnum().WithMessage(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED);

            //utilização de GreaterThan() para validar a quantidade mínima de item a ser aceito na lista (ingredients, instructions)
            RuleFor(recipe => recipe.Ingredients.Count).GreaterThan(0).WithMessage(ResourceMessagesExceptions.AT_LEAST_ONE_INGREDIENT);
            RuleFor(recipe => recipe.Instructions.Count).GreaterThan(0).WithMessage(ResourceMessagesExceptions.AT_LEAST_ONE_INSTRUCTION);

            //utitilizando RuleForEach() para validar todos os elementos da lista 
            RuleForEach(recipe => recipe.DishTypes).IsInEnum().WithMessage(ResourceMessagesExceptions.DISH_TYPE_NOT_SUPPORTED);
            RuleForEach(recipe => recipe.Ingredients).NotEmpty().WithMessage(ResourceMessagesExceptions.INGREDIENT_EMPTY);

            //utilizalção de ChildRules() para validar as regras de instructions com base nas propriedades do objeto Instructions
            RuleForEach(recipe => recipe.Instructions).ChildRules(instructionRule =>
            {
                instructionRule.RuleFor(instruction => instruction.Step).GreaterThan(0).WithMessage(ResourceMessagesExceptions.NON_NEGATIVE_INSTRUCTION_STEP);
                instructionRule
                .RuleFor(instruction => instruction.Text)
                .NotEmpty()
                .WithMessage(ResourceMessagesExceptions.INSTRUCTION_EMPTY)
                .MaximumLength(2000)
                .WithMessage(ResourceMessagesExceptions.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);
            });

            //regra para garantir que não tenha steps duplicados, utilizando as funções Must(), Select() para selecionar somente os números dos passos, Distinct()
            //...Count() == instructions.Count para validar se a lista original se difere da lista fornecida (instructions.Count), se for diferente retornará a exception e mensagem de erro
            RuleFor(recipe => recipe.Instructions)
                .Must(instructions => instructions.Select(i => i.Step).Distinct().Count() == instructions.Count)
                .WithMessage(ResourceMessagesExceptions.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER);
        }
    }
}
