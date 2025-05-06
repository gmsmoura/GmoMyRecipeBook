namespace MyRecipeBook.Domain.Repositories.User;

//novo repository do tipo Interface de User para separar as regras de negócios, para evitar deixar tudo centralizado em um devido a permissões de CRUD no banco de dados e assim facilitar manutenção
public interface IUserWriteOnlyRepository
{
    //quem implementar essa interface terá a função de adicionar
    public Task Add(Entities.User user);
}
