using System;
using System.IO;
using System.Xml.Linq;
using NetSchema;
using NetSchema.Resolve.Nodes;
using NetSchema.Syntax;

namespace Demo
{
    public static class DataSerializerTest
    {
        public static readonly XName WrapperElementName = Program.Netconf + "data";
        public static void TestXml(ISyntaxSchema schema)
        {
            var resolved = schema.Resolve();
            var doc = XDocument.Load(Program.MakePath("Data", "net-device.xml"));
            if (!resolved.DeserializeDataTreeXml(doc, WrapperElementName).Try(out var dataTree, out var error))
            {
                Console.WriteLine(error.ToString());
                return;
            }
            dataTree.ToXml(WrapperElementName).Save(Program.MakePath("Data", "net-device_OUT.xml"));
        }

        public static void TestJson(ISyntaxSchema schema)
        {
            var resolved = schema.Resolve();
            var doc = File.ReadAllText(Program.MakePath("Data", "net-device.json"));
            if (!resolved.DeserializeDataTreeJson(doc).Try(out var dataTree, out var error))
            {
                Console.WriteLine(error.ToString());
                return;
            }
            File.WriteAllText(Program.MakePath("Data", "net-device_OUT.json"), dataTree.ToJson());
        }
    }
}