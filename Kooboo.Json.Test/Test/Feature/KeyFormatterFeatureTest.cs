using JsonValidatorTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class KeyFormatterFeatureTest
    {
        class R01_User
        {
            public string R01_Name;
            public int R01_Age;
        }

        [TestMethod]
        public void KeyFormatterFeature_filter_key_should_be_correct()
        {
            JsonSerializerOption option1 = new JsonSerializerOption
            {
                GlobalKeyFormat = (key, parentType, handler) =>
                {
                    if (parentType == typeof(R01_User))
                    {
                        return key.Substring(4);
                    }

                    return key;
                }
            };

            var ru = new R01_User();
            var json = JsonSerializer.ToJson(ru, option1);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":null,\"Age\":0}", json);

            JsonDeserializeOption option2 = new JsonDeserializeOption
            {
                GlobalKeyFormat = (key, parentType) =>
                {
                    if (parentType == typeof(R01_User))
                    {
                        return "R01_" + key;
                    }

                    return key;
                }
            };

            var p = JsonSerializer.ToObject<R01_User>(json, option2);
            Assert.AreEqual(ru.R01_Age, p.R01_Age);
            Assert.AreEqual(ru.R01_Name, p.R01_Name);
        }
    }
}