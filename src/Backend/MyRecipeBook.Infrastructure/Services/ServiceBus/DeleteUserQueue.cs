using Azure.Messaging.ServiceBus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Infrastructure.Services.ServiceBus;
public class DeleteUserQueue : IDeleteUserQueue
{
    private readonly ServiceBusSender _serviceBusSender;

    public DeleteUserQueue(ServiceBusSender serviceBusSender)
    {
        _serviceBusSender = serviceBusSender;
    }

    // método que irá enviar a mensagem para a fila de exclusão
    public async Task SendMessage(User user)
    {
        // enviando a mensagem para a fila de exclusão do Service Bus e a mensagem a ser passada será sempre uma string (nesse caso será o identificador de um user)
        await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(user.UserIdentifier.ToString()));
    }
}
