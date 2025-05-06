using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Token;

namespace CommonTestUtilities.Repositories;
// Classe auxiliar (builder) para facilitar a criação de instâncias mockadas de ITokenRepository em testes
public class TokenRepositoryBuilder
{
    // Campo privado que armazena o mock do repositório de tokens
    private readonly Mock<ITokenRepository> _repository;

    // Construtor que inicializa o mock
    public TokenRepositoryBuilder() => _repository = new Mock<ITokenRepository>();

    // Método que configura o comportamento do mock para retornar um RefreshToken específico
    public TokenRepositoryBuilder Get(RefreshToken? refreshToken)
    {
        // Se o token passado não for nulo, configura o mock para que,
        // ao chamar Get(tokenValue), ele retorne o objeto fornecido
        if (refreshToken is not null)
            _repository
                .Setup(repository => repository.Get(refreshToken.Value)) // Aqui está o problema: refreshToken.Value é uma string, mas você está usando como chave para buscar o próprio token. Isso causará erro se não corrigido.
                .ReturnsAsync(refreshToken);

        return this;
    }

    // Método que retorna a instância mockada de ITokenRepository pronta para uso em testes
    public ITokenRepository Build() => _repository.Object;
}

