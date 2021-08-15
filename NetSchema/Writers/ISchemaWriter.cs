using System;
using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Syntax;

namespace NetSchema.Writers
{
    public interface IWritable
    {
        IEnumerable<ISchemaDocument> WriteSchema(ISchemaWriter writer);
        IEnumerable<ISchemaDocumentItem> Modules { get; }
    }
    
    public interface ISchemaWritableChild
    {
        ISyntaxNode CreateSyntaxNode();
        StatementType StatementType { get; }
        string Argument { get; }
        IEnumerable<ISyntaxNode> Children { get; }
    }

    public interface ISchemaDocumentItem : ISchemaWritableChild
    {
        string Name { get; }
        string? RevisionDate { get; }
    }
    
    public interface ISchemaWriter : IDisposable
    {
        void OpenDocument(string name, string? revisionDate);
        ISchemaDocument CloseDocument();
        void OpenStatement(StatementType type, string argument);
        void CloseStatement(StatementType type, string argument);
        void WriteStatement(StatementType type, string argument);
    }
}