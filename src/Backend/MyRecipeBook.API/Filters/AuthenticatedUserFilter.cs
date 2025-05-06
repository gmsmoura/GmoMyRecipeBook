using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters
{
    public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
    {
        private readonly IAccessTokenValidator _accessTokenValidator;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;

        public AuthenticatedUserFilter(
            IAccessTokenValidator accessTokenValidator
            , IUserReadOnlyRepository userReadOnlyRepository
        )
        {
            _accessTokenValidator = accessTokenValidator;
            _userReadOnlyRepository = userReadOnlyRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Console.WriteLine("AuthenticatedUserFilter foi chamado!");
            try
            {
                var token = TokenOnRequest(context);

                var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

                var exist = await _userReadOnlyRepository.ExistActiveUserWithIdentifier(userIdentifier);
                if (exist.IsFalse())
                {
                    throw new UnauthorizedException(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
                }

                if (exist.IsFalse())
                {
                    throw new UnauthorizedException(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
                }
            }
            
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("TokenExpired")
                {
                    TokenExpired = true
                });
            }
            
            catch (MyRecipeBookException ex)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
            }
            
            catch
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
            }
        }

        private static string TokenOnRequest(AuthorizationFilterContext filterContext)
        {
            var authentication = filterContext.HttpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authentication) || !authentication.StartsWith("Bearer "))
            {
                throw new UnauthorizedException(ResourceMessagesExceptions.NO_TOKEN);
            }

            return authentication["Bearer ".Length..].Trim();
        }
    }
}
