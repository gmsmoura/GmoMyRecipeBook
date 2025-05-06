using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;
public class FilterRecipeUseCase : IFilterRecipeUseCase
{
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly IBlobStorageService _blobStorageService;

    public FilterRecipeUseCase(
        IMapper mapper,
        IRecipeReadOnlyRepository repository,
        ILoggedUser loggedUser,
        IBlobStorageService blobStorageService
        )
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _repository = repository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
    {
        //validando a requisição
        Validate(request);

        //recebendo o user logado
        var loggedUser = await _loggedUser.User();

        //utilização do DTO para resumir o retorno somente do que será necessário do filtro de receitas
        var filters = new Domain.Dtos.FilterRecipesDto
        {
            RecipeTitle_Ingredient = request.RecipeTitle_Ingredient,
            CookingTimes = request.CookingTimes.Distinct().Select(c => (Domain.Enums.CookingTime)c).ToList(),
            Difficulties = request.Difficulties.Distinct().Select(c => (Domain.Enums.Difficulty)c).ToList(),
            DishTypes = request.DishTypes.Distinct().Select(c => (Domain.Enums.DishType)c).ToList()
        };

        //utilização do repositório para filtrar diretamente do repositório com as respectivas queries 
        var recipes = await _repository.Filter(loggedUser, filters);

        //retornando resultado filtrado através do repositório
        return new ResponseRecipesJson
        {
            Recipes = await recipes.MapToShortRecipeJson(loggedUser, _blobStorageService, _mapper)
        };
    }

    //validando o retorno e em casos de erros exibindo as respectivas mensagens
    private static void Validate(RequestFilterRecipeJson request)
    {
        var validator = new FilterRecipeValidator();

        var result = validator.Validate(request);

        if (result.IsValid.IsFalse())
        {
            //tratando a lista de erros com Distinct() para evitar duplicação de retorno das mensagens devido poder existir mais de um cenário de erro, ex: valores vazios, em branco ou null
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).Distinct().ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
