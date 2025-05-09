﻿using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenRepository _tokenRepository;

        public DoLoginUseCase(
            IUserReadOnlyRepository repository
            , IPasswordEncripter passwordEncripter
            , IAccessTokenGenerator accessTokenGenerator
            , IRefreshTokenGenerator refreshTokenGenerator
            , IUnitOfWork unitOfWork
            , ITokenRepository tokenRepository)
        {
            _repository = repository;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _unitOfWork = unitOfWork;
            _tokenRepository = tokenRepository;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            
            var user = await _repository.GetByEmail(request.Email);

            
            if (user is null || _passwordEncripter.IsValid(request.Password, user.Password).IsFalse())
                throw new InvalidLoginException();

            
            var refreshToken = await CreateAndSaveRefreshToken(user);

            
            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Email = user.Email,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier), 
                    RefreshToken = refreshToken 
                }
            };
        }

        
        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            
            var refreshToken = new Domain.Entities.RefreshToken
            {
                Value = _refreshTokenGenerator.Generate(),
                UserId = user.Id
            };

            
            await _tokenRepository.SaveNewRefreshToken(refreshToken);

            
            await _unitOfWork.Commit();

            
            return refreshToken.Value;
        }
    }
}
