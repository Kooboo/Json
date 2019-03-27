using JsonValidatorTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class IgnoreKeysFeatureTest
    {
        class Member
        {
            public string Plain { get; set; }

            public string RealName { get; set; }

        }

        [TestMethod]
        public void IgnoreKeysFeature_use_option_should_be_work_correct()
        {
            var json = JsonSerializer.ToJson(
                new Member
                {
                    Plain = "hello world",
                    RealName = "Really RealName"
                }, new JsonSerializerOption() { IgnoreKeys = new List<string>() { "RealName" } });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Plain\":\"hello world\"}", json);
        }

        class SkipMember
        {
            public string Plain { get; set; }

            [IgnoreKey]
            public string RealName { get; set; }

        }

        [TestMethod]
        public void IgnoreKeysFeature_use_attribute_should_be_work_correct()
        {
            var json = JsonSerializer.ToJson(
                new SkipMember
                {
                    Plain = "hello world",
                    RealName = "Really RealName"
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Plain\":\"hello world\"}", json);
        }
    }
}