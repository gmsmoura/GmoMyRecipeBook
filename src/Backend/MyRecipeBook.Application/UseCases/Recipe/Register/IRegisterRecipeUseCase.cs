using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;
public interface IRegisterRecipeUseCase
{
    public Task<ResponseRegisteredRecipeJson> Execute(RequestRegisterRecipeFormData request);
}
