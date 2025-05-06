using FileTypeChecker.Extensions;
using FileTypeChecker.Types;

namespace MyRecipeBook.Application.Extensions;
public static class StreamImageExtensions
{
    // facilitação de leitura utilizar os parâmetros com retorno nomeado, conforme foi usado o isValidImage e extension
    public static (bool isValidImage, string extension) ValidateAndGetImageExtension(this Stream stream)
    {
        // Inicializa a tupla de retorno como inválida e com extensão vazia.
        var result = (false, string.Empty);

        // Verifica se o stream corresponde ao formato PNG.
        if (stream.Is<PortableNetworkGraphic>())
            // Se for PNG, define como válido e normaliza a extensão.
            result = (true, NormalizeExtension(PortableNetworkGraphic.TypeExtension));
        // Caso contrário, verifica se é do tipo JPEG.
        else if (stream.Is<JointPhotographicExpertsGroup>())
            // Se for JPEG, define como válido e normaliza a extensão.
            result = (true, NormalizeExtension(JointPhotographicExpertsGroup.TypeExtension));

        // Reinicia a posição do stream para o início, para evitar problemas de leitura futura e renderização de imagem.
        stream.Position = 0;

        // Retorna o resultado indicando se é uma imagem válida e a extensão associada.
        return result;
    }

    private static string NormalizeExtension(string extension)
    {
        // Garante que a extensão tenha um ponto no início (ex: ".jpg" em vez de "jpg").
        return extension.StartsWith('.') ? extension : $".{extension}";
    }
}
