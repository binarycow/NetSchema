using System;

namespace NetSchema.Common.Exceptions
{
    public class NetSchemaDataException : NetSchemaException
    {
        public NetSchemaDataException(string message) : base(message)
        {
        }

        public NetSchemaDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}