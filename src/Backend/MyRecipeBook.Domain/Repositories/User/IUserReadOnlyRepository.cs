namespace MyRecipeBook.Domain.Repositories.User
{
    //novo repository do tipo Interface de User para separar as regras de negócios, para evitar deixar tudo centralizado em um devido a permissões de CRUD no banco de dados
    //e assim facilitar manutenção
    public interface IUserReadOnlyRepository
    {
        public Task<bool> ExistActiveUserWithEmail(string email);
        public Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier);
        public Task<Entities.User?> GetByEmail(string email);
    }
}
