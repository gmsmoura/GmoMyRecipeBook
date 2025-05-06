using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;
public class FilterRecipeValidator : AbstractValidator<RequestFilterRecipeJson>
{
    public FilterRecipeValidator()
    {
        //validando as listas e se são Enums válidos
        RuleForEach(r => r.CookingTimes).IsInEnum().WithMessage(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED);
        RuleForEach(r => r.Difficulties).IsInEnum().WithMessage(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED);
        RuleForEach(r => r.DishTypes).IsInEnum().WithMessage(ResourceMessagesExceptions.DISH_TYPE_NOT_SUPPORTED);
    }
}