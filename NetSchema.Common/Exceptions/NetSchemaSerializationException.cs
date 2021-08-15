using System;

namespace NetSchema.Common.Exceptions
{
    public class NetSchemaSerializationException : NetSchemaException
    {
        public NetSchemaSerializationException(string message) : base(message)
        {
        }

        public NetSchemaSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}