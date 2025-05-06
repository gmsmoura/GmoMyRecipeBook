using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {
        
        
        public static bool IsUnitTestEnviroment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }
        
        
        public static DatabaseType DatabaseType(this IConfiguration configuration)
        {
            var databaseType = configuration.GetConnectionString("DatabaseType");

            
            return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType!);
        }
        
        public static string ConnectionString(this IConfiguration configuration)
        {
            
            var databaseType = configuration.DatabaseType();

            
            
            if (databaseType is Domain.Enums.DatabaseType.Sql)
                return configuration.GetConnectionString("ConnectionSQLServer")!;
            else
                return configuration.GetConnectionString("Caso houver ConnectionMySQLServer")!;

        }
    }
}
