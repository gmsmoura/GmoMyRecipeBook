using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserReadOnlyRepositoryBuilder
    {
        private readonly Mock<IUserReadOnlyRepository> _repository;

        //utilizando o constructor para iniciar uma nova instância de _repository de IUserReadOnlyRepository
        //para garantir que somente o constructor da classe poderá instanciá-lo (outras classses não poderão)
        public UserReadOnlyRepositoryBuilder() => _repository = new Mock<IUserReadOnlyRepository>();

        //método para validar se e-mail existe no unit test
        public void ExistActiveUserWithEmail(string email)
        {
            //método Setup para dar acesso as funções da interface para quando algo for chamado na interface obter um retorno
            //It.IsAny para informar no parâmetro que poderá ser um e-mail de qualquer tipo e que deverá retornar true
            _repository.Setup(repository => repository.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
        }

        public void GetByEmail(User user)
        {
            _repository.Setup(repository => repository.GetByEmail(user.Email)).ReturnsAsync(user);
        }
        public IUserReadOnlyRepository Build() => _repository.Object;
    }
}
