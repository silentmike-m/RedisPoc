namespace RedisPoC.Application.Common
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public abstract class ApplicationException : Exception
    {
        public Guid Id { get; protected set; }
        public abstract string Code { get; }

        protected ApplicationException(string message, Exception? innerException = null)
            : base(message, innerException)
        { }

        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
