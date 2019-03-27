using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class IgnoreValueNullFeatureTest
    {
        class E
        {
            public int? a;
            public int b;
        }

        [TestMethod]
        public void IgnoreValueNullFeature_should_be_work_correct()
        {
            E e = new E();
            JsonSerializerOption option = new JsonSerializerOption
            {
                IsIgnoreValueNull = true
            };
            var json = JsonSerializer.ToJson(e, option);
            Assert.AreEqual("{\"b\":0}", json);
        }
    }
}