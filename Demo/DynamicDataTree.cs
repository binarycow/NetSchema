using System;
using NetSchema;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Syntax;

namespace Demo
{
    public static class DynamicDataTree
    {

        public static void Test(ISyntaxSchema schema)
        {
            var resolved = schema.Resolve();
            TestDynamic(resolved.CreateDataTree());
        }
        
        private static void TestDynamic(IDataTree dataObject)
        {
            if (!dataObject.TryFindOrCreate(new ("net-device", "device"), out var node))
                throw new NotImplementedException();
            Dynamic(node.ToDynamic());
            Console.WriteLine(dataObject.ToJson());
        }
        
        private static void Dynamic(dynamic dataObject)
        {
            dataObject.hostname = "my-device";
            dataObject["mgmt-ip"] = "10.25.0.1";
            dataObject["spanning-tree"].mst["region-name"] = "mst-region-1";
            dataObject["non-keyed-list"][0].foobar["sub-item"] = "Test";
            dataObject["non-keyed-list"][0]["test-item"] = "Alpha";
            dataObject["non-keyed-list"][1]["test-item"] = "Bravo";
            dataObject.interfaces.@interface[name: "GigabitEthernet1/0/1"].enabled = true;
            dataObject.interfaces.@interface[name: "GigabitEthernet1/0/2"].enabled = false;
            dataObject["dns-server"] = new[] { "8.8.8.8", "1.1.1.1", };
        }
    }
}