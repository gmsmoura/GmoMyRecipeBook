using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase
{
    public class ErrorOnValidationException : MyRecipeBookException
    {
        
        public IList<string> _errorMessages { get; set; }

        
        
        public ErrorOnValidationException(IList<string> errorMessages) : base(string.Empty) {
            _errorMessages = errorMessages;
        }

        public override IList<string> GetErrorMessages() => _errorMessages;

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
