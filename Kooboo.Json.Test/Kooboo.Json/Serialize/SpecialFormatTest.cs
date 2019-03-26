using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class SpecialFormatTest
    {
        [TestMethod]
        public void Guid_serialize_should_be_correct_format()
        {
            var g = Guid.Parse("edabd1c6-bc0f-497f-a0f3-5298fda179dd");
            var json = JsonSerializer.ToJson(g);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"edabd1c6-bc0f-497f-a0f3-5298fda179dd\"", json);

            var guidLists = new List<Guid> { new Guid("DE01D5B0-069B-47EE-BFF2-8A1C10A32FCD"), new Guid("DE01D5B0-069B-47EE-BFF2-8A1C10A32FCC"), new Guid("DE01D5B0-069B-47EE-BFF2-8A1C10A32FCB") };
            json = JsonSerializer.ToJson(guidLists);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[\"de01d5b0-069b-47ee-bff2-8a1c10a32fcd\",\"de01d5b0-069b-47ee-bff2-8a1c10a32fcc\",\"de01d5b0-069b-47ee-bff2-8a1c10a32fcb\"]", json);

            var guidDict = new Dictionary<string, Guid> { { "hello", new Guid("DE01D5B0-069B-47EE-BFF2-8A1C10A32FCD") }, { "world", new Guid("DE01D5B0-069B-47EE-BFF2-8A1C10A32FCB") } };
            json = JsonSerializer.ToJson(guidDict);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"hello\":\"de01d5b0-069b-47ee-bff2-8a1c10a32fcd\",\"world\":\"de01d5b0-069b-47ee-bff2-8a1c10a32fcb\"}", json);
        }

        [TestMethod]
        public void Uri_serialize_should_be_correct_format()
        {
            {
                Uri u = new Uri("https://kooboo.cn");
                string json = JsonSerializer.ToJson(u);
                Assert.IsTrue(JsonValidator.IsValid(json));
                Assert.AreEqual("\"https://kooboo.cn\"", json);
            }

            {
                Uri u = new Uri("https://www.google.com/search?newwindow=1&ei=E-87XLP7LMTdhwOw16q4BQ&q=google&oq=google&gs_l=psy-ab.3..0i67l10.5398035.5398714..5398905...0.0..0.232.556.1j2j1......0....1..gws-wiz.......0.7Sb7ctt5_Cc");
                string json = JsonSerializer.ToJson(u);
                Assert.IsTrue(JsonValidator.IsValid(json));
                Assert.AreEqual("\"https://www.google.com/search?newwindow=1&ei=E-87XLP7LMTdhwOw16q4BQ&q=google&oq=google&gs_l=psy-ab.3..0i67l10.5398035.5398714..5398905...0.0..0.232.556.1j2j1......0....1..gws-wiz.......0.7Sb7ctt5_Cc\"", json);
            }
        }

        class IssueException
        {
            public DateTime DateField { get; set; }
            public int IntField { get; set; }
            public Dictionary<string, object> DictField { get; set; }
            public Exception ExceptionField { get; set; }

        }

        [TestMethod]
        public void Exception_serialize_should_be_correct_format()
        {
            Exception e = null;
            try
            {
                var x = int.Parse("330") / int.Parse("0");
            }
            catch (Exception _) { e = _; }

            Assert.IsNotNull(e);

            var obj =
                new IssueException
                {
                    DateField = DateTime.UtcNow,
                    IntField = 123,
                    DictField = new Dictionary<string, object> { { "foo", "bar" } },
                    ExceptionField = e
                };

            var str = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(str));
            Assert.IsNotNull(str);
        }

        [TestMethod]
        public void Linq_serialize_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson(Enumerable.Range(0, 5000).Select(i => i * 1.1m));
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.IsNotNull(json);
        }

        class VersionClass
        {
            public VersionClass(string version1, string version2)
            {
                StringProperty1 = "StringProperty1";
                Version1 = new Version(version1);
                Version2 = new Version(version2);
                StringProperty2 = "StringProperty2";
            }

            public VersionClass()
            {
            }

            public string StringProperty1 { get; set; }
            public Version Version1 { get; set; }
            public Version Version2 { get; set; }
            public string StringProperty2 { get; set; }
        }

        [TestMethod]
        public void Version_serialize_should_be_correct_format()
        {
            VersionClass versionClass = new VersionClass("1.0.0.0", "2.0.0.0");
            string json = JsonSerializer.ToJson(versionClass);
            string expectedJson = "{\"StringProperty1\":\"StringProperty1\",\"Version1\":{\"Major\":1,\"Minor\":0,\"Build\":0,\"Revision\":0,\"MajorRevision\":0,\"MinorRevision\":0},\"Version2\":{\"Major\":2,\"Minor\":0,\"Build\":0,\"Revision\":0,\"MajorRevision\":0,\"MinorRevision\":0},\"StringProperty2\":\"StringProperty2\"}";
            Assert.AreEqual(expectedJson, json);
        }
    }
}
