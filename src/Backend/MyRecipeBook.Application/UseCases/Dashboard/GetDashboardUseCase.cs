using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.Dashboard;
public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IBlobStorageService _blobStorageService;

    public GetDashboardUseCase(
        IRecipeReadOnlyRepository repository,
        IMapper mapper,
        ILoggedUser loggedUser,
        IBlobStorageService blobStorageService)
    {
        _repository = repository;
        _mapper = mapper;
        _loggedUser = loggedUser;
        _blobStorageService = blobStorageService;
    }

    // Método público assíncrono que retorna um objeto ResponseRecipesJson
    public async Task<ResponseRecipesJson> Execute()
    {
        // Obtém o usuário logado por meio do serviço de autenticação (provavelmente vem do contexto do token)
        var loggedUser = await _loggedUser.User();

        // Busca do repositório as receitas desse usuário para exibição no dashboard
        var recipes = await _repository.GetForDashboard(loggedUser);

        // Retorna um objeto de resposta contendo as receitas convertidas no formato simplificado (com imagem se houver)
        return new ResponseRecipesJson
        {
            // Usa o método de extensão MapToShortRecipeJson para mapear a lista de Recipe para ResponseShortRecipeJson
            Recipes = await recipes.MapToShortRecipeJson(loggedUser, _blobStorageService, _mapper)
        };
    }
}
