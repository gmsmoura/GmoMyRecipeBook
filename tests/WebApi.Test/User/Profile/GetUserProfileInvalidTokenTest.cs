using CommonTestUtilities.Tokens;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net;

namespace WebApi.Test.User.Profile;

public class GetUserProfileInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user";

    public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Error_Token_Invalid(string culture)
    {
        var response = await DoGet(METHOD, token: "tokenInvalid", culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Error_Without_Token(string culture)
    {
        var response = await DoGet(METHOD, token: string.Empty, culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Error_Token_With_User_NotFound(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoGet(METHOD, token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}