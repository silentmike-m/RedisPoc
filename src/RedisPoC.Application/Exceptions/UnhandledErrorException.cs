namespace RedisPoC.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using RedisPoC.Application.Common.Constants;
    using ApplicationException = RedisPoC.Application.Common.ApplicationException;

    [Serializable]
    public sealed class UnhandledErrorException : ApplicationException
    {
        public override string Code => ErrorCodes.UNHANDLED_ERROR;

        public UnhandledErrorException()
            : base("Unhandled error has occurred")
        {
        }

        private UnhandledErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
