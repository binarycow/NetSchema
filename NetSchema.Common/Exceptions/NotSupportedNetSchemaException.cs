using System;
using System.ComponentModel;
using NetSchema.Common.Extensions;

namespace NetSchema.Common.Exceptions
{
    public enum NotSupportedFeature
    {
        [Description("Multiple modules")]
        MultipleModules,
        Extensions,
        SubModules
    }
    
    public class NotSupportedNetSchemaException : NetSchemaException
    {
        public const string MESSAGE = "Feature '{0}' is not yet supported";
        public NotSupportedNetSchemaException(NotSupportedFeature feature) : base(MakeMessage(feature))
        {
        }


        public NotSupportedNetSchemaException(NotSupportedFeature feature, Exception innerException) 
            : base(MakeMessage(feature), innerException)
        {
        }

        private static string MakeMessage(NotSupportedFeature feature)
            => string.Format(MESSAGE, feature.GetDescription());
    }
}