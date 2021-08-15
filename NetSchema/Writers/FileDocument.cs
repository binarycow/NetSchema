using System.IO;

namespace NetSchema.Writers
{
    internal class FileDocument : ISchemaDocument
    {
        public FileInfo File { get; }

        public FileDocument(FileInfo file)
        {
            this.File = file;
        }
    }
}