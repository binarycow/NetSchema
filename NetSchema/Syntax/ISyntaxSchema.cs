using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;

namespace NetSchema.Syntax
{
    public interface ISyntaxSchema
    {
        IReadOnlyModuleDictionary<ISyntaxNode> Modules { get; }
    }

    internal class SyntaxSchema : ISyntaxSchema
    {
        public SyntaxSchema(IEnumerable<ISyntaxNode> modules)
        {
            Modules = new ModuleDictionary(modules);
        }

        public IReadOnlyModuleDictionary<ISyntaxNode> Modules { get; }




        private class ModuleDictionary : MutableModuleDictionary<ISyntaxNode>
        {
            public ModuleDictionary(IEnumerable<ISyntaxNode> modules)
            {
                base.AddRange(modules);
            }

            private string Validate(ISyntaxNode module, StatementType arg, string name)
            {
                if (module.Type != StatementType.Module)
                    throw new InvalidOperationException("Syntax node is not a module.");
                return module.GetChildArgument(arg) 
                    ?? throw new InvalidOperationException($"Module does not have a {name} statement.");
            }

            protected override XNamespace GetNamespaceFromModule(ISyntaxNode module)
                => Validate(module, StatementType.Namespace, "namespace");
            protected override string GetNameFromModule(ISyntaxNode module) 
                => module.Argument;
        }
    }
}