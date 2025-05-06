namespace MyRecipeBook.Communication.Responses
{
    public class ResponseErrorJson
    {
        //carregando uma lista de erros (IList)
        public IList<string> Errors { get; set; }

        //construtor para enviar a lista de erros (parâmetro IList<string> errors) independente de quais classes instanciarem a esta classe
        public ResponseErrorJson(IList<string> errors) => Errors = errors;

        //segundo construtor para receber uma mensagem de erro (parâmetro string error somente para uma mensagem e não a lista toda)
        public ResponseErrorJson(string error)
        {
            //instanciando a lista de errors para entregar uma mensagem de erro (error)
            Errors = new List<string> 
            { 
                error
            };
        }

        //variável para guardar value do token expirado
        public bool TokenExpired { get; set; } = false;
    }
}
