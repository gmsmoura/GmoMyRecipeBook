using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories
{
    public class UnitOfWorkBuilder
    {
        public static IUnitOfWork Build()
        {
            //como parâmetro do tipo/origem recebido para mockar será utilizado a interface IUnitOfWork para retornar uma implementação fake
            var mock = new Mock<IUnitOfWork>();

            //retornando a implementação fake de UnitOfWork
            return mock.Object;
        }
    }
}
