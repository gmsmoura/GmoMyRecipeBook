using Bogus;
using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace CommonTestUtilities.BlobStorage;
public class BlobStorageServiceBuilder
{
    private readonly Mock<IBlobStorageService> _mock;

    public BlobStorageServiceBuilder() => _mock = new Mock<IBlobStorageService>();

    public BlobStorageServiceBuilder GetFileUrl(User user, string? fileName)
    {
        // Se o nome do arquivo for nulo ou vazio, não faz nada e retorna o builder atual
        if (string.IsNullOrWhiteSpace(fileName))
            return this;

        // Cria uma URL fake usando a biblioteca Bogus (Faker)
        var faker = new Faker();
        var imageUrl = faker.Image.LoremFlickrUrl();

        // Configura o mock para que, ao chamar GetFileUrl com esse user e fileName,
        // ele retorne a URL gerada fake de forma assíncrona
        _mock.Setup(blobStorage => blobStorage.GetFileUrl(user, fileName)).ReturnsAsync(imageUrl);

        // Retorna o próprio builder para permitir encadeamento de chamadas (fluent API)
        return this;
    }

    public BlobStorageServiceBuilder GetFileUrl(User user, IList<Recipe> recipes)
    {
        // Para cada receita na lista, aplica o método que configura o mock de URL da imagem
        foreach (var recipe in recipes)
        {
            GetFileUrl(user, recipe.ImageIdentifier);
        }

        // Retorna o builder atual para permitir encadeamento
        return this;
    }

    public IBlobStorageService Build() => _mock.Object;
}
