namespace RedisPoC.Application.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentValidation.Results;
    using RedisPoC.Application.Common.Constants;
    using ApplicationException = RedisPoC.Application.Common.ApplicationException;

    [Serializable]
    public sealed class ValidationException : ApplicationException
    {
        public override string Code => ErrorCodes.VALIDATION_FAILED;
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this("Errors have occurred during validation process")
        {
            var errors = failures
                .GroupBy(e => e.ErrorCode, e => e.ErrorMessage)
                .ToDictionary(
                    failureGroup => failureGroup.Key,
                    failureGroup => failureGroup.ToArray()
                );

            Errors = errors;
        }

        public ValidationException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }

        private ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
