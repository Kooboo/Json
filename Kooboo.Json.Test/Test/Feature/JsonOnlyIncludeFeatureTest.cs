using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class JsonOnlyIncludeFeatureTest
    {
        class G
        {
            [JsonOnlyInclude]
            public int a;
            [IgnoreKey]
            public int b;
            public int c;
        }

        [TestMethod]
        public void JsonOnlyIncludeFeature_should_be_work_correct()
        {
            G g = new G();
            var json = JsonSerializer.ToJson(g);
            //When JsonOnlyInclude exists, IgnoreKey will be useless
            Assert.AreEqual("{\"a\":0}", json);
        }
    }
}