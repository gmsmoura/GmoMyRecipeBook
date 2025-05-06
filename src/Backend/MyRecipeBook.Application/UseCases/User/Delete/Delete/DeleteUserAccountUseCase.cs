using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.User.Delete.Delete;
public class DeleteUserAccountUseCase : IDeleteUserAccountUseCase
{
    //separando e criando uma interface para o repositório de exclusão de usuário para não misturar com a lógica e interface de IUserWriteOnlyRepository
    private readonly IUserDeleteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteUserAccountUseCase(
        IUserDeleteOnlyRepository repository,
        IBlobStorageService blobStorageService,
        IUnitOfWork unitOfWork)
    {
        _blobStorageService = blobStorageService;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid userIdentifier)
    {
        // Remove o container de arquivos (ex: imagens) relacionado ao usuário no serviço de armazenamento em nuvem (Blob Storage)
        await _blobStorageService.DeleteContainer(userIdentifier);

        // Executa a exclusão do usuário e de suas receitas no banco de dados
        await _repository.DeleteAccount(userIdentifier);

        // Realiza o commit das alterações no banco de dados para efetivar a transação
        await _unitOfWork.Commit();
    }
}
