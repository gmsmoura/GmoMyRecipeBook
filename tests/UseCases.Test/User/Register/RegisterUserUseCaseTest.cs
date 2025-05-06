using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.Register
{
    public class RegisterUserUseCaseTest
    {
        //unit test para validar o envio de sucesso para a criação do usuário
        [Fact]
        public async Task Success()
        {
            //instanciando a classe que trás as instâncias de RequestRegisterUserJson para ser utilizado nos testes sem ter que chamar manualmente para cada teste
            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase();

            //simula o comportamento da requisição
            var result = await useCase.Execute(request);

            //valida se o result é nulo, se não for nulo, prossegue para a próxima verificação
            result.Should().NotBeNull();

            //validando se o Token e AccessToken não é nulo ou vazio
            result.Tokens.Should().NotBeNull();
            result.Tokens.AccessToken.Should().NotBeNullOrEmpty();

            //validando prop Name (request.Name) se é igual a prop Name (user.Name) dentro de RegisterUserUseCase
            result.Name.Should().Be(request.Name);
        }

        //unit test para validar um erro de email que já existe
        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            var useCase = CreateUseCase(request.Email);

            //guardando uma função dentro de uma variável e utilizando o Execute() para chamar de fato e testar a requisição do useCase
            Func<Task> act = async () => await useCase.Execute(request);

            //linha para lançar a exception de error de RegisterUserUseCase do método Validate()
            //validando para além de lançar a excpetion ErrorOnValidationException também lançar a mensagem de erro dentro de ResourceMessagesExceptions utilizando o método Contains()
            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                 .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));
        }

        //unit test para validar se o nome é vazio
        [Fact]
        public async Task Error_Name_Empty()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            //forçando o value do e-mail a ser vazio
            request.Name = string.Empty;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.Execute(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                 .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.NAME_EMPTY));
        }

        //string email = null para tornar o parâmetro opcional(nullable) e não impactar nas chamadas do método quando não for necessário informar parâmetro
        private static RegisterUserUseCase CreateUseCase(string? email = null, string? name = null)
        {
            //declaração das variáveis de Build para utilização no useCase
            var mapper = MapperBuilder.Build();
            var passwordEncripter = PasswordEncripterBuilder.Build();
            var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
            //o Build do readRepository foi necessário instanciar e configurar diferentes dos outros pelo fato de ser uma consulta e ter retorno,
            //para os casos static que não possuí retorno a implementação segue da forma dos três itens acima
            var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
            var tokenRepository = new TokenRepositoryBuilder().Build(); // instanciando o TokenRepositoryBuilder

            // instanciando a classe com as dependências de autenticação
            var authDependencies = new AuthDependencies(accessTokenGenerator, refreshTokenGenerator, tokenRepository);

            //se o e-mail existir (condição abaixo for verdadeira chamará o método ExistActiveUserWithEmail() para validar o email
            if (string.IsNullOrEmpty(email) == false)
                readRepositoryBuilder.ExistActiveUserWithEmail(email);     

            //os parâmetros precisam estar na ordem correta de como estão no constructor de injeção de depêndencia da classe RegisterUserUseCase
            return new RegisterUserUseCase(writeRepository, readRepositoryBuilder.Build(), unitOfWork, passwordEncripter, mapper, authDependencies);
        }
    }
}
