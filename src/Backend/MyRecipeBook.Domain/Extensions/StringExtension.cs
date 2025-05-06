using System.Diagnostics.CodeAnalysis;

namespace MyRecipeBook.Domain.Extensions
{
    public static class StringExtension
    {
        //convenção utilizando ? para sinalizar que mesmo que o value seja nulo poderá ser utilizado
        //utilização do [NotNullWhen(true)] para sinalizar que quando a função retornar true, será garantido que o retorno não será nulo
        public static bool NotEmpty([NotNullWhen(true)] this string? value) => string.IsNullOrWhiteSpace(value).IsFalse();
    }
}
