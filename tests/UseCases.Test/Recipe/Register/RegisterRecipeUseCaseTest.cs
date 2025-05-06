using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Exceptions;
using CommonTestUtilities.Requests;
using UseCases.Test.Recipe.InlineDatas;
using Microsoft.AspNetCore.Http;
using CommonTestUtilities.BlobStorage;

namespace UseCases.Test.Recipe.Register
{
    //testes de unidade
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task Success_Without_Image()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestRegisterRecipeFormDataBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(request);

            //validando se o resultado não é null
            result.Should().NotBeNull();

            //validando se o Id não é null e não contém espaços
            result.Id.Should().NotBeNullOrWhiteSpace();

            //validando se o resultado está com o título preenchido
            result.Title.Should().Be(request.Title);
        }

        // Teste que valida se a funcionalidade de cadastro de receita funciona corretamente com uma imagem válida
        [Theory]
        [ClassData(typeof(ImageTypesInlineData))] // Usa dados da classe que fornece imagens válidas
        public async Task Success_With_Image(IFormFile file)
        {
            // Cria um usuário fictício para o teste
            (var user, _) = UserBuilder.Build();

            // Cria um request de receita incluindo a imagem recebida como parâmetro
            var request = RequestRegisterRecipeFormDataBuilder.Build(file);

            // Instancia o caso de uso com as dependências
            var useCase = CreateUseCase(user);

            // Executa o caso de uso com o request montado
            var result = await useCase.Execute(request);

            // Valida se o retorno não é nulo e os dados estão coerentes com o esperado
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrWhiteSpace();
            result.Title.Should().Be(request.Title);
        }

        // Teste que verifica se uma exceção é lançada quando o título da receita está vazio
        [Fact]
        public async Task Error_Title_Empty()
        {
            (var user, _) = UserBuilder.Build(); // Cria usuário de teste

            var request = RequestRegisterRecipeFormDataBuilder.Build(); // Cria uma requisição padrão
            request.Title = string.Empty; // Força o título a ser vazio para disparar a validação

            var useCase = CreateUseCase(user); // Instancia o caso de uso

            // Define uma função que executa o caso de uso, usada para capturar exceções
            Func<Task> act = async () => { await useCase.Execute(request); };

            // Espera que a exceção personalizada seja lançada com a mensagem de erro correta
            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY));
        }

        // Teste que verifica se arquivos inválidos (não imagens) geram erro
        [Fact]
        public async Task Error_Invalid_File()
        {
            (var user, _) = UserBuilder.Build(); // Cria usuário fictício

            var textFile = FormFileBuilder.Txt(); // Gera um arquivo .txt para simular upload inválido

            var request = RequestRegisterRecipeFormDataBuilder.Build(textFile); // Monta request com o arquivo inválido
            var useCase = CreateUseCase(user); // Instancia o caso de uso

            var act = async () => { await useCase.Execute(request); }; // Executa o teste esperando exceção

            // Valida se a exceção correta foi lançada e com a mensagem esperada
            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesExceptions.ONLY_IMAGES_ACCEPTED));
        }

        // Método auxiliar que monta o caso de uso com todas as dependências simuladas
        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var mapper = MapperBuilder.Build(); // Simula o AutoMapper
            var unitOfWork = UnitOfWorkBuilder.Build(); // Simula o UnitOfWork
            var loggedUser = LoggedUserBuilder.Build(user); // Simula o usuário logado
            var repository = RecipeWriteOnlyRepositoryBuilder.Build(); // Simula o repositório de escrita
            var blobStorage = new BlobStorageServiceBuilder().Build(); // Simula o serviço de armazenamento de imagem

            return new RegisterRecipeUseCase(loggedUser, repository, unitOfWork, mapper, blobStorage);
        }
    }
}
