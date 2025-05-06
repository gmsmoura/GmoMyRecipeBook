using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.Extensions;
public static class RecipeListExtension
{
    // Método de extensão assíncrono que mapeia uma lista de objetos Recipe para uma lista de ResponseShortRecipeJson
    public static async Task<IList<ResponseShortRecipeJson>> MapToShortRecipeJson(
        this IList<Recipe> recipes,                  // Lista de receitas de entrada
        User user,                                   // Usuário logado (usado para buscar a URL da imagem)
        IBlobStorageService blobStorageService,      // Serviço responsável por buscar a URL da imagem no blob
        IMapper mapper)                              // Mapper para converter Recipe em ResponseShortRecipeJson
    {
        // Mapeia cada recipe da lista para um ResponseShortRecipeJson de forma assíncrona
        var result = recipes.Select(async recipe =>
        {
            // Usa o AutoMapper para converter um objeto Recipe em ResponseShortRecipeJson
            var response = mapper.Map<ResponseShortRecipeJson>(recipe);

            // Se a receita tiver uma imagem vinculada (ImageIdentifier não está vazio)
            if (recipe.ImageIdentifier.NotEmpty())
            {
                // Busca a URL da imagem no serviço de blob storage e atribui à resposta
                response.ImageUrl = await blobStorageService.GetFileUrl(user, recipe.ImageIdentifier);
            }

            // Retorna a resposta preenchida
            return response;
        });

        // Aguarda a conclusão de todas as tarefas assíncronas (mapeamentos + busca de URLs)
        var response = await Task.WhenAll(result);

        // Retorna a lista final com todas as receitas mapeadas
        return response;
    }
}
