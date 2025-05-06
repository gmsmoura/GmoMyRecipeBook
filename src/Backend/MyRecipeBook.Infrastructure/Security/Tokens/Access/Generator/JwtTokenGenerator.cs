using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator
{
    public class JwtTokenGenerator : JwtTokenHandler, IAccessTokenGenerator
    {
        //variáveis abaixo que ficará em appSettings para tempo de expiração do token e chave de assinatura para validar o token recebido
        private readonly uint _expirationTimeMinutes;
        private readonly string _signingKey;

        public JwtTokenGenerator(uint expirationTimeMinutes, string signingKey)
        {
            _expirationTimeMinutes = expirationTimeMinutes;
            _signingKey = signingKey;
        }

        //utilizando o Guid para gerar um identificador único para o user e para que o user não consiga alterar o valor
        public string Generate(Guid userIdentifier)
        {
            //passando o Guid para o ClaimsIdentity com uma lista de Claims
            var claims = new List<Claim>()
            {
                //utilizando types pré definido com ClaimTypes.Sid e o valor como userIdentifier em string
                new Claim(ClaimTypes.Sid, userIdentifier.ToString())
            };

            //configuração do tokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            { 
                Subject = new ClaimsIdentity(claims), //inserindo o Guid dentro do token               
                Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes), //capturando a data atual e adicionando os minutos parametrizados
                SigningCredentials = new SigningCredentials(SecurityKey(_signingKey), SecurityAlgorithms.HmacSha256Signature) //configurando chave de assinatura e algoritmo de assinatura que será utilizado o mais comum e usado HmacSha256Signature
            };

            //geração de token
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            //retornando o token e gerando como string
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
