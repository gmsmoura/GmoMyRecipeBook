using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_REFRESH_TOKEN, "Create new table to save refresh token")]

    public class Version0000004 : VersionBase
    {
        public override void Up()
        {
            CreateTable("RefreshTokens")
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_RefreshToken_User_Id", "Users", "Id");
        }
    }
}
