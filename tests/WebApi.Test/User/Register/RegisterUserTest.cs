using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

//testes de integração/API
namespace WebApi.Test.User.Register
{
    //IClassFixture para acessar um server interno (WebApplicationFactory) do .Net para ser possível criar os unit tests para envios de requisições da API
    //Dentro de WebApplicationFactory informo o Program para sinalizar que deverá executar os tests referente a minha API (onde sua inicialização está na class default Program)
    public class RegisterUserTest : MyRecipeBookClassFixture
    {
        private readonly string method = "user";

        //base(factory) utilizado para uso da classe custom MyRecipeBookClassFixture
        public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }

        //simulando erro status 200 Success
        [Fact]
        public async Task Success()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            //informando o path da controller "User" e a request como parâmetro para PostAsJsonAsync e utilizando a função custom DoPost da classe custom MyRecipeBookClassFixture para evitar repetição de código
            var response = await DoPost(method: method, request: request);

            //informando a response que o retorno deve ser um StatusCode de Created (201)
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            //verificando se todas as props da resposta estão sendo preenchidas conforme o esperado, maneira de boas práticas ao invés de utilizar o "Desserialize"
            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace().And.Be(request.Name);
            responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        }

        //simulando erro status 400 Bad Request
        //utiliznando o Theory e posteriormente InlineData para simular mais de um caso de erro, cada return dentro da classe CultureInlineDataTest representa um cenário de erro na execução do unit test
        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string culture)
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            //utilizando a função custom DoPost da classe custom MyRecipeBookClassFixture para evitar repetição de código
            var response = await DoPost(method: method, request: request, culture: culture);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            //convertendo a cadeia de errors em array com EnumerateArray
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            //configurando para que a mensagem do erro criada no ResourceMessagesExceptions seja traduziada para o cultureInfo customizado
            var expectMessage = ResourceMessagesExceptions.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

            //simulando para receber somente uma mensagem de erro com ContainSingle
            //utilização do ponto de exclamação pós método GetString para sinalizar que o value não será null
            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectMessage));
        }
    }
}
