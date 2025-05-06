using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.User.Update
{
    public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
    {
        public UpdateUserValidator()
        {
            //validando entradas vazias
            RuleFor(request => request.Name).NotEmpty().WithMessage(ResourceMessagesExceptions.NAME_EMPTY);
            RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesExceptions.EMAIL_EMPTY);

            //validando email inválido quando o value não for vazio ou em branco
            When(request => string.IsNullOrWhiteSpace(request.Email).IsFalse(), () =>
            {
                RuleFor(request => request.Email).EmailAddress().WithMessage(ResourceMessagesExceptions.EMAIL_NOT_VALID);
            });
        }
    }
}
