using Microsoft.AspNetCore.Http;

namespace CommonTestUtilities.Requests;
public class FormFileBuilder
{
    // classe responsável pela criação de arquivos para os testes em formatos png, jpg e txt
    public static IFormFile Png()
    {
        // Observação: No Visual Studio, é importante marcar o arquivo como "Copy always" nas propriedades,
        // para que ele seja copiado automaticamente para a pasta de saída (bin/...) e possa ser acessado no teste.

        // Abre o arquivo PNG existente na pasta "Files" em modo somente leitura.
        var stream = File.OpenRead("Files/FilePng.png");

        // Cria um objeto do tipo FormFile simulando o upload de um arquivo via HTTP.
        var file = new FormFile(
            baseStream: stream,          // Stream com os dados do arquivo.
            baseStreamOffset: 0,         // Posição inicial da leitura do stream.
            length: stream.Length,       // Tamanho total do arquivo.
            name: "File",                // Nome do campo no formulário.
            fileName: "IMG0001.png")     // Nome do arquivo (como seria enviado).
        {
            Headers = new HeaderDictionary(), // Inicializa os headers HTTP (pode ficar vazio no teste).
            ContentType = "image/png"         // Define o tipo de conteúdo como imagem PNG.
        };

        // Retorna o arquivo mockado, que pode ser usado em testes como se fosse um upload real.
        return file;
    }


    public static IFormFile Jpg()
    {
        var stream = File.OpenRead("Files/FileJpg.jpg");

        var file = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: stream.Length,
            name: "File",
            fileName: "IMG0001.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };

        return file;
    }

    public static IFormFile Txt()
    {
        var stream = File.OpenRead("Files/FileText.txt");

        var file = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: stream.Length,
            name: "File",
            fileName: "FILE0001.txt")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        return file;
    }

    public static IList<IFormFile> ImageCollection()
    {
        return
        [
            Png(),
            Jpg()
        ];
    }
}
