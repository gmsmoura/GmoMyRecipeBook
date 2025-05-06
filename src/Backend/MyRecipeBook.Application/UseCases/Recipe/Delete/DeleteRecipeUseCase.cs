using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;
public class DeleteRecipeUseCase : IDeleteRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _repositoryRead;
    private readonly IRecipeWriteOnlyRepository _repositoryWrite;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteRecipeUseCase(
        ILoggedUser loggedUser,
        IRecipeReadOnlyRepository repositoryRead,
        IRecipeWriteOnlyRepository repositoryWrite,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _loggedUser = loggedUser;
        _repositoryRead = repositoryRead;
        _repositoryWrite = repositoryWrite;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task Execute(long recipeId)
    {
        //capturando o user logado
        var loggedUser = await _loggedUser.User();

        //capturando a recipe pertencente ao user logado
        var recipe = await _repositoryRead.GetById(loggedUser, recipeId);

        //se a recipe for nula, lançar exceção de não encontrada
        if (recipe is null)
            throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

        //se a imagem não for vazia, deletar a imagem do blob storage
        if (recipe.ImageIdentifier.NotEmpty())
            await _blobStorageService.Delete(loggedUser, recipe.ImageIdentifier);

        await _repositoryWrite.Delete(recipeId);

        await _unitOfWork.Commit();
    }
}