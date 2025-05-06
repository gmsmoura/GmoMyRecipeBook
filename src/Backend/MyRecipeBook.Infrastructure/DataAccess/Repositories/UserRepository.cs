using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;
using System.Diagnostics.Metrics;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    //herdando as duas interfaces para separação das regras de negócios, interface são como contratos
    public class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository, IUserDeleteOnlyRepository
    {
        private readonly MyRecipeBookDbContext _dbContext;

        //construtor
        public UserRepository(MyRecipeBookDbContext dbContext) => _dbContext = dbContext;

        //adicionando a entidade User e a prop User user como parâmetro, sempre que tiver a opção de utilizar métodos assincronos é o ideal
        public async Task Add(User user) => await _dbContext.Users.AddAsync(user);

        //verificando se existe user ativo com o email, entre <> informando que a Task irá devolver um value boleano
        public async Task<bool> ExistActiveUserWithEmail(string email) => await _dbContext.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);
        
        //verificando se existe user ativo com Guid "userIdentifier", comparando o value do parâmetro com o que o user pode conter já preenchido
        public async Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier) => await _dbContext.Users.AnyAsync(user => user.UserIdentifier.Equals(userIdentifier) && user.Active);

        public async Task<User?> GetByEmailAndPassword(string email, string password)
        {
            //utilizando função AsNoTracking para evitar que a entidade user não seja atualizada por melhoria de performance do repositório para métodos assíncronos
            var response = await _dbContext
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Active && user.Email.Equals(email) && user.Password.Equals(password));

            return response;
        }

        public async Task<User> GetById(long id)
        {
            //para este caso de GetById não será utilizado o AsNoTracking, pois será um cenário de atualização de dados da regra de negócio de IUserUpdateOnlyRepository
            return await _dbContext
                .Users
                .FirstAsync(user => user.Id == id);
        }

        //função para atualização do user na base de dados utilizando via void
        public void Update(User user) => _dbContext.Users.Update(user);

        public async Task DeleteAccount(Guid userIdentifier)
        {
            // Busca o usuário no banco de dados com base no identificador único (GUID)
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserIdentifier == userIdentifier);

            // Se não encontrar o usuário, encerra o método (nada será feito)
            if (user is null)
                return;

            // Seleciona todas as receitas que pertencem ao usuário encontrado
            var recipes = _dbContext.Recipes.Where(recipe => recipe.UserId == user.Id);

            // Remove todas as receitas encontradas relacionadas ao usuário com uso do método RemoveRange()
            // Lembrar de deletar tudo relacionado ao user antes de deletar o user em definitivo para evitar exceptions e erros
            _dbContext.Recipes.RemoveRange(recipes);

            // Remove o próprio usuário do banco de dados
            _dbContext.Users.Remove(user);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _dbContext
                .Users
                .AsNoTracking() // Consulta sem rastreamento, útil para leitura (melhora desempenho)
                .FirstOrDefaultAsync(user => user.Active && user.Email.Equals(email));
        }
    }
}
