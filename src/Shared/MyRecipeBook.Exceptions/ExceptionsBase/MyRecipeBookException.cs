using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

//herança com a classe SystemException
public abstract class MyRecipeBookException : SystemException
{
    //construtor para obrigar todas as classes que herdam de MyRecipeBookException passar uma mensagem de retorno para os erros
    //base para chamar o construtor da classe SystemException
    protected MyRecipeBookException(string message) : base(message) { }
    public abstract IList<string> GetErrorMessages();
    public abstract HttpStatusCode GetStatusCode();
}
