namespace MyRecipeBook.Infrastructure.Migrations
{
    //classe para configurar as versões de migrations e o modificador de acesso "abstract" para não deixa-la ser instanciada (mas permite ser implementada/herdada por outras classes)
    public abstract class DatabaseVersions
    {
        //var do type const não permite alterações em tempo de execução
        public const int TABLE_USER = 1;
        public const int TABLE_RECIPES = 2;
        public const int IMAGES_FOR_RECIPES = 3;
        public const int TABLE_REFRESH_TOKEN = 4;
    }
}
