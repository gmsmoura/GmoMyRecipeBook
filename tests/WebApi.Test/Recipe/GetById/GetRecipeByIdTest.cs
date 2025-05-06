using CommonTestUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.Recipe.GetById;
public class GetRecipeByIdTest : MyRecipeBookClassFixture
{
    private const string METHOD = "recipe";

    private readonly Guid _userIdentifier;
    private readonly string _recipeId;
    private readonly string _recipeTitle;

    //gets para capturar o userIdentifier, recipeId e recipeTitle para serem usados nos testes
    public GetRecipeByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeId();
        _recipeTitle = factory.GetRecipeTitle();
    }

    //teste de integração para validar a busca de uma receita pelo id sendo success 200
    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoGet($"{METHOD}/{_recipeId}", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetString().Should().Be(_recipeId);
        responseData.RootElement.GetProperty("title").GetString().Should().Be(_recipeTitle);
    }

    //teste de integração para validar a busca de uma receita pelo id not found 404
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Recipe_Not_Found(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var id = IdEncripterBuilder.Build().Encode(1000);

        var response = await DoGet($"{METHOD}/{id}", token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));
    }
}
