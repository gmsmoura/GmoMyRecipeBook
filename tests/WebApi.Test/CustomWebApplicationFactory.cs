using Azure.Messaging.ServiceBus;
using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.IdEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.Services.ServiceBus;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Test
{
    //servidor que ficará disponível dentro do EntityFramework para criação de banco de dados em memória exclusivo para os unit tests
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        //utilizando default! nas entidades para garantir que o objeto não será null
        
        private MyRecipeBook.Domain.Entities.Recipe _recipe = default!;
        private MyRecipeBook.Domain.Entities.User _user = default!;
        private string _password = string.Empty;

        //override para sobreescreve um método
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //criando ambiente em appsettings para centralizar os unit tests
            builder.UseEnvironment("Test")
                 .ConfigureServices(services =>
                 {
                     //recuperando um serviço na injeção de dependência com SingleOrDefault
                     //e no serviço de injeção de dependência verificando se no serviço existe o DbContext "MyRecipeBookDbContext"
                     //se existir, o descriptor será removido para não haver riscos de se conectar ao servidor real
                     var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MyRecipeBookDbContext>));
                     if (descriptor is not null)
                         services.Remove(descriptor);
                         
                     //configurando banco de dados em memória
                     var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                     //configurando o BlobStorageService para criar o mock que será adicionado no serviço de injeção de dependência
                     //e quando o useCase precisar de uma instância de IStorageService, ele irá devolver o mock
                     var blobStorage = new BlobStorageServiceBuilder().Build();
                     services.AddScoped(option => blobStorage);

                     //adicionando o novo service ao DbContext para uso em memória
                     services.AddDbContext<MyRecipeBookDbContext>(options =>
                     {
                         options.UseInMemoryDatabase("InMemoryDbForTesting");
                         options.UseInternalServiceProvider(provider);
                     });

                     //chamando CreateScope para devolver o IServiceScope, adicionando o using para ser usado em memória
                     using var scope = services.BuildServiceProvider().CreateScope();

                     //chamando GetRequiredService de ServiceProvider para crair o dbContext de MyRecipeBookDbContext
                     var dbContext = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();

                     //garantindo que a base de dados inicialize vazia
                     dbContext.Database.EnsureDeleted();

                     StartDatabase(dbContext);
                 });
        }

        //funções para capturar o email e password
        public string GetEmail() => _user.Email;
        public string GetPassword() => _password;
        public string GetName() => _user.Name;
        public Guid GetUserIdentifier() => _user.UserIdentifier;

        //função para capturar o os values das propriedades da entidade Recipe
        public string GetRecipeId() => IdEncripterBuilder.Build().Encode(_recipe.Id);
        public string GetRecipeTitle() => _recipe.Title;
        public Difficulty GetRecipeDifficulty() => _recipe.Difficulty!.Value;
        public CookingTime GetRecipeCookingTime() => _recipe.CookingTime!.Value;
        public IList<DishType> GetDishTypes() => _recipe.DishTypes.Select(c => c.Type).ToList();

        //função privada para separar as responsabilidade e deixar o código mais organizado e irá preencher as variáveis privadas onde somente esta classe terá acesso
        private void StartDatabase(MyRecipeBookDbContext dbContext)
        {
            (_user, _password) = UserBuilder.Build();

            _recipe = RecipeBuilder.Build(_user);

            dbContext.Users.Add(_user);

            dbContext.Recipes.Add(_recipe);

            dbContext.SaveChanges();
        }
    }
}