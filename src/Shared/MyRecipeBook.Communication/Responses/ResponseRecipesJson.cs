namespace MyRecipeBook.Communication.Responses;
public class ResponseRecipesJson
{
    //lista para devolver os values das propriedades mapeadas em ResponseShortRecipeJson do filtro de receitas
    public IList<ResponseShortRecipeJson> Recipes { get; set; } = [];
}
