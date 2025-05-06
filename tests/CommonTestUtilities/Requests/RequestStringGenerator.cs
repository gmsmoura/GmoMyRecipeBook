using Bogus;

namespace CommonTestUtilities.Requests;
public class RequestStringGenerator
{
    //classe responsável por gerar um faker de parágrafos para serem utilizados nos unit tests
    public static string Paragraphs(int minCharacters)
    {
        var faker = new Faker();

        var longText = faker.Lorem.Paragraphs(count: 7);

        while (longText.Length < minCharacters)
        {
            longText = $"{longText} {faker.Lorem.Paragraph()}";
        }

        return longText;
    }
}
