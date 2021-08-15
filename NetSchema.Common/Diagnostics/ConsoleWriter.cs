using System;
using System.IO;
using System.Text;

namespace NetSchema.Common.Diagnostics
{
    internal class ConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value) => Console.Write(value);
        public override void Write(string value) => Console.Write(value);
        public override void WriteLine(char value) => Console.WriteLine(value);
        public override void WriteLine(string value) => Console.WriteLine(value);
    }
}