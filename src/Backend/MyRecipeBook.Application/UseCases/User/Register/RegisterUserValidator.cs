using FluentValidation;
using MyRecipeBook.Application.SharedValidators;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    //utilizando a herança da class AbstractValidator para informar a validação e entre <> sinalizando qual classe será validada
    public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
    {
        public RegisterUserValidator() {
            //validando se prop Name é vazia e customizando o retorno do erro
            RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessagesExceptions.NAME_EMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesExceptions.EMAIL_EMPTY);
            //validando regras de password com classe customizada PasswordValidator
            RuleFor(user => user.Password).SetValidator(new PasswordValidator<RequestRegisterUserJson>());
            //validando para executar a mensagem de email válido somente se o value de Email estiver preenchido, caso contrário não executa a validação de email válido
            When(user => !string.IsNullOrEmpty(user.Email), () =>
            {
                //validando se email está no formato correto
                RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessagesExceptions.EMAIL_NOT_VALID);
            });
        }
    }
}
