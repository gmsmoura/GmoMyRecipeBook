using MyRecipeBook.Domain.Security.Cryptography;

namespace MyRecipeBook.Infrastructure.Security.Cryptography
{
    public class BCryptNet : IPasswordEncripter
    {
        public string Encrypt(string password)
        {
            // para gerar criptografia com bcrypt
            // e irá retornar como um hash (string) de 60 caracteres da senha informada
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool IsValid(string password, string hashPassword)
        {
            // para verificar se a senha informada é válida são equivalente ao hash
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
    }
}
