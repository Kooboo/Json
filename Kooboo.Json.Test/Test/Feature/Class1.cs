using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Json.Test.Test.Feature
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void AliasName_in_object_shorect_format()
        {
            string json = "[1,2,3,456789999]";

            Stream s =new MemoryStream(Encoding.UTF8.GetBytes(json));
            StreamReader sr = new StreamReader(s);

            var c = JsonSerializer.ToObject(sr, typeof(int[]));
        }
    }
}
