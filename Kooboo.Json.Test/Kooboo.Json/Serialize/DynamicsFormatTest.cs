using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class DynamicsFormatTest
    {
        [TestMethod]
        public void Dynamic_serialize_should_be_correct_formatt()
        {
            dynamic obj = new ExpandoObject();
            var json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{}", json);

            obj.Name = "zz";
            obj.Age = 10;
            json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"zz\",\"Age\":10}", json);

            json = JsonSerializer.ToJson(new
            {
                A = 1,
                B = (int?)null,
                C = "hello world"
            });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"A\":1,\"B\":null,\"C\":\"hello world\"}", json);
        }
    }
}
