using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordTest : MyRecipeBookClassFixture
{
    private const string METHOD = "user/change-password";

    private readonly string _password;
    private readonly string _email;
    private readonly Guid _userIdentifier;

    public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _password = factory.GetPassword();
        _email = factory.GetEmail();
        _userIdentifier = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = _password;

        //gerando um token válido para o user
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPut(METHOD, request, token);

        //testando se o statusCode é 200 Ok ou 204 NoContent
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        //criando a requisição de login
        var loginRequest = new RequestLoginJson
        {
            Email = _email,
            Password = _password,
        };

        //teste de unauthorized após a troca efetiva do password acima
        response = await DoPost(method: "login", request: loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        loginRequest.Password = request.NewPassword;

        //teste de success ok após a troca efetiva do password acima
        response = await DoPost(method: "login", request: loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    //teste de integração para new password vazio
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_NewPassword_Empty(string culture)
    {
        var request = new RequestChangePasswordJson
        {
            Password = _password,
            NewPassword = string.Empty
        };

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPut(METHOD, request, token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("PASSWORD_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));
    }
}