using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test;
using WebApi.Test.InlineData;

namespace Validators.Test.Login
{
    //teste de integração
    public class DoLoginTest : MyRecipeBookClassFixture
    {
        private readonly string METHOD = "login";
        private readonly string _email;
        private readonly string _password;
        private readonly string _name;

        public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
        {
            //acessando as funções abaixo que estão em CustomWebApplicationFactory para o retorno de email e password
            _email = factory.GetEmail();
            _password = factory.GetPassword();
            _name = factory.GetName();
        }

        [Fact]
        public async Task Success()
        {
            var request = new RequestLoginJson
            {
                Email = _email,
                Password = _password
            };

            //informando o caminho da controler "login" e a request como parâmetro para PostAsJsonAsync e utilizando a função custom DoPost da classe custom MyRecipeBookClassFixture
            //utilização de parâmetros nomeados para uso sem necessariamente ter uma ordem na chamada do método
            var response = await DoPost(method: METHOD, request: request); 

            //informando a response que o retorno deve ser um StatusCode de Created (201)
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            //verificando se todas as props da resposta estão sendo preenchidas conforme o esperado, maneira de boas práticas ao invés de utilizar o "Desserialize"
            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace().And.Be(_name);
            responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Login_Invalid(string culture)
        {
            var request = RequestLoginJsonBuilder.Build();

            //utilizando a função custom DoPost da classe custom MyRecipeBookClassFixture para evitar repetição de código
            var response = await DoPost(method: METHOD, request: request, culture: culture);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectMessage = ResourceMessagesExceptions.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectMessage));
        }
    }
}
