using System;

namespace NetSchema.Common.Exceptions
{
    public abstract class NetSchemaException : Exception
    {
        protected NetSchemaException(string message) : base(message)
        {
        }

        protected NetSchemaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class GenericNetSchemaException : NetSchemaException
    {
        public GenericNetSchemaException(string message) : base(message)
        {
        }

        public GenericNetSchemaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}