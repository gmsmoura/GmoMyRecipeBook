using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.OpenAI;
using OpenAI.Chat;

namespace MyRecipeBook.Infrastructure.Services.OpenAI;
public class ChatGptService : IGenerateRecipeAI
{
    private readonly ChatClient _chatClient;

    public ChatGptService(ChatClient chatClient) => _chatClient = chatClient;

    public async Task<GeneratedRecipeDto> Generate(IList<string> ingredients)
    {
        // Cria uma lista de mensagens para enviar ao modelo de linguagem
        var messages = new List<ChatMessage>
        {
            // Adiciona uma mensagem do sistema para iniciar o contexto da conversa (provavelmente uma instrução como "Crie uma receita com base nos ingredientes")
            new SystemChatMessage(ResourceOpenAI.STARTING_GENERATE_RECIPE),

            // Adiciona uma mensagem do usuário com os ingredientes separados por ponto e vírgula
            new UserChatMessage(string.Join(";", ingredients))
        };

        // Envia a conversa para o cliente de chat da OpenAI e aguarda a resposta
        var completion = await _chatClient.CompleteChatAsync(messages);

        // Pega o conteúdo da resposta (primeira parte do texto), divide por quebras de linha, 
        // remove linhas vazias e também remove colchetes [ ] dos itens
        var responseList = completion.Value.Content[0].Text
            .Split("\n") // divide o texto em linhas
            .Where(response => response.Trim().Equals(string.Empty).IsFalse()) // remove linhas em branco
            .Select(item => item.Replace("[", "").Replace("]", "")) // remove colchetes
            .ToList();

        var step = 1;

        // Cria e retorna o objeto final com os dados extraídos da resposta do modelo
        return new GeneratedRecipeDto
        {
            // o título da receita (primeira linha)
            Title = responseList[0], 

            // Converte a segunda linha para o enum CookingTime
            CookingTime = (CookingTime)Enum.Parse(typeof(CookingTime), responseList[1]),

            // Terceira linha contém os ingredientes separados por ponto e vírgula
            Ingredients = responseList[2].Split(";"),

            // Quarta linha contém as instruções separadas por @, convertidas em uma lista de objetos com texto e número da etapa
            Instructions = responseList[3].Split("@").Select(instruction => new GeneratedInstructionDto
            {
                Text = instruction.Trim(), // texto da instrução
                Step = step++              // número da etapa, incrementado a cada item
            }).ToList()
        };
    }
}
