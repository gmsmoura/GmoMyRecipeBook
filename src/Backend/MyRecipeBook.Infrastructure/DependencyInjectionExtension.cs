﻿using Azure.Messaging.ServiceBus;
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
    
    
    
    
    public static class DependencyInjectionExtension
    {
        
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            AddPasswordEncrypter(services);
            AddRepositories(services);
            AddLoggedUser(services);
            AddTokens(services, configuration);
            AddOpenAI(services, configuration);
            AddAzureStorage(services, configuration);
            AddQueue(services, configuration);

            
            if (configuration.IsUnitTestEnviroment())
                return;

            
            var databaseType = configuration.DatabaseType();

            
            if (databaseType == DatabaseType.Sql)
            {
                AddDbContext_SqlServer(services, configuration);
                AddFluentMigrator_Sql(services, configuration);
            }
        }

        
        
        private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration configuration)
        {
            
            var connectionString = configuration.ConnectionString();

            
            services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(connectionString);
            });
        }

        
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

        
        private static void AddPasswordEncrypter(IServiceCollection services)
        {
            
            services.AddScoped<IPasswordEncripter, BCryptNet>();
        }

        private static void AddOpenAI(IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddScoped<IGenerateRecipeAI, ChatGptService>();

            
            var apiKey = configuration.GetValue<string>("Settings:OpenAI:ApiKey");

            
            services.AddScoped(c => new ChatClient(MyRecipeBookRuleConstants.CHAT_MODEL, apiKey));
        }

        private static void AddAzureStorage(IServiceCollection services, IConfiguration configuration)
        {
            
            var connectionString = configuration.GetValue<string>("Settings:BlobStorage:Azure");

            
            if (connectionString.NotEmpty())
            {
                
                services.AddScoped<IBlobStorageService>(c =>
                    new AzureStorageService(new BlobServiceClient(connectionString)));
            }
        }

        private static void AddQueue(IServiceCollection services, IConfiguration configuration)
        {
            
            var connectionString = configuration.GetValue<string>("Settings:ServiceBus:DeleteUserAccount")!;

            
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            
            var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            
            var deleteQueue = new DeleteUserQueue(client.CreateSender("user"));

            
            
            var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor("user", new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1
            }));

            
            services.AddSingleton(deleteUserProcessor);

            
            services.AddScoped<IDeleteUserQueue>(options => deleteQueue);

            
            
            
            
        }
    }
}
