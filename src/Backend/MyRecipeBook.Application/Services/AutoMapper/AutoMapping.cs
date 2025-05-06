using AutoMapper;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Dtos;
using Sqids;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    //herdando da classe Profile do AutoMapper
    public class AutoMapping : Profile
    {
        //variável para utilização da criptografia do id
        private readonly SqidsEncoder<long> _idEncoder;

        //criando construtor da classe para chamar os métodos utilizados
        public AutoMapping(SqidsEncoder<long> idEncoder) 
        {
            _idEncoder = idEncoder;
            RequestToDomain();
            DomainToResponse();
        }    

        private void RequestToDomain()
        {
            //o que está entre <> no método se trata da fonte de onde está vindo os dados, utilizando os dois parâmetros RequestRegisterUserJson, Domain.Entities.User
            CreateMap<RequestRegisterUserJson, Domain.Entities.User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            //mapeia a propriedade de destino (neste caso a entidade User) para a entidade fonte (src) de origem com o MapFrom() e caso não seja utilizada pode ser usado o Ignore() 

            CreateMap<RequestRecipeJson, Domain.Entities.Recipe>()
                //ignorando o mapeando da entidade/prop Instructions pois ela está sendo mapeada separadamente direto no método de Create
                .ForMember(dest => dest.Instructions, opt => opt.Ignore())
                //mepeando Ingredients e DishTypes com Distinct() para ignorar itens duplicados
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(source => source.Ingredients.Distinct()))
                .ForMember(dest => dest.DishTypes, opt => opt.MapFrom(source => source.DishTypes.Distinct()));

            //necessário mapear a prop Item para a entidade Ingredient devido a prop Item ser do tipo String (propriedade que será usado para guardar o retorno)
            CreateMap<string, Domain.Entities.Ingredient>()
                .ForMember(dest => dest.Item, opt => opt.MapFrom(source => source));

            //necessário mapear a prop Type para a entidade DishType devido a prop Type ser do tipo Enum  (propriedade que será usado para guardar o retorno)
            CreateMap<DishType, Domain.Entities.DishType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source));

            //necessário mapear RequestInstructionJson para a entidade Instruction, da maneira abaixo já funciona devido as props serem as mesmas da request para a entidade
            CreateMap<RequestInstructionJson, Domain.Entities.Instruction>();
        }

        private void DomainToResponse()
        {
            //mapeamento de domínio (entidades) para response, testa no DoLoginUseCase no método Execute
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();

            //sempre que for mapeado uma entidade em uma resposta, será utilizado o idEncoder
            CreateMap<Domain.Entities.Recipe, ResponseRegisteredRecipeJson>()
                //criptografando o Id da response com _idEncoder.Encode(source.Id) para o Id da entidade Receita
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEncoder.Encode(source.Id)));

            //mapeando uma entidade Recipe para ResponseShortRecipeJson (a resposta ao usuário, o que será retornado)
            CreateMap<Domain.Entities.Recipe, ResponseShortRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEncoder.Encode(source.Id)))
                .ForMember(dest => dest.AmountIngredients, config => config.MapFrom(source => source.Ingredients.Count))
                //TO DO, criar mapeamento para trazer a lista de DishTypes na response, será necessário alterar o objeto de response
                .ForMember(dest => dest.DishTypes, config => config.MapFrom(source => source.DishTypes.Count));

            //mapeando uma entidade Recipe para ResponseRecipeJson (a resposta ao usuário, o que será retornado)
            CreateMap<Domain.Entities.Recipe, ResponseRecipeJson>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => _idEncoder.Encode(source.Id)))
                .ForMember(dest => dest.DishTypes, opt => opt.MapFrom(source => source.DishTypes.Select(r => r.Type)));

            CreateMap<Domain.Entities.Ingredient, ResponseIngredientJson>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => _idEncoder.Encode(source.Id)));

            CreateMap<Domain.Entities.Instruction, ResponseInstructionJson>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => _idEncoder.Encode(source.Id)));
        }
    }
}
