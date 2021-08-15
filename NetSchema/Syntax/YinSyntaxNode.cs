using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Exceptions;

namespace NetSchema.Syntax
{
    internal class YinSyntaxNode : ISyntaxNode
    {
        private const string YIN = "urn:ietf:params:xml:ns:yang:yin:1";
        private static readonly XNamespace yin = YIN;
        private readonly IReadOnlyList<YinSyntaxNode> children;

        public YinSyntaxNode(ISyntaxNode? parent, XElement element)
        {
            Parent = parent;
            (Type, Argument, children) = GetArgAndChildren(this, element);
        }

        private static (StatementType Type, string Argument, IReadOnlyList<YinSyntaxNode> Children) GetArgAndChildren(
            YinSyntaxNode parent, 
            XElement element
        )
        {
            var type = GetStatementType(element.Name);
            var (isElement, argName) = GetArgInfo(type);
            var (arg, children) = isElement switch
            {
                { } when argName is null => (
                    Arg: string.Empty,
                    Children: element.Elements()
                ),
                true => (
                    Arg: element.Element(yin + argName)?.Value,
                    Children: element.Elements().Where(e => e.Name != yin + argName)
                ),
                false => (
                    Arg: element.Attribute(argName)?.Value,
                    Children: element.Elements()
                ),
            };
            return (
                type, 
                arg ?? string.Empty, 
                children.Select(c => new YinSyntaxNode(parent, c)).ToList().AsReadOnly()
            );
        }

        private static (bool IsElement, string? ArgName) GetArgInfo(StatementType type) => type switch
        {
            StatementType.Action => (false, "name"),
            StatementType.Anydata => (false, "name"),
            StatementType.Anyxml => (false, "name"),
            StatementType.Argument => (false, "name"),
            StatementType.Augment => (false, "target-node"),
            StatementType.Base => (false, "name"),
            StatementType.BelongsTo => (false, "module"),
            StatementType.Bit => (false, "name"),
            StatementType.Case => (false, "name"),
            StatementType.Choice => (false, "name"),
            StatementType.Config => (false, "value"),
            StatementType.Contact => (true, "text"),
            StatementType.Container => (false, "name"),
            StatementType.Default => (false, "value"),
            StatementType.Description => (true, "text"),
            StatementType.Deviate => (false, "value"),
            StatementType.Deviation => (false, "target-node"),
            StatementType.Enum => (false, "name"),
            StatementType.ErrorAppTag => (false, "value"),
            StatementType.ErrorMessage => (true, "value"),
            StatementType.Extension => (false, "name"),
            StatementType.Feature => (false, "name"),
            StatementType.FractionDigits => (false, "value"),
            StatementType.Grouping => (false, "name"),
            StatementType.Identity => (false, "name"),
            StatementType.IfFeature => (false, "name"),
            StatementType.Import => (false, "module"),
            StatementType.Include => (false, "module"),
            StatementType.Input => (default, null),
            StatementType.Key => (false, "value"),
            StatementType.Leaf => (false, "name"),
            StatementType.LeafList => (false, "name"),
            StatementType.Length => (false, "value"),
            StatementType.List => (false, "name"),
            StatementType.Mandatory => (false, "value"),
            StatementType.MaxElements => (false, "value"),
            StatementType.MinElements => (false, "value"),
            StatementType.Modifier => (false, "value"),
            StatementType.Module => (false, "name"),
            StatementType.Must => (false, "condition"),
            StatementType.Namespace => (false, "uri"),
            StatementType.Notification => (false, "name"),
            StatementType.OrderedBy => (false, "value"),
            StatementType.Organization => (true, "text"),
            StatementType.Output => (default, null),
            StatementType.Path => (false, "value"),
            StatementType.Pattern => (false, "value"),
            StatementType.Position => (false, "value"),
            StatementType.Prefix => (false, "value"),
            StatementType.Presence => (false, "value"),
            StatementType.Range => (false, "value"),
            StatementType.Reference => (true, "text"),
            StatementType.Refine => (false, "target-node"),
            StatementType.RequireInstance => (false, "value"),
            StatementType.Revision => (false, "date"),
            StatementType.RevisionDate => (false, "date"),
            StatementType.Rpc => (false, "name"),
            StatementType.Status => (false, "value"),
            StatementType.Submodule => (false, "name"),
            StatementType.Type => (false, "name"),
            StatementType.Typedef => (false, "name"),
            StatementType.Unique => (false, "tag"),
            StatementType.Units => (false, "name"),
            StatementType.Uses => (false, "name"),
            StatementType.Value => (false, "value"),
            StatementType.When => (false, "condition"),
            StatementType.YangVersion => (false, "value"),
            StatementType.YinElement => (false, "value"),
            StatementType.ExtensionUsage => throw new NotSupportedNetSchemaException(NotSupportedFeature.Extensions),
            _ => throw new ArgumentOutOfRangeException(),
        };

        private static StatementType GetStatementType(XName elementName) => elementName.LocalName switch
        {
            { } when elementName.Namespace != yin => StatementType.ExtensionUsage,
            "module" => StatementType.Module,
            "container" => StatementType.Container,
            "list" => StatementType.List,
            "leaf-list" => StatementType.LeafList,
            "leaf" => StatementType.Leaf,
            "namespace" => StatementType.Namespace,
            "prefix" => StatementType.Prefix,
            "type" => StatementType.Type,
            "config" => StatementType.Config,
            "key" => StatementType.Key,
            _ => throw new NotImplementedException(),
        };

        public ISyntaxNode? Parent { get; }
        public StatementType Type { get; }
        public string Argument { get; }
        public IEnumerable<ISyntaxNode> GetChildren() => this.children;
    }
}