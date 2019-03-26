using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class SecurityIssueTest
    {
        private class Window
        {
            public string onclick { get; set; }
        }

        private class Root
        {
            public Window window { get; set; }
        }

        [TestMethod]
        public void Json_serialize_should_be_consider_security()
        {
            //Security Considerations
            //front-end interactive: system variable needs avoid(eg, window)
            var obj = new Root { window = new Window { onclick = "alert(123)" } };
            var json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"window\":{\"onclick\":\"alert(123)\"}}", json);
            var json1 = json;
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                StringAssert.DoesNotMatch(json1, new Regex("\"window\":"));
            });
            //directly generates executable code
            var str = "alert('hello world')";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"alert('hello world')\"", json);
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                StringAssert.DoesNotMatch(json, new Regex("alert[(](.*?)[)]"));
            });
        }
    }
}
