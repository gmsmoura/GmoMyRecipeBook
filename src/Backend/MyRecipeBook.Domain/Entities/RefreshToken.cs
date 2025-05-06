namespace MyRecipeBook.Domain.Entities
{
    // Classe que representa um token de atualização, usado para renovar o access token

    // Também é possível incluir outras propriedades para guardar dados adicionais, como data de expiração, IP do usuário, localização, etc
    public class RefreshToken : EntityBase
    {
        // Valor do refresh token (geralmente um GUID ou string aleatória segura)
        public string Value { get; set; } = string.Empty;

        // Chave estrangeira que referencia o usuário ao qual o token pertence
        public long UserId { get; set; }

        // Propriedade de navegação para o usuário associado ao token
        public User User { get; set; } = default!;
    }
}
