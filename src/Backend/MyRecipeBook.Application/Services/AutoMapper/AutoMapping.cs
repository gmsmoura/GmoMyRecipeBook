﻿using AutoMapper;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Dtos;
using Sqids;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    
    public class AutoMapping : Profile
    {
        
        private readonly SqidsEncoder<long> _idEncoder;

        
        public AutoMapping(SqidsEncoder<long> idEncoder) 
        {
            _idEncoder = idEncoder;
            RequestToDomain();
            DomainToResponse();
        }    

        private void RequestToDomain()
        {
            
            CreateMap<RequestRegisterUserJson, Domain.Entities.User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            

            CreateMap<RequestRecipeJson, Domain.Entities.Recipe>()
                
                .ForMember(dest => dest.Instructions, opt => opt.Ignore())
                
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(source => source.Ingredients.Distinct()))
                .ForMember(dest => dest.DishTypes, opt => opt.MapFrom(source => source.DishTypes.Distinct()));

            
            CreateMap<string, Domain.Entities.Ingredient>()
                .ForMember(dest => dest.Item, opt => opt.MapFrom(source => source));

            
            CreateMap<DishType, Domain.Entities.DishType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source));

            
            CreateMap<RequestInstructionJson, Domain.Entities.Instruction>();
        }

        private void DomainToResponse()
        {
            
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();

            
            CreateMap<Domain.Entities.Recipe, ResponseRegisteredRecipeJson>()
                
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEncoder.Encode(source.Id)));

            
            CreateMap<Domain.Entities.Recipe, ResponseShortRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEncoder.Encode(source.Id)))
                .ForMember(dest => dest.AmountIngredients, config => config.MapFrom(source => source.Ingredients.Count))
                
                .ForMember(dest => dest.DishTypes, config => config.MapFrom(source => source.DishTypes.Count));

            
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
