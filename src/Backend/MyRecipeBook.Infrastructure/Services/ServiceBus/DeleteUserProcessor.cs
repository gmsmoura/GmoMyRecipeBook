using Azure.Messaging.ServiceBus;

namespace MyRecipeBook.Infrastructure.Services.ServiceBus;

// classe responsável por processar as mensagens da fila de exclusão de usuários
public class DeleteUserProcessor
{
    private readonly ServiceBusProcessor _processor;

    public DeleteUserProcessor(ServiceBusProcessor processor)
    {
        _processor = processor;
    }

    // método que irá processar as mensagens da fila de exclusão de usuários e controlorá a fila que desejo usar
    // futuramente poderá ser utilizado para processar outras filas
    public ServiceBusProcessor GetProcessor() => _processor;
}
