using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Token;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;
// Repositório responsável por lidar com operações de persistência relacionadas aos tokens de refresh
public class TokenRepository : ITokenRepository
{
    // Contexto do banco de dados (EF Core) injetado via construtor
    private readonly MyRecipeBookDbContext _dbContext;

    // Construtor que recebe o contexto e o armazena em um campo privado
    public TokenRepository(MyRecipeBookDbContext dbContext) => _dbContext = dbContext;

    // Busca um refresh token pelo valor e carrega o usuário relacionado
    public async Task<RefreshToken?> Get(string refreshToken)
    {
        return await _dbContext
            .RefreshTokens
            .AsNoTracking()                             // Indica que a entidade não será rastreada (apenas leitura)
            .Include(token => token.User)               // Inclui os dados do usuário relacionado ao token
            .FirstOrDefaultAsync(token => token.Value.Equals(refreshToken)); // Busca o primeiro token que corresponda ao valor
    }

    // Salva um novo refresh token e remove os anteriores do mesmo usuário
    public async Task SaveNewRefreshToken(RefreshToken refreshToken)
    {
        // Busca todos os tokens existentes do usuário
        var tokens = _dbContext.RefreshTokens.Where(token => token.UserId == refreshToken.UserId);

        // Remove os tokens antigos
        _dbContext.RefreshTokens.RemoveRange(tokens);

        // Adiciona o novo refresh token
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }
}
