using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NetSchema;
using NetSchema.Common;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Resolve;
using NetSchema.Resolve.Nodes;
using NetSchema.Restrictions;
using NetSchema.Syntax;

namespace Demo
{
    internal static class Program
    {
        public static string MakePath(string dir, string filename) 
            => Path.Combine(RootDirectory.FullName, dir, filename);
        public static string MakePath(string dir) 
            => Path.Combine(RootDirectory.FullName, dir);

        public static readonly DirectoryInfo RootDirectory = new (@"C:\Users\mikec\RiderProjects\NetSchema-final\Data");
        public static readonly DirectoryInfo SchemaDirectory = new (MakePath("Schema"));
        
        public const string NETCONF = "urn:ietf:params:xml:ns:netconf:base:1.0";
        public static readonly XNamespace Netconf = Program.NETCONF;
        
        public static void Main()
        {
            XsltConversion.Run(
                inputXmlPath: MakePath("Data", "net-device.xml"), 
                outputJsonPath: MakePath("Data", "net-device.json")
            );
            
            var schema = LoadSchema();

            DynamicDataTree.Test(schema);

            DataSerializerTest.TestJson(schema);
            DataSerializerTest.TestXml(schema);
            Console.WriteLine("Done");
        }


        private static ISyntaxSchema LoadSchema()
        {
            return SchemaLoader.LoadSyntaxSchema(SchemaSettings.Default with
            {
                SearchDirectory = SchemaDirectory,
                ModuleNames = new[]
                {
                    "net-device",
                },
            });
        }

        private static IResolvedSchema ResolveSchema(ISyntaxSchema schema) => schema.Resolve();
        
        

    }
}