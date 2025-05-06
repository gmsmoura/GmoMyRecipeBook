using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {
        //classe responsável para criação de extensão das configs de connectionString para utilização nas classes de configurações e chamadas envolvendo migrations por exemplo
        
        public static bool IsUnitTestEnviroment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }
        
        //DatabaseType utilizado no lugar de "void" para informar o type que será responsável pelo método
        public static DatabaseType DatabaseType(this IConfiguration configuration)
        {
            var databaseType = configuration.GetConnectionString("DatabaseType");

            //chamando Enum.Parse para converter a string "DatabaseType" e identificar o enum do type do banco de dados (0 ou 1) que será convertido no valor da string, no caso de 0 seria Sql
            return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType!);
        }
        
        public static string ConnectionString(this IConfiguration configuration)
        {
            //para chamar o DatabaseType que é uma extensão de IConfiguration
            var databaseType = configuration.DatabaseType();

            //verificação para validar se o databseType == SQL
            //exclamação no final do return para informar que o retorno não será null
            if (databaseType is Domain.Enums.DatabaseType.Sql)
                return configuration.GetConnectionString("ConnectionSQLServer")!;
            else
                return configuration.GetConnectionString("Caso houver ConnectionMySQLServer")!;

        }
    }
}
