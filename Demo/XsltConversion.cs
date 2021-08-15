using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Demo
{
    public static class XsltConversion
    {
    // 
        public static void Run(string inputXmlPath, string outputJsonPath)
        {
            XsltSettings sets = new (true, true);
            var resolver = new XmlUrlResolver();
            XslCompiledTransform xslt = new ();
            xslt.Load(Program.MakePath("Transforms", "net-device.xsl"), sets, resolver);
            xslt.Transform(inputXmlPath, outputJsonPath);
        }
    }
}