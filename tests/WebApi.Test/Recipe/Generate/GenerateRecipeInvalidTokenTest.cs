using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;
using System.Net;
using Xunit;

namespace WebApi.Test.Recipe.Generate;
public class GenerateRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private const string METHOD = "recipe/generate";

    public GenerateRecipeInvalidTokenTest(CustomWebApplicationFactory webApplication) : base(webApplication)
    {
    }

    // Garante que a API retorna 401 Unauthorized quando o token informado é inválido.

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();

        var response = await DoPost(method: METHOD, request: request, token: "tokenInvalid");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Verifica que a API bloqueia o acesso (status 401) quando a requisição é feita sem token.

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();

        var response = await DoPost(method: METHOD, request: request, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Testa se a API retorna 401 Unauthorized quando o token é válido, mas o usuário não existe no sistema.

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPost(method: METHOD, request: request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}