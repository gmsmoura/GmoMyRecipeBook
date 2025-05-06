using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructure.DataAccess
{
    public class MyRecipeBookDbContext : DbContext
    {
        //construtor para receber as opções de banco de dados (SQL, MySql, etc)
        public MyRecipeBookDbContext(DbContextOptions options) : base(options) { }

        //DbSet representa uma tabela e está sendo acessada dentro de <User> com Users
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        //sobreescrevendo método do Entity Framework
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //forçando o entity a utilizar as configs do projeto atual, neste caso com o MyRecipeBookDbContext
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyRecipeBookDbContext).Assembly);
        }
    }
}
