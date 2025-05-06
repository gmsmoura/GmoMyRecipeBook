using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access.Validator
{
    public class JwtTokenValidator : JwtTokenHandler, IAccessTokenValidator
    {
        private readonly string _signingKey;

        public JwtTokenValidator(string signingKey) => _signingKey = signingKey;

        //verifica a validade do token JWT e extrai o Guid do usuário a partir da claim Sid. Se o token for inválido ou expirado, uma exceção será lançada durante a validação
        public Guid ValidateAndGetUserIdentifier(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                //ValidateAudience e ValidateIssuer: Desativados(não verifica o público - alvo nem o emissor do token).
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = SecurityKey(_signingKey), //define a chave de assinatura usada para verificar a integridade do token (signingKey)
                ClockSkew = new TimeSpan(0) //define uma tolerância zero para diferenças de horário (evita validações com atraso).
            };

            //instanciando a classe JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //verifica se o token é válido (assinatura, data de expiração, etc.) e principal representa a identidade do user após a validação, incluindo as claims associadas ao token
            //utilização de out _ para ignorar o terceiro parâmetro
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            //busca a claim do tipo Sid (Session ID) que contém o Guid do usuário
            var userIdentifier = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

            //converte o valor extraído em um Guid
            return Guid.Parse(userIdentifier);
        }
    }
}
