using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    //classe para estruturar o cenário de teste que será executado de fato nas classes de testes
    public class RequestUpdateUserJsonBuilder
    {
        public static RequestUpdateUserJson Build(int passwordLength = 10)
        {
            //retornando dados ficticios gerados pela biblioteca Bogus, ex:  (f) => f.Person.FirstName (importando via nugetPackage) e utilizando como type o objeto de RequestRegisterUserJson
            //RuleFor() do FluentValidator
            return new Faker<RequestUpdateUserJson>()
                .RuleFor(user => user.Name, (faker) => faker.Person.FirstName)
                .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name));
        }
    }
}
