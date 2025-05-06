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

        
        private static void EnsureDatabaseCreated_Sql(string connectionString)
        {
            
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;

            
            connectionStringBuilder.Remove("Database");

            
            using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

            
            var parameters = new DynamicParameters();
            parameters.Add("name", databaseName);

            
            var records = dbConnection.Query("SELECT * FROM sys.databases WHERE name = @name", parameters);
            
            
            if(records.Any().IsFalse())
            {
                
                dbConnection.Execute($"CREATE DATABASE {databaseName}");
            }
        }

        
        private static void MigrationDatabase(IServiceProvider serviceProvider)
        {
            
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            
            
            runner.ListMigrations();

            
            runner.MigrateUp();
        }
    }
}
