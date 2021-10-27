namespace RedisPoC.WebApi.Filters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.Logging;
    using RedisPoC.Application.Common.Constants;
    using RedisPoC.Application.Exceptions;
    using RedisPoC.WebApi.Models;
    using ApplicationException = RedisPoC.Application.Common.ApplicationException;

    // ReSharper disable once ClassNeverInstantiated.Global
    [ExcludeFromCodeCoverage]
    public sealed class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> logger;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger) =>
            (this.logger) = (logger);

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleApplicationException(ExceptionContext context)
        {
            var exception = context.Exception as ApplicationException;

            if (exception is null)
            {
                throw new UnhandledErrorException();
            }

            var response = new BaseResponse<object>
            {
                Code = exception.Code,
                Error = exception.Message,
            };

            context.Result = new ObjectResult(response)
            {
                ContentTypes = new MediaTypeCollection
                {
                    MediaTypeNames.Application.Json,
                },
                StatusCode = 200,
            };
            context.ExceptionHandled = true;
        }

        private void HandleException(ExceptionContext context)
        {
            this.logger.LogError(context.Exception, context.Exception.Message);

            var exceptionHandler = context.Exception switch
            {
                ValidationException => this.HandleValidationException,
                ApplicationException => this.HandleApplicationException,
                _ => new Action<ExceptionContext>(this.HandleUnknownException),
            };

            exceptionHandler.Invoke(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var response = new BaseResponse<object>
            {
                Code = ErrorCodes.UNKNOWN_ERROR,
                Error = ErrorCodes.UNKNOWN_ERROR_MESSAGE,
            };

            context.Result = new ObjectResult(response)
            {
                ContentTypes = new MediaTypeCollection
                {
                    MediaTypeNames.Application.Json,
                },
                StatusCode = 200,
            };
            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;

            if (exception is null)
            {
                throw new UnhandledErrorException();
            }

            var response = new BaseResponse<object>
            {
                Code = exception.Code,
                Error = exception.Message,
                Response = exception.Errors,
            };

            context.Result = new ObjectResult(response)
            {
                ContentTypes = new MediaTypeCollection
                {
                    MediaTypeNames.Application.Json,
                },
                StatusCode = 200,
            };
            context.ExceptionHandled = true;
        }
    }
}
