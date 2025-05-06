using Microsoft.AspNetCore.Http;

namespace MyRecipeBook.Communication.Requests;
public class RequestRegisterRecipeFormData : RequestRecipeJson
{
    // herdando da classe RequestRecipeJson para aplicar o registro da receita com a imagem
    // e a classe RequestRecipeJson está sendo usado pelo processo de update
    public IFormFile? Image { get; set; }
}
