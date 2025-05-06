using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Image;
public class AddUpdateImageCoverUseCase : IAddUpdateImageCoverUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public AddUpdateImageCoverUseCase(
        ILoggedUser loggedUser,
        IRecipeUpdateOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }
    public async Task Execute(long recipeId, IFormFile file)
    {
        // Obtém o usuário atualmente logado no sistema.
        var loggedUser = await _loggedUser.User();

        // Busca a receita pelo ID informado, garantindo que pertença ao usuário logado.
        var recipe = await _repository.GetById(loggedUser, recipeId);

        // Se a receita não for encontrada, lança uma exceção de "não encontrado".
        if (recipe is null)
            throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

        // Abre o stream do arquivo enviado (geralmente uma imagem).
        var fileStream = file.OpenReadStream();

        // Valida se o arquivo é uma imagem e obtém sua extensão.
        (var isValidImage, var extension) = fileStream.ValidateAndGetImageExtension();

        // Se o arquivo não for uma imagem válida, lança uma exceção de validação.
        if (isValidImage.IsFalse())
        {
            throw new ErrorOnValidationException([ResourceMessagesExceptions.ONLY_IMAGES_ACCEPTED]);
        }

        // Se a receita ainda não tiver um identificador de imagem, cria um novo.
        if (string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            // Gera um identificador único com a extensão da imagem e associa à receita, o uso de $"" chama-se "String Interpolation"
            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            // Atualiza a receita no repositório com o novo identificador.
            _repository.Update(recipe);

            // Confirma as alterações no banco de dados.
            await _unitOfWork.Commit();
        }

       // Realiza o upload da imagem para o serviço de armazenamento em nuvem (blob storage).
       await _blobStorageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
    }
}
