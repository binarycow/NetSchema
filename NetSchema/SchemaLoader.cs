using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Exceptions;
using NetSchema.Syntax;

namespace NetSchema
{
    public class SchemaLoader
    {
        public static ISyntaxSchema LoadSyntaxSchema(SchemaSettings settings) => new SchemaLoader(settings).LoadSyntaxSchema();

        private static readonly Regex filenameRegex = new Regex(
            @"(?<name>[A-Za-z_][A-Za-z0-9_.-]*)(@(?<date>\d{4}-\d{2}-\d{2}))?\.(?<ext>yang|yin)",
            RegexOptions.ExplicitCapture
        );
        private readonly SchemaSettings settings;
        internal SchemaLoader(SchemaSettings settings)
        {
            this.settings = settings;
        }

        private ISyntaxSchema LoadSyntaxSchema()
        {
            var modules = new List<ISyntaxNode>();
            var names = new HashSet<string>();
            var namespaces = new HashSet<string>();
            foreach (var module in this.settings.ModuleNames)
            {
                if (!TryFindFile(module, out var syntaxNode))
                    throw new NotImplementedException();
                // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                var validatedNode = syntaxNode.Type switch
                {
                    StatementType.Module when ValidateModule(syntaxNode, names, namespaces) => syntaxNode,
                    StatementType.Module => null,
                    StatementType.Submodule => throw new NotSupportedNetSchemaException(NotSupportedFeature.SubModules),
                    _ => throw new GenericNetSchemaException($"Invalid top-level statement type {syntaxNode.Type}"),
                };
                if (validatedNode is not null)
                    modules.Add(validatedNode);
            }

            return new SyntaxSchema(modules);
            
            static bool ValidateModule(ISyntaxNode syntaxNode, HashSet<string> names, HashSet<string> namespaces)
            {
                if(syntaxNode.GetChildren(StatementType.Import).Any())
                    throw new NotSupportedNetSchemaException(NotSupportedFeature.MultipleModules);
                if(syntaxNode.GetChildren(StatementType.Include).Any())
                    throw new NotSupportedNetSchemaException(NotSupportedFeature.SubModules);
                var namespaceName = syntaxNode.GetChildArgument(StatementType.Namespace)
                    ?? throw new GenericNetSchemaException($"module statement is missing a namespace value.");
                var moduleName = syntaxNode.Argument;
                if(names.Add(moduleName) == false) throw new GenericNetSchemaException($"module name {moduleName} is already in use.");
                if(namespaces.Add(namespaceName) == false) throw new GenericNetSchemaException($"namespace {namespaceName} is already in use.");
                return true;
            }
        }

        #region Get syntax nodes
        private IEnumerable<FileInfo> GetCandidates(string name, string? revisionDate)
        {
            var searchOption = this.settings.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var allFiles = this.settings.SearchDirectory.EnumerateFiles("*", searchOption);
            foreach (var file in allFiles)
            {
                if (!SchemaLoader.filenameRegex.TryMatch(file.Name, out var match))
                    continue;
                var fileRevDate = match.Groups["date"]?.Value;
                var fileName = match.Groups["name"]?.Value;
                if(revisionDate is not null && fileRevDate != revisionDate)
                    continue;
                if(fileName != name)
                    continue;
                yield return file;
            }
        }
        
        private bool TryFindFile(string module, [NotNullWhen(true)] out ISyntaxNode? syntaxNode) 
            => TryFindFile(module, null, out syntaxNode);

        private bool TryFindFile(
            string name, 
            string? revisionDate, 
            [NotNullWhen(true)] out ISyntaxNode? syntaxNode
        )
        {
            syntaxNode = default;
            foreach (var file in GetCandidates(name, revisionDate))
            {
                switch (file.Extension)
                {
                    case ".yin" when YinSyntaxNodeFactory.TryGetSyntaxNode(file, out syntaxNode):
                        return true;
                    case ".yang" when YangSyntaxNodeFactory.TryGetSyntaxNode(file, out syntaxNode):
                        return true;
                }
            }
            return false;
        }
        #endregion Get syntax nodes
    }

}