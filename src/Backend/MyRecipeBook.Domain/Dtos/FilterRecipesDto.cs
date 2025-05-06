using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Dtos;

//A principal diferença entre um record e uma class é que os records são imutáveis por padrão e...
//...oferecem implementação automática de igualdade por valor (ao invés de igualdade por referência).
//Isso os torna ideais para DTOs, onde geralmente queremos apenas representar dados e não modificar seu estado.
public record FilterRecipesDto
{
    //classe DTO para finalidade de transferência de dados sem expor as entidades reais do projeto
    //facilita evolução e manutenção, melhor performance e a compatibilidade com serialização e o uso do init para tornar o value da propriedade imutável/inalterável após ser instânciado
    public string? RecipeTitle_Ingredient { get; init; }
    public IList<CookingTime> CookingTimes { get; init; } = [];
    public IList<Difficulty> Difficulties { get; init; } = [];
    public IList<DishType> DishTypes { get; init; } = [];
}