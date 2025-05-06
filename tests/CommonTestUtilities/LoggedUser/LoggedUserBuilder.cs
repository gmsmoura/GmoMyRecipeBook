using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.LoggedUser;

namespace CommonTestUtilities.LoggedUser
{
    public class LoggedUserBuilder
    {
        public static ILoggedUser Build(User user)
        {
            //cria o mock do user logado
            var mock = new Mock<ILoggedUser>();

            //configuração chamando a função User() de ILoggedUser para retorno do usuário 
            mock.Setup(x => x.User()).ReturnsAsync(user);

            return mock.Object;
        }
    }
}
