using System;

namespace NetSchema.Common.Exceptions
{
    internal class UnknownDerivedTypeException<T> : UnknownDerivedTypeNetSchemaException
    {
        public UnknownDerivedTypeException(Type actualType) : base(actualType, typeof(T))
        {
        }

        public UnknownDerivedTypeException(object? actualType) : base(actualType, typeof(T))
        {
        }
    }
    internal class UnknownDerivedTypeNetSchemaException : NetSchemaException
    {
        public const string MESSAGE = "{0} is an unknown derived type; expected a subclass of {1}";
        
        public UnknownDerivedTypeNetSchemaException(Type actualType, Type expectedType) 
            : base(CreateMessage(actualType, expectedType))
        {
            
        }
        public UnknownDerivedTypeNetSchemaException(object? actualType, Type expectedType) 
            : this(actualType?.GetType() ?? typeof(object), expectedType)
        {
            
        }

        private static string CreateMessage(Type actualType, Type expectedType) 
            => string.Format(MESSAGE, actualType.FullName ?? actualType.Name, expectedType.FullName ?? expectedType.Name);
    }
}