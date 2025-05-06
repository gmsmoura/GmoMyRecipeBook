using System.Collections;

namespace WebApi.Test.InlineData
{
    public class CultureInlineDataTest : IEnumerable<object[]>
    {
        //implementando enumerable com constantes para utilização em outras classes
        public IEnumerator<object[]> GetEnumerator()
        {
            //no return é possível devolter qualquer type de values (string, int, bool, double, etc)
            //yield responsável para sinalizar que as linhas abaixo sejam executados, caso não fosse informado o yield seria finalizado no primeiro return
            yield return new object[] { "en" };
            yield return new object[] { "pt-PT" };
            yield return new object[] { "pt-BR" };
            yield return new object[] { "fr" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
