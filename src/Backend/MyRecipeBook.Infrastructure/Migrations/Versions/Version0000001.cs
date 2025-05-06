using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    //informando qual número e descrição da versão
    [Migration(DatabaseVersions.TABLE_USER, "Create table to save the user's information")]
    public class Version0000001 : VersionBase
    {
        //caso haja necessidade de reverse, é possível utilizar o método Down(), disponível somente com a classe Migration executa um reverse do que foi executado na Up()

        //método Up() para escrever o código que será executado para o banco de dados, classe ForwardOnlyMigration permite utilizar somente o método Up()
        public override void Up()
        {
            //criando tabela implementando o método herdado da classe VersionBase
            CreateTable("Users")
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Email").AsString(255).NotNullable()
                .WithColumn("Password").AsString(2000).NotNullable()
                .WithColumn("UserIdentifier").AsGuid().NotNullable();
        }
    }
}