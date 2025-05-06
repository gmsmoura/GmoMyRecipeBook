using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;
public class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;

    public RegisterRecipeUseCase(
        ILoggedUser loggedUser,
        IRecipeWriteOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggedUser = loggedUser;
        _blobStorageService = blobStorageService;
    }

    public async Task<ResponseRegisteredRecipeJson> Execute(RequestRegisterRecipeFormData request)
    {
        //chamando o método Validate() para validar a entrada de dados da requisição
        Validate(request);

        //recuperando o usuário logado
        var loggedUser = await _loggedUser.User();

        //mapeando da requisição RequestRecipeJson para a entidade Recipe e guardando o UserId do usuário proprietário da receita (guarda o Id do usuário logado)
        var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
        recipe.UserId = loggedUser.Id;

        //retornando as instructions e ordenando do menor para o maior e guarda na lista
        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();

        //varrendo os itens de instructions e com base no número do step inicializa através do index iniciando de 0 e assim sucessivamente
        for (var index = 0; index < instructions.Count; index++)
            instructions[index].Step = index + 1;

        //mapenado os instructions separadamente devido ser lista e precisar mapear item por item
        recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        //verificando se a imagem não é nula, caso não seja, abre o stream do arquivo enviado (geralmente uma imagem)
        if (request.Image is not null)
        {
            var fileStream = request.Image.OpenReadStream();

            // validando se o arquivo é uma imagem e obtendo sua extensão
            (var isValidImage, var extension) = fileStream.ValidateAndGetImageExtension();

            // se o arquivo não for uma imagem válida, lança uma exceção de validação
            if (isValidImage.IsFalse())
            {
                throw new ErrorOnValidationException([ResourceMessagesExceptions.ONLY_IMAGES_ACCEPTED]);
            }

            // Se a receita ainda não tiver um identificador de imagem, cria um novo
            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            // atualiza a receita no repositório com o novo identificador
            await _blobStorageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
        }

        //adicionando o item ao repositorio com Add()
        await _repository.Add(recipe);

        //persistindo os dados com Commit()
        await _unitOfWork.Commit();

        return _mapper.Map<ResponseRegisteredRecipeJson>(recipe);
    }

    //método para chamar o Validate do FluentValidator que foi criado para validação das entradas com as respectivas mensagens
    private static void Validate(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid.IsFalse())
            //utilização do Distinct() para não retornar mensagens duplicdas, em caso da entrada estar "", null, "    "
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
    }
}
