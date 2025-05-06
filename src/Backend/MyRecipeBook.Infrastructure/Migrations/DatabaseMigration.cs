using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;

namespace MyRecipeBook.Infrastructure.Migrations
{
    public static class DatabaseMigration
    {
        public static void Migrate(DatabaseType databaseType, string connectionString, IServiceProvider serviceProvider)
        {
            if(databaseType is DatabaseType.Sql) EnsureDatabaseCreated_Sql(connectionString);

            MigrationDatabase(serviceProvider);
        }

        //garantir que o databse seja criada com o type SQL 
        private static void EnsureDatabaseCreated_Sql(string connectionString)
        {
            //recuperando schema da connection string
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;

            //para evitar erros de consultas aos databases, o que neste cenários não será necessário
            connectionStringBuilder.Remove("Database");

            //criando conexão com o database, passando como param o "connectionStringBuilder.ConnectionString" para utilizar a connectionString já tratada (na linha 23)
            using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

            //criano paramêtros, com chave e valor, "name", databaseName
            var parameters = new DynamicParameters();
            parameters.Add("name", databaseName);

            //query para verificar se já existe schema com o nome solicitado
            var records = dbConnection.Query("SELECT * FROM sys.databases WHERE name = @name", parameters);
            
            //records.Any() para validar a existência de resultados dentro de records, ou seja, se conter qualquer retorno (Any)
            if(records.Any().IsFalse())
            {
                //se não existir nenhum schema com o nome informado nos parâmetros, será criado o database, o $ para utilização do "string interpolation"
                dbConnection.Execute($"CREATE DATABASE {databaseName}");
            }
        }

        //método para realizar os migrations das versões, serviceProvider é o serviço da injeção de dependência
        private static void MigrationDatabase(IServiceProvider serviceProvider)
        {
            //chamando método GetRequiredService() para informar qual serviço será recuperado do AddFluentMigratorCore() da classe DependencyInjectionExtension.cs
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            
            //listando as versões das migrations
            runner.ListMigrations();

            //para executar um controle no database e informar o que precisa ou não ser migrado
            runner.MigrateUp();
        }
    }
}
