using Bogus;
using MyRecipeBook.Communication.Requests;
using System.Security.Cryptography.X509Certificates;

namespace CommonTestUtilities.Requests
{
    
    public class RequestRegisterUserJsonBuilder
    {
        public static RequestRegisterUserJson Build(int passwordLength = 8) 
        {
            
            
            return new Faker<RequestRegisterUserJson>()
                .RuleFor(user => user.Name, (faker) => faker.Person.FirstName)
                .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name))
                .RuleFor(user => user.Password, (faker) => faker.Internet.Password(passwordLength));
        }
    }
}
