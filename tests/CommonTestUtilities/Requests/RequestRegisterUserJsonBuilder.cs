using Bogus;
using MyRecipeBook.Communication.Requests;
using System.Security.Cryptography.X509Certificates;

namespace CommonTestUtilities.Requests
{
    //classe para estruturar o cenário de teste que será executado de fato nas classes de testes
    public class RequestRegisterUserJsonBuilder
    {
        public static RequestRegisterUserJson Build(int passwordLength = 8) 
        {
            //retornando dados ficticios gerados pela biblioteca Bogus, ex:  (f) => f.Person.FirstName (importando via nugetPackage) e utilizando como type o objeto de RequestRegisterUserJson
            //RuleFor() do FluentValidator
            return new Faker<RequestRegisterUserJson>()
                .RuleFor(user => user.Name, (faker) => faker.Person.FirstName)
                .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name))
                .RuleFor(user => user.Password, (faker) => faker.Internet.Password(passwordLength));
        }
    }
}
