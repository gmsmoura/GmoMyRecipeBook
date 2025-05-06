using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class UserUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IUserUpdateOnlyRepository> _repository;

    public UserUpdateOnlyRepositoryBuilder() => _repository = new Mock<IUserUpdateOnlyRepository>();

    //funções abaixo diferentes das funções de UserReadOnlyRepositoryBuilder que são void por não devolver nada, essas já precisam devolver o objeto buscado da base para
    public UserUpdateOnlyRepositoryBuilder GetById(User user)
    {
        _repository.Setup(x => x.GetById(user.Id)).ReturnsAsync(user);

        //this para devolver a própria instância (UserUpdateOnlyRepositoryBuilder) que foi usada para chamar a função GetById
        return this;
    }

    //por fim chama o Build de IUserUpdateOnlyRepository
    public IUserUpdateOnlyRepository Build() => _repository.Object;
}
