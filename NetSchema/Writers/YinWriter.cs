using System.IO;
using NetSchema.Common;

namespace NetSchema.Writers
{
    internal sealed class YinWriter : ISchemaWriter
    {
        public YinWriter(DirectoryInfo directory)
        {
            throw new System.NotImplementedException();
        }

        public void OpenDocument(string name, string? revisionDate)
        {
            throw new System.NotImplementedException();
        }

        public ISchemaDocument CloseDocument() => throw new System.NotImplementedException();

        public void OpenStatement(StatementType type, string argument)
        {
            throw new System.NotImplementedException();
        }

        public void CloseStatement(StatementType type, string argument)
        {
            throw new System.NotImplementedException();
        }

        public void WriteStatement(StatementType type, string argument)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}