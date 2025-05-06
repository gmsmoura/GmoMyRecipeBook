using MyRecipeBook.Domain.Security.Cryptography;

namespace MyRecipeBook.Infrastructure.Security.Cryptography
{
    public class BCryptNet : IPasswordEncripter
    {
        public string Encrypt(string password)
        {
            
            
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool IsValid(string password, string hashPassword)
        {
            
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
    }
}
