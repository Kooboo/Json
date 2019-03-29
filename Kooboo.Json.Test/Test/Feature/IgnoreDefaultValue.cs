using JsonValidatorTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class IgnoreDefaultValue
    {
        [IgnoreDefaultValue]
        class User
        {
            public int? Num { get; set; }

            public int Age { get; set; }

            public string Name { get; set; }

        }
        [TestMethod]
        public void IgnoreDefaultValueFeature_use_classAttribute_should_be_work_correct()
        {
            var json = JsonSerializer.ToJson(new User());
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{}", json);
        }

        class User2
        {
            [IgnoreDefaultValue]
            public int? Num { get; set; }

            public int Age { get; set; }

            public string Name { get; set; }
        }

        [TestMethod]
        public void IgnoreDefaultValueFeature_use_attribute_should_be_work_correct()
        {
            var json = JsonSerializer.ToJson(new User2());
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Age\":0,\"Name\":null}", json);
        }

    }
}