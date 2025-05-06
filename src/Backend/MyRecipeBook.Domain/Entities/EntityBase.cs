using System.Text.Json.Serialization;

namespace MyRecipeBook.Domain.Entities
{
    public class EntityBase
    {
        public long Id { get; set; }
        public bool Active { get; set; } = true;

        //UtcNow recomendado para pegar o timezone local (indepentente do país onde está sendo executado a ação
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
