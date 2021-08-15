using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NetSchema.Common;

namespace NetSchema.Writers
{
    internal sealed class YangWriter : ISchemaWriter
    {
        private readonly DirectoryInfo directoryInfo;
        private FileStream? stream;
        private StreamWriter? textWriter;
        private IndentedTextWriter? writer;
        private FileInfo? file;

        public YangWriter(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo;
        }
        public void OpenDocument(string name, string? revisionDate)
        {
            this.textWriter?.Dispose();
            this.writer?.Dispose();
            this.stream?.Dispose();
            this.file = new (GetPath(name, revisionDate));
            this.stream = file.OpenWrite();
            this.textWriter = new (this.stream);
            this.writer = new (this.textWriter);
        }

        private string GetPath(string name, string? revisionDate)
        {
            var filename = revisionDate is null ? $"{name}.yang" : $"{name}@{revisionDate}.yang";
            return Path.Combine(this.directoryInfo.FullName, filename);
        }

        public ISchemaDocument CloseDocument()
        {            
            this.textWriter?.Dispose();
            this.writer?.Dispose();
            this.stream?.Dispose();
            this.textWriter = null;
            this.writer = null;
            this.stream = null;
            return new FileDocument(CheckNull(this.file));
        }

        private void WriteStatement(StatementType type, string argument, bool hasChildren)
        {
            var w = this.CheckNull(this.writer);
            w.Write(GetStatementName(type));
            if (argument.IsNotNullOrWhiteSpace())
            {
                w.Write(" ");
                w.Write(argument);
            }
            if (hasChildren)
            {
                w.WriteLine(" {");
                ++w.Indent;
            }
            else
            {
                w.WriteLine(";");
            }
        }

        private string GetStatementName(StatementType type)
        {
            return type switch
            {
                StatementType.Action => "action",
                StatementType.Anydata => "anydata",
                StatementType.Anyxml => "anyxml",
                StatementType.Argument => "argument",
                StatementType.Augment => "augment",
                StatementType.Base => "base",
                StatementType.BelongsTo => "belongs-to",
                StatementType.Bit => "bit",
                StatementType.Case => "case",
                StatementType.Choice => "choice",
                StatementType.Config => "config",
                StatementType.Contact => "contact",
                StatementType.Container => "container",
                StatementType.Default => "default",
                StatementType.Description => "description",
                StatementType.Deviate => "deviate",
                StatementType.Deviation => "deviation",
                StatementType.Enum => "enum",
                StatementType.ErrorAppTag => "error-app-tag",
                StatementType.ErrorMessage => "error-message",
                StatementType.Extension => "extension",
                StatementType.Feature => "feature",
                StatementType.FractionDigits => "fraction-digits",
                StatementType.Grouping => "grouping",
                StatementType.Identity => "identity",
                StatementType.IfFeature => "if-feature",
                StatementType.Import => "import",
                StatementType.Include => "include",
                StatementType.Input => "input",
                StatementType.Key => "key",
                StatementType.Leaf => "leaf",
                StatementType.LeafList => "leaf-list",
                StatementType.Length => "length",
                StatementType.List => "list",
                StatementType.Mandatory => "mandatory",
                StatementType.MaxElements => "max-elements",
                StatementType.MinElements => "min-elements",
                StatementType.Modifier => "modifier",
                StatementType.Module => "module",
                StatementType.Must => "must",
                StatementType.Namespace => "namespace",
                StatementType.Notification => "notification",
                StatementType.OrderedBy => "ordered-by",
                StatementType.Organization => "organization",
                StatementType.Output => "output",
                StatementType.Path => "path",
                StatementType.Pattern => "pattern",
                StatementType.Position => "position",
                StatementType.Prefix => "prefix",
                StatementType.Presence => "presence",
                StatementType.Range => "range",
                StatementType.Reference => "reference",
                StatementType.Refine => "refine",
                StatementType.RequireInstance => "require-instance",
                StatementType.Revision => "revision",
                StatementType.RevisionDate => "revision-date",
                StatementType.Rpc => "rpc",
                StatementType.Status => "status",
                StatementType.Submodule => "submodule",
                StatementType.Type => "type",
                StatementType.Typedef => "typedef",
                StatementType.Unique => "unique",
                StatementType.Units => "units",
                StatementType.Uses => "uses",
                StatementType.Value => "value",
                StatementType.When => "when",
                StatementType.YangVersion => "yang-version",
                StatementType.YinElement => "yin-element",
                _ => throw new NotImplementedException()
            };
        }

        public void OpenStatement(StatementType type, string argument) => WriteStatement(type, argument, true);

        public void CloseStatement(StatementType type, string argument)
        {
            var w = this.CheckNull(this.writer);
            --w.Indent;
            w.WriteLine("}");
        }


        private T CheckNull<T>(T? item) => item ?? throw new InvalidOperationException("Document has not been opened yet.");
        
        public void WriteStatement(StatementType type, string argument) => WriteStatement(type, argument, false);

        public void Dispose()
        {
            this.stream?.Dispose();
            this.writer?.Dispose();
            this.textWriter?.Dispose();
        }
    }
}