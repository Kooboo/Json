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
        public void Serialize_ignoreKeysFeature_use_option_should_be_work_correct()
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

        [TestMethod]
        public void Deserialize_isIgnoreExtraKeysInJSON_use_option_should_be_work_correct()
        {
            string json = "{\"Plain\":\"hello world\",\"Surplus\":3}";
            var obj = JsonSerializer.ToObject<Member>(json, new JsonDeserializeOption() { IsIgnoreExtraKeysInJSON = true });
            Assert.AreEqual("hello world", obj.Plain);
            Assert.AreEqual(null, obj.RealName);
        }

        [TestMethod]
        public void Deserialize_ignoreKeysFeature_use_option_should_be_work_correct()
        {
            string json = "{\"Plain\":\"hello world\",\"RealName\":\"a\"}";
            var obj = JsonSerializer.ToObject<Member>(json, new JsonDeserializeOption() { IgnoreJsonKeys = new HashSet<string>() { "RealName" } });
            Assert.AreEqual("hello world", obj.Plain);
            Assert.AreEqual(null, obj.RealName);
        }

        class SkipMember
        {
            public string Plain { get; set; }

            [IgnoreKey]
            public string RealName { get; set; }

        }

        [TestMethod]
        public void Serialize_IgnoreKeysFeature_use_attribute_should_be_work_correct()
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