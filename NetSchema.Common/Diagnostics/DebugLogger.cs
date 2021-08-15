using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace NetSchema.Common.Diagnostics
{
    internal class DebugLogger
    {
        private static ConsoleWriter? Writer;
        private static IndentedTextWriter? IndentWriter;

        private static IndentedTextWriter GetWriter()
        {
            Writer ??= new ();
            IndentWriter ??= new (Writer);
            return IndentWriter;
        }
        
        [Conditional("DEBUG")]
        public static void Indent() => ++GetWriter().Indent;
        
        [Conditional("DEBUG")]
        public static void Dedent() => --GetWriter().Indent;
        
        [Conditional("DEBUG")]
        public static void WriteLine(
            string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0
        )
        {
            message = $"{Path.GetFileName(filePath)} ({lineNumber.ToString()}): {memberName}: {message}";
            GetWriter().WriteLine(message);
        }
    }
}