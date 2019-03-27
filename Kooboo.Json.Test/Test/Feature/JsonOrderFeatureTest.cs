using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class JsonOrderFeatureTest
    {
        class F
        {
            public int a;
            public int b;
            public int c;
        }

        class F_JsonOrder
        {
            [JsonOrder(3)]
            public int a;
            [JsonOrder(2)]
            public int b;
            [JsonOrder(1)]
            public int c;
        }

        [TestMethod]
        public void JsonOrderFeature_should_be_work_correct()
        {
            {
                F f = new F();
                var json = JsonSerializer.ToJson(f);
                Assert.AreEqual("{\"a\":0,\"b\":0,\"c\":0}", json);
            }

            {
                F_JsonOrder f = new F_JsonOrder();
                var json = JsonSerializer.ToJson(f);
                Assert.AreEqual("{\"c\":0,\"b\":0,\"a\":0}", json);
            }
        }
    }
}