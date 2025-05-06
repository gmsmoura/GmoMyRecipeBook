using Moq;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserWriteOnlyRepositoryBuilder
    {
        public static IUserWriteOnlyRepository Build()
        {
            //como parâmetro do tipo/origem recebido para mockar será utilizado a interface IUnitOfWork para retornar uma implementação fake
            var mock = new Mock<IUserWriteOnlyRepository>();

            //retornando a implementação fake de UnitOfWork
            return mock.Object;
        }
    }
}
