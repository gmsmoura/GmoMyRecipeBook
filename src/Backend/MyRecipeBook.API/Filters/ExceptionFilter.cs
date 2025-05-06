using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using System;
using System.Net;

namespace MyRecipeBook.API.Filters
{
    
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            
            if (context.Exception is MyRecipeBookException)
                HandleProjectException(context);
            else
                
                ThrowUnknowException(context);
        }

        private static void HandleProjectException(ExceptionContext context)
        {
            if (context.Exception is InvalidLoginException)
            {
                
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
            else if (context.Exception is ErrorOnValidationException exception)
            {

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                context.Result = new BadRequestObjectResult(new ResponseErrorJson(exception.GetErrorMessages()));
            }
            else if (context.Exception is NotFoundException)
            {
                
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
        }

        private static void ThrowUnknowException(ExceptionContext context)
        {
            if (context.Exception is ErrorOnValidationException)
            {
                
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.UNKNOWN_ERROR));
            }
        }
    }
}
