using CommonTestUtilities.Requests;
using System.Collections;

namespace UseCases.Test.Recipe.InlineDatas;

// Classe usada para fornecer dados de teste parametrizado no xUnit
public class ImageTypesInlineData : IEnumerable<object[]>
{
    // Método que retorna o enumerador para iterar sobre os dados de teste
    public IEnumerator<object[]> GetEnumerator()
    {
        // Obtém uma coleção de imagens mockadas (por exemplo, IFormFile) para uso nos testes
        var images = FormFileBuilder.ImageCollection();

        // Itera sobre cada imagem da coleção
        foreach (var image in images)
            // Para cada imagem, retorna um array de objetos contendo a imagem
            // Esse formato é necessário para funcionar com [Theory] no xUnit
            yield return new object[] { image };
    }

    // Implementação do IEnumerable não genérico que apenas chama o genérico acima
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
