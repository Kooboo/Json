using JsonValidatorTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class AliasFeatureTest
    {
        class DataMemberName
        {
            public string Plain { get; set; }

            [Alias("FakeName")]
            public string RealName { get; set; }

            [Alias("NotSoSecretName")]
            public int SecretName { get; set; }
        }

        [TestMethod]
        public void AliasName_in_object_should_be_serialize_to_correct_format()
        {
            var json = JsonSerializer.ToJson(
                new DataMemberName
                {
                    Plain = "hello world",
                    RealName = "Really RealName",
                    SecretName = 314159
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Plain\":\"hello world\",\"FakeName\":\"Really RealName\",\"NotSoSecretName\":314159}", json);
        }
    }
}