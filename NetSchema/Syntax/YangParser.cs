using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NetSchema.Common;

namespace NetSchema.Syntax
{
    internal static class YangParser
    {
        public static ISyntaxNode Parse(ref TokenEnumerator tokens)
        {
            if(tokens.MoveNext() == false)
                throw new System.NotImplementedException();
            if(!TryParse(ref tokens, out var node))
                throw new System.NotImplementedException();
            if(tokens.Current.Type != YangTokenType.EndOfFile)
                throw new System.NotImplementedException();
            return node;
        }

        private static bool TryParse(
            ref TokenEnumerator enumerator, 
            [NotNullWhen(true)] out MemorySyntaxNode? node
        )
        {
            ConsumeTrivia(ref enumerator);
            if (enumerator.Current.Type == YangTokenType.CloseBrace)
            {
                node = default;
                return false;
            }
            if(!TryParseStatementType(ref enumerator, out var type))
                throw new System.NotImplementedException();
            SkipPastSeparatorAndConsumeWhiteSpace(ref enumerator);
            var arg = ParseArgument(ref enumerator);
            enumerator.MoveNext();
            ConsumeTrivia(ref enumerator);
            if (enumerator.Current.Type == YangTokenType.Semicolon)
            {
                node = new MemorySyntaxNode(type, arg, Enumerable.Empty<MemorySyntaxNode>());
                enumerator.MoveNext();
                ConsumeTrivia(ref enumerator);
                return true;
            }
            if(enumerator.Current.Type != YangTokenType.OpenBrace)
                throw new NotImplementedException();
            enumerator.MoveNext();
            ConsumeTrivia(ref enumerator);
            var children = new List<MemorySyntaxNode>();
            while (TryParse(ref enumerator, out var child))
            {
                children.Add(child);
            }
            if(enumerator.Current.Type != YangTokenType.CloseBrace)
                throw new NotImplementedException();
            node = new (type, arg, children);
            enumerator.MoveNext();
            ConsumeTrivia(ref enumerator);
            return true;
        }

        private static string ParseArgument(ref TokenEnumerator enumerator) => enumerator.Current.Type switch
        {
            YangTokenType.SingleQuotedString => UnquoteString(enumerator.GetCurrentString()),
            YangTokenType.DoubleQuotedString => UnquoteString(enumerator.GetCurrentString()),
            YangTokenType.UnquotedString => enumerator.GetCurrentString(),
            YangTokenType.Semicolon => string.Empty,
            YangTokenType.OpenBrace => string.Empty,
            _ => throw new NotImplementedException(),
        };

        private static bool TryParseStatementType(ref TokenEnumerator enumerator, out StatementType type)
        {
            ConsumeTrivia(ref enumerator);
            if(!TryParsePrefixedName(ref enumerator, out var name))
                throw new System.NotImplementedException();
            if(!TryGetStatementType(name, out type))
                throw new System.NotImplementedException();
            if(type == StatementType.ExtensionUsage)
                throw new System.NotImplementedException();
            return true;
        }

        private static bool TryGetStatementType(PrefixedName name, out StatementType type)
        {
            if (name.Prefix is not null)
            {
                type = StatementType.ExtensionUsage;
                return true;
            }
            type = name.Name switch
            {
                "action" => StatementType.Action,
                "anydata" => StatementType.Anydata,
                "anyxml" => StatementType.Anyxml,
                "argument" => StatementType.Argument,
                "augment" => StatementType.Augment,
                "base" => StatementType.Base,
                "belongs-to" => StatementType.BelongsTo,
                "bit" => StatementType.Bit,
                "case" => StatementType.Case,
                "choice" => StatementType.Choice,
                "config" => StatementType.Config,
                "contact" => StatementType.Contact,
                "container" => StatementType.Container,
                "default" => StatementType.Default,
                "description" => StatementType.Description,
                "deviate" => StatementType.Deviate,
                "deviation" => StatementType.Deviation,
                "enum" => StatementType.Enum,
                "error-app-tag" => StatementType.ErrorAppTag,
                "error-message" => StatementType.ErrorMessage,
                "extension" => StatementType.Extension,
                "feature" => StatementType.Feature,
                "fraction-digits" => StatementType.FractionDigits,
                "grouping" => StatementType.Grouping,
                "identity" => StatementType.Identity,
                "if-feature" => StatementType.IfFeature,
                "import" => StatementType.Import,
                "include" => StatementType.Include,
                "input" => StatementType.Input,
                "key" => StatementType.Key,
                "leaf" => StatementType.Leaf,
                "leaf-list" => StatementType.LeafList,
                "length" => StatementType.Length,
                "list" => StatementType.List,
                "mandatory" => StatementType.Mandatory,
                "max-elements" => StatementType.MaxElements,
                "min-elements" => StatementType.MinElements,
                "modifier" => StatementType.Modifier,
                "module" => StatementType.Module,
                "must" => StatementType.Must,
                "namespace" => StatementType.Namespace,
                "notification" => StatementType.Notification,
                "ordered-by" => StatementType.OrderedBy,
                "organization" => StatementType.Organization,
                "output" => StatementType.Output,
                "path" => StatementType.Path,
                "pattern" => StatementType.Pattern,
                "position" => StatementType.Position,
                "prefix" => StatementType.Prefix,
                "presence" => StatementType.Presence,
                "range" => StatementType.Range,
                "reference" => StatementType.Reference,
                "refine" => StatementType.Refine,
                "require-instance" => StatementType.RequireInstance,
                "revision" => StatementType.Revision,
                "revision-date" => StatementType.RevisionDate,
                "rpc" => StatementType.Rpc,
                "status" => StatementType.Status,
                "submodule" => StatementType.Submodule,
                "type" => StatementType.Type,
                "typedef" => StatementType.Typedef,
                "unique" => StatementType.Unique,
                "units" => StatementType.Units,
                "uses" => StatementType.Uses,
                "value" => StatementType.Value,
                "when" => StatementType.When,
                "yang-version" => StatementType.YangVersion,
                "yin-element" => StatementType.YinElement,
                _ => throw new NotImplementedException(),
            };
            return type != StatementType.Unknown;
        }

        private static bool TryParsePrefixedName(
            ref TokenEnumerator enumerator, 
            out PrefixedName name
        )
        {
            if(IsString(enumerator.Current.Type) == false)
                throw new NotImplementedException();
            var str = UnquoteString(enumerator.GetCurrentString());
            return PrefixedName.TryParse(str, out name);
        }

        private static string UnquoteString(string value) 
            => (value.Length == 0 ? '\0' : value[0]) switch
        {
            '\'' => UnquoteSingle(value),
            '\"' => UnquoteDouble(value),
            _ => value
        };

        private static string UnquoteDouble(string value)
        {
            var sb = new StringBuilder();
            for (var i = 1; i < value.Length - 1; ++i)
            {
                if (value[i] != '\\')
                {
                    sb.Append(value[i]);
                    continue;
                }
                throw new NotImplementedException();
            }
            return sb.ToString();
        }

        private static string UnquoteSingle(string value) => value.Substring(1, value.Length - 2);

        private static bool IsString(YangTokenType type) => type switch
        {
            YangTokenType.UnquotedString => true,
            YangTokenType.SingleQuotedString => true,
            YangTokenType.DoubleQuotedString => true,
            _ => false,
        };

        private static void ConsumeTrivia(ref TokenEnumerator enumerator)
        {
            while (IsSkippable(enumerator.Current.Type) && enumerator.MoveNext())
            {
                
            }
        }
        private static void SkipPastSeparatorAndConsumeWhiteSpace(ref TokenEnumerator enumerator)
        {
            if (enumerator.MoveNext() == false)
                return;
            if (enumerator.Current.Type != YangTokenType.WhiteSpace)
                throw new NotImplementedException();
            while (IsSkippable(enumerator.Current.Type) && enumerator.MoveNext())
            {
                
            }
        }

        private static bool IsSkippable(YangTokenType token) => token switch
        {
            YangTokenType.WhiteSpace => true,
            YangTokenType.Comment => true,
            _ => false,
        };
    }
}