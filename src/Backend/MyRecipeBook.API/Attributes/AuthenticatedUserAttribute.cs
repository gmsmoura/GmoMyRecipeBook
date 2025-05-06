using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;
using System.Reflection;

namespace MyRecipeBook.API.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthenticatedUserAttribute : TypeFilterAttribute
    {
        public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
        {
        }
    }
}
