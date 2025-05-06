namespace MyRecipeBook.Communication.Requests
{
    public class RequestRegisterUserJson
    {
        //string.Empty para tratar se não for enviado valores a essas propriedades, invés de nulo será vazio
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

//ctrl + r + g limpa as referencias da classe que não estão sendo utilizadas