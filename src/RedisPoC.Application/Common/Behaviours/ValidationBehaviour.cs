﻿namespace RedisPoC.Application.Common.Behaviours
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using ValidationException = RedisPoC.Application.Exceptions.ValidationException;

    internal sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) =>
            (this.validators) = (validators);

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (this.validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validation = this.validators.Select(v => v.ValidateAsync(context, cancellationToken));
                var validationResults = await Task.WhenAll(validation);

                var failures = validationResults
                    .SelectMany(vr => vr.Errors)
                    .Where(vr => vr is not null)
                    .ToList();

                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}