using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Application.UseCases.Login.External;
public class ExternalLoginUseCase : IExternalLoginUseCase
{
    private readonly IUserReadOnlyRepository _repository;
    private readonly IUserWriteOnlyRepository _repositoryWrite;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public ExternalLoginUseCase(
        IUserReadOnlyRepository repository,
        IUserWriteOnlyRepository repositoryWrite,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _accessTokenGenerator = accessTokenGenerator;
        _repository = repository;
        _repositoryWrite = repositoryWrite;
        _unitOfWork = unitOfWork;
    }

    // Método assíncrono que executa uma operação baseada no nome e e-mail recebidos
    public async Task<string> Execute(string name, string email)
    {
        // Busca um usuário existente pelo e-mail informado
        var user = await _repository.GetByEmail(email);

        // Se o usuário não existir, será criado um novo usuário
        // Também é possível direcionar o user para criar um cadastro pelo fluxo padrão para o cadastro na API ser criado
        if (user is null)
        {
            // Cria um novo usuário com o nome e e-mail fornecidos e senha padrão "-"
            user = new Domain.Entities.User
            {
                Name = name,
                Email = email,
                Password = "-" 
                // setando password padrão para usuários externos
                // neste caso não haverá problema de segurança
                // pois essa senha não será usada no fluxo de autenticação padrão da API (não autentica pelo fluxo de login padrão)
                // só será autenticado quando o fluxo de login com o Google for válido
            };

            // Adiciona o novo usuário no repositório
            await _repositoryWrite.Add(user);

            // Confirma a transação no banco de dados
            await _unitOfWork.Commit();
        }

        // Gera e retorna um token de acesso para o usuário
        return _accessTokenGenerator.Generate(user.UserIdentifier);
    }
}
