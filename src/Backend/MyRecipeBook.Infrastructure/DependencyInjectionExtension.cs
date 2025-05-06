using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Domain.Services.ServiceBus;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Validator;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;
using MyRecipeBook.Infrastructure.Services.LoggedUser;
using MyRecipeBook.Infrastructure.Services.OpenAI;
using MyRecipeBook.Infrastructure.Services.ServiceBus;
using MyRecipeBook.Infrastructure.Services.Storage;
using OpenAI.Chat;
using System.Reflection;

namespace MyRecipeBook.Infrastructure
{
    //classes para injeção de dependências precisam ser statics
    //utilizado static para criar bibliotecas de funções ou utilitários, como funções matemáticas
    //classes static são úteis quando desejamos organizar código que não depende de instâncias de objetos e precisa ser acessado de forma global.
    //todos os métodos da classe static precisam ser do mesmo modificador de acesso static
    public static class DependencyInjectionExtension
    {
        //void pois o método não irá retornar nada e o this no parâmetro pelo método ser público, utilizando dois parâmetros, o IConfiguration para ser chamado no Program.cs para o builder da connectionString ser utilizado
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            AddPasswordEncrypter(services);
            AddRepositories(services);
            AddLoggedUser(services);
            AddTokens(services, configuration);
            AddOpenAI(services, configuration);
            AddAzureStorage(services, configuration);
            AddQueue(services, configuration);

            //se for um unit test os blocos de adição de dbcontext abaixo não serão executados
            if (configuration.IsUnitTestEnviroment())
                return;

            //databaseType sendo referenciado da classe de extensão ConfigurationExtension
            var databaseType = configuration.DatabaseType();

            //chamando os métodos de injeção de depedências
            if (databaseType == DatabaseType.Sql)
            {
                AddDbContext_SqlServer(services, configuration);
                AddFluentMigrator_Sql(services, configuration);
            }
        }

        //método para configurar o ambiente de acesso a dados para a aplicação, ap registrar o MyRecipeBookDbContext como um serviço, a API pode solicitar instâncias desse contexto em outros lugares do código via injeção de dependência
        //isso é fundamental para realizar operações com o banco de dados, como consultas, inserções, atualizações e exclusões de dados, de maneira integrada e eficiente
        private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration configuration)
        {
            //passando connectionString para informar como será preenchido para o database SqlServer, connectionString sendo referenciado da classe de extensão ConfigurationExtension
            var connectionString = configuration.ConnectionString();

            //como origem dentro de <> está sendo chamado a classe MyRecipeBookDbContext
            services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(connectionString);
            });
        }

        //método privado para configurar a injeção de depências para o UserRepository e adicionando o escopo deles no serviço
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserDeleteOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeUpdateOnlyRepository, RecipeRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
        }

        //método para adicionar a injeção de dependência para utilização do fluent migrator
        private static void AddFluentMigrator_Sql(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
        }

        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>(option => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
            services.AddScoped<IAccessTokenValidator>(option => new JwtTokenValidator(signingKey!));

            services.AddScoped<IRefreshTokenGenerator>(option => new RefreshTokenGenerator());
        }

        private static void AddLoggedUser(IServiceCollection services) => services.AddScoped<ILoggedUser, LoggedUser>();

        // método para adicionar a injeção de dependência para o PasswordEncripter
        private static void AddPasswordEncrypter(IServiceCollection services)
        {
            // registra a implementação do serviço de criptografia de senhas (BCryptNet)
            services.AddScoped<IPasswordEncripter, BCryptNet>();
        }

        private static void AddOpenAI(IServiceCollection services, IConfiguration configuration)
        {
            // Registra a implementação do serviço de geração de receitas usando IA (ChatGptService)
            services.AddScoped<IGenerateRecipeAI, ChatGptService>();

            // Recupera a chave de API da OpenAI do arquivo de configuração (appsettings.json)
            var apiKey = configuration.GetValue<string>("Settings:OpenAI:ApiKey");

            // Registra uma instância de ChatClient com o modelo e a chave da API como dependência injetável
            services.AddScoped(c => new ChatClient(MyRecipeBookRuleConstants.CHAT_MODEL, apiKey));
        }

        private static void AddAzureStorage(IServiceCollection services, IConfiguration configuration)
        {
            // Recupera a connection string do Azure Blob Storage do arquivo de configuração
            var connectionString = configuration.GetValue<string>("Settings:BlobStorage:Azure");

            // Verifica se a connection string não está vazia para não ter riscos de conflitar com o mock dentro de CustomWebApplicationFactory do unit test para instanciar o BlobStorageService
            if (connectionString.NotEmpty())
            {
                // Registra a implementação do serviço de armazenamento de blobs (AzureStorageService)
                services.AddScoped<IBlobStorageService>(c =>
                    new AzureStorageService(new BlobServiceClient(connectionString)));
            }
        }

        private static void AddQueue(IServiceCollection services, IConfiguration configuration)
        {
            // Obtém a connection string da fila do Service Bus a partir do appsettings.json
            var connectionString = configuration.GetValue<string>("Settings:ServiceBus:DeleteUserAccount")!;

            // Se a connection string estiver vazia ou nula, a configuração da fila é ignorada
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            // Cria uma instância do ServiceBusClient utilizando AMQP via WebSockets (útil em ambientes com firewall restritivo)
            var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            // Cria um "sender" para a fila chamada "user", usado para enviar mensagens
            var deleteQueue = new DeleteUserQueue(client.CreateSender("user"));

            // Cria um "processor" para consumir mensagens da fila "user"
            // MaxConcurrentCalls = 1 significa que o processamento será sequencial (uma mensagem por vez)
            var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor("user", new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1
            }));

            // Registra o processor como singleton no container de DI
            services.AddSingleton(deleteUserProcessor);

            // Registra o sender (DeleteUserQueue) como serviço Scoped que implementa a interface IDeleteUserQueue
            services.AddScoped<IDeleteUserQueue>(options => deleteQueue);

            /// <summary>
            /// AddScoped: O serviço será criado uma vez por solicitação e descartado no final da solicitação.
            /// AddSingleton?: O serviço será criado uma vez e compartilhado entre todas as solicitações.
            /// <\summary>
        }
    }
}
