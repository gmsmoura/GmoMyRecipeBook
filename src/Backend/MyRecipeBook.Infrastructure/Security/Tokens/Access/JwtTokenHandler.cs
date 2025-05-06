using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access
{
    //abstract para proibir que outras classe a implementem
    //classe responsável por converter e retornar a signingKey em bytes que estão sendo utilizados em JwtTokenGenerator e JwtTokenValidator
    public abstract class JwtTokenHandler
    {
        //função para converter a _signingKey de string para um array de bytes
        protected static SymmetricSecurityKey SecurityKey(string signingKey)
        {
            var bytes = Encoding.UTF8.GetBytes(signingKey);

            return new SymmetricSecurityKey(bytes);
        }
    }
}
