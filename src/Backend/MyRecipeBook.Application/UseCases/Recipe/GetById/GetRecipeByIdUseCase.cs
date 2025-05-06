using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;
public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
{
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly IBlobStorageService _blobStorageService;

    public GetRecipeByIdUseCase(
        IMapper mapper,
        ILoggedUser loggedUser,
        IRecipeReadOnlyRepository repository,
        IBlobStorageService blobStorageService)
    {
        _repository = repository;
        _mapper = mapper;
        _loggedUser = loggedUser;
        _blobStorageService = blobStorageService;
    }

    //useCase responsável por buscar a receita pelo id e usuário
    public async Task<ResponseRecipeJson> Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();

        //toda busca está sendo feita pelo repositório RecipeRepository
        var recipe = await _repository.GetById(loggedUser, recipeId);

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

        // Mapeia o objeto 'recipe' para o tipo de resposta 'ResponseRecipeJson' usando AutoMapper
        var response = _mapper.Map<ResponseRecipeJson>(recipe);

        // Verifica se o campo 'ImageIdentifier' da receita não está vazio ou nulo
        if (recipe.ImageIdentifier.NotEmpty())
        {
            // Obtém a URL da imagem armazenada no Blob Storage, associada ao usuário logado
            var url = await _blobStorageService.GetFileUrl(loggedUser, recipe.ImageIdentifier);

            // Atribui a URL obtida ao campo 'ImageUrl' da resposta
            response.ImageUrl = url;
        }

        return response;
    }
}
