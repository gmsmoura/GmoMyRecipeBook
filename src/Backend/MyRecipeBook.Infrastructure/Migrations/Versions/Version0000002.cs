using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    //informando qual número e descrição da versão
    [Migration(DatabaseVersions.TABLE_RECIPES, "Create table to save the recipe's information")]
    public class Version0000002 : VersionBase
    {
        private const string RECIPE_TABLE_NAME = "Recipes";

        //caso haja necessidade de reverse, é possível utilizar o método Down(), disponível somente com a classe Migration executa um reverse do que foi executado na Up()

        //método Up() para escrever o código que será executado para o banco de dados, classe ForwardOnlyMigration permite utilizar somente o método Up()
        public override void Up()
        {
            //criando tabela implementando o método herdado da classe VersionBase
            CreateTable("Recipes")
                .WithColumn("Title").AsString(255).NotNullable()
                //utilização de Int32 para o uso de enums
                .WithColumn("CookingTime").AsInt32().Nullable()
                .WithColumn("Difficulty").AsInt32().Nullable()
                //Int64 mesmo que long e criando relacionamento das colunas UserId da tabela Recipe com Id da tabela de Users
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_Recipe_User_Id", "Users", "Id");

            CreateTable("Ingredients")
                .WithColumn("Item").AsString(255).NotNullable()
                //criando relacionamento das colunas RecipeId da tabela Recipe com Id da tabela de Recipes
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Ingredient_Recipe_Id", RECIPE_TABLE_NAME, "Id")
                //criando regra para quando a receita for deletada, também deletar os ingredientes associados, sugerido utilizar o método OnDelete na tabela auxiliar e não na tabela principal
                .OnDelete(System.Data.Rule.Cascade);

            CreateTable("Instructions")
                .WithColumn("Step").AsInt32().NotNullable()
                .WithColumn("Text").AsString(2000).NotNullable()
                //criando relacionamento das colunas RecipeId da tabela Instructions com Id da tabela de Recipes
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Instructions_Recipe_Id", RECIPE_TABLE_NAME, "Id")
                //criando regra para quando a receita for deletada, também deletar as instruções associadas, sugerido utilizar o método OnDelete na tabela auxiliar e não na tabela principal
                .OnDelete(System.Data.Rule.Cascade);

            CreateTable("DishTypes")
                .WithColumn("Type").AsInt32().NotNullable()
                //criando relacionamento das colunas RecipeId da tabela DishTypes com Id da tabela de Recipes
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_DishTypes_Recipe_Id", RECIPE_TABLE_NAME, "Id")
                //criando regra para quando a receita for deletada, também deletar os tipos de pratos associados, sugerido utilizar o método OnDelete na tabela auxiliar e não na tabela principal
                .OnDelete(System.Data.Rule.Cascade);
        }
    }
}
