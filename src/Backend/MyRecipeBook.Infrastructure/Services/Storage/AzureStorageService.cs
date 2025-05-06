using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;

namespace MyRecipeBook.Infrastructure.Services.Storage;

// classe responsável pelas operações de manipulação de arquivos de imagens no Azure Blob Storage
public class AzureStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task Upload(User user, Stream file, string fileName)
    {
        // Obtém o container (configurado no Azure) associado ao usuário a partir do identificador
        var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());

        // Cria o container se ele ainda não existir
        await container.CreateIfNotExistsAsync();

        // Obtém uma referência ao blob (arquivo) com o nome fornecido
        var blobClient = container.GetBlobClient(fileName);

        // Faz o upload do arquivo, sobrescrevendo se já existir
        await blobClient.UploadAsync(file, overwrite: true);
    }

    public async Task<string> GetFileUrl(User user, string fileName)
    {
        // Converte o identificador do usuário em nome de container
        var containerName = user.UserIdentifier.ToString();

        // Obtém o container
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Verifica se o container existe
        var exist = await containerClient.ExistsAsync();
        if (exist.Value.IsFalse())
            return string.Empty; // Retorna string vazia se o container não existir

        // Obtém o blob (arquivo) dentro do container
        var blobClient = containerClient.GetBlobClient(fileName);

        // Verifica se o arquivo existe
        exist = await blobClient.ExistsAsync();
        if (exist.Value)
        {
            // Cria um SAS (Shared Access Signature) com permissões de leitura e tempo de expiração
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b", // Indica que o recurso é um blob
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(MyRecipeBookRuleConstants.MAXIMUM_IMAGE_URL_LIFETIME_IN_MINUTES),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read); // Define permissão de leitura

            // Gera a URL com SAS e retorna como string
            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        // Retorna string vazia se o arquivo não existir
        return string.Empty;
    }

    // método para uso interno com a finalidade de executar o download localmente invés de usar o BlobStorage do Azure
    public async Task<Stream> Download(User user, string fileName)
    {
        // Obtém o container do usuário
        var containerName = user.UserIdentifier.ToString();

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Verifica se o container existe
        var exist = await containerClient.ExistsAsync();
        if (exist.Value.IsFalse())
            return new MemoryStream(); // Retorna um stream vazio se o container não existir

        // Obtém o blob (arquivo) dentro do container
        var blobClient = containerClient.GetBlobClient(fileName);

        // Verifica se o arquivo existe
        exist = await blobClient.ExistsAsync();

        // Se o arquivo existir, baixa o conteúdo do blob
        if (exist.Value.IsFalse())
            return new MemoryStream();

        // Faz o download do conteúdo do blob e retorna como stream
        var result = await blobClient.DownloadContentAsync();

        return result.Value.Content.ToStream();
    }

    // Método responsável por deletar um arquivo (blob) do container do usuário no Azure Blob Storage
    public async Task Delete(User user, string fileName)
    {
        // Obtém o container do usuário com GetBlobContainerClient()
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());

        // Verifica se o container existe
        var exist = await containerClient.ExistsAsync();
        if (exist.Value)
        {
            // Deleta o arquivo (blob) se ele existir
            await containerClient.DeleteBlobIfExistsAsync(fileName);
        }
    }

    // Método responsável por deletar o contatiner do usuário no Azure Blob Storage
    public async Task DeleteContainer(Guid userIdentifier)
    {
        // Obtém o container pelo identificador do usuário com GetBlobContainerClient()
        var container = _blobServiceClient.GetBlobContainerClient(userIdentifier.ToString());

        // Deleta o container se ele existir
        await container.DeleteIfExistsAsync();
    }
}