using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace BugStore.API.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BugStoreException)
                HandleCustomException(context);
            else
                HandleUnknownException(context);
        }

        private void HandleCustomException(ExceptionContext context)
        {
            if (context.Exception is OnValidationException)
            {
                var exception = context.Exception as OnValidationException;
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(
                    new { errors = exception!.ErrorMessages }
                );
            }
            else if (context.Exception is OnInvalidOperationException)
            {
                var exception = context.Exception as OnInvalidOperationException;
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(
                    new { errors = exception!.ErrorMessages }
                );
            }
            else if (context.Exception is NotFoundException)
            {
                var exception = context.Exception as NotFoundException;
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(
                    new { errors = exception!.ErrorMessages }
                );
            }
            else if (context.Exception is ConflitException)
            {
                var exception = context.Exception as ConflitException;
                context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Result = new ConflictObjectResult(
                    new { errors = exception!.ErrorMessages }
                );
            }
            else if (context.Exception is FormatInvalidException)
            {
                var exception = context.Exception as FormatInvalidException;
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(
                    new { errors = exception!.ErrorMessages }
                );
            }
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(
                new { error = ResourceExceptionMessage.UNKNOWN_ERROR }
            );
        }
    }
}
