using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    public abstract class VersionBase : ForwardOnlyMigration
    {
        //protected permite somente classe que tenha heranças possam implementar/utilizar o método
        protected ICreateTableColumnOptionOrWithColumnSyntax CreateTable(string table)
        {
            //criando tabela e colunas padrão que serão herdadas pelas outras classes de migration
            return Create.Table(table)
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CreatedOn").AsDateTime().NotNullable()
                .WithColumn("Active").AsBoolean().NotNullable();
        }
    }
}
