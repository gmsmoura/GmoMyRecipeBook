using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Application.UseCases.User.Delete.Request;

// classe de request para solicitação de exclusão de usuário (para enviar o user a ser excluído para a fila do Service Bus)
public class RequestDeleteUserUseCase : IRequestDeleteUserUseCase
{
    private readonly IDeleteUserQueue _queue;
    private readonly IUserUpdateOnlyRepository _userUpdateRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public RequestDeleteUserUseCase(
        IDeleteUserQueue queue,
        IUserUpdateOnlyRepository userUpdateRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _queue = queue;
        _loggedUser = loggedUser;
        _userUpdateRepository = userUpdateRepository;
    }

    // método que irá desativar o user no banco de dados
    public async Task Execute()
    {
        var loggedUser = await _loggedUser.User();

        var user = await _userUpdateRepository.GetById(loggedUser.Id);

        user.Active = false;
        _userUpdateRepository.Update(user);

        // enviando as alterações no banco de dados
        await _unitOfWork.Commit();

        // enviado o user para a fila de exclusão
        await _queue.SendMessage(loggedUser);
    }
}
