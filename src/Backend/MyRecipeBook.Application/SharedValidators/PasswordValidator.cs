using FluentValidation;
using FluentValidation.Validators;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.SharedValidators;

//classe responsável para validações específicas de password para ser utilizado em várias classes, pois será utilizado muitas vezes para validações
//para o funcionamento, é necessário herdar a classe PropertyValidator
public class PasswordValidator<T> : PropertyValidator<T, string>
{
    //override (sobrescrevendo) a função IsValid() de PropertyValidator para customizar conforme necessário
    public override bool IsValid(ValidationContext<T> context, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            //utilizando a função AppendArgument() para uso do MessageFormatter e retornar o value da mensagem de erro
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesExceptions.PASSWORD_EMPTY);

            return false;
        }

        if (password.Length < 8)
        {
            //utilizando a função AppendArgument() para uso do MessageFormatter e retornar o value da mensagem de erro
            context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessagesExceptions.INVALID_PASSWORD);

            return false;
        }

        return true;
    }

    //para o funcionamento é necessário sobrescrever a propriedade Name e passar o nome da classe, neste caso PasswordValidator
    public override string Name => "PasswordValidator";

    //por fim sobrescreve a função GetDefaultMessageTemplate() informando qual a chave utilizada na função IsValid(), neste caso guardando a chave ErrorMessage como string
    protected override string GetDefaultMessageTemplate(string errorCode) => "{ErrorMessage}";
}
