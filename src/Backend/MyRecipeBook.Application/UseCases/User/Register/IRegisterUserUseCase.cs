using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public interface IRegisterUserUseCase
    {
        //'classe ResponseRegisteredUserJson' que será implementada na classe RegisterUserUseCase e como parâmetro será necessário chamar a regra de negócio 'classe controller RequestRegisterUserJson request'
        public Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request);
    }
}
