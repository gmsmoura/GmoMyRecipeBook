using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase
{
    public class ErrorOnValidationException : MyRecipeBookException
    {
        //lista (utilizada do .net) para utilização da lista de Errors
        public IList<string> _errorMessages { get; set; }

        //constructor para receber a lista de erros
        //base(string.Empty) para não obrigar o construtor a passar uma mensagem específica de erro, neste caso utilizará a lista ErrorMessages
        public ErrorOnValidationException(IList<string> errorMessages) : base(string.Empty) {
            _errorMessages = errorMessages;
        }

        public override IList<string> GetErrorMessages() => _errorMessages;

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
