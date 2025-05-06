using Bogus;
using CommonTestUtilities.Cryptography;
using MyRecipeBook.Domain.Entities;

namespace CommonTestUtilities.Entities
{
    public class UserBuilder
    {
        //função Build() para devolver dois parâmetros para user e password para especificar o retorno do UserBuilder no teste
        public static (User user, string password) Build()
        {
            var passwordEncripter = PasswordEncripterBuilder.Build();
            var password = new Faker().Internet.Password();

            var user = new Faker<User>()
                .RuleFor(user => user.Id, () => 1)
                .RuleFor(user => user.Name, f => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(user => user.UserIdentifier, _ => Guid.NewGuid()) //underline na lambda para sinalizar que só será retornado o Guid do UserIdentifier
                .RuleFor(user => user.Password, f => passwordEncripter.Encrypt(password));

            return (user, password);
        }
    }
}
