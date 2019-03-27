using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class CollectionsFormatTest
    {
        [TestMethod]
        public void NameValueCollection_serialize_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson((NameValueCollection)null);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("null", json);

            var obj = new NameValueCollection();
            json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{}", json);

            obj["Name"] = "zz";
            obj["Age"] = "10";
            json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"zz\",\"Age\":\"10\"}", json);
        }

        public class _List
        {
            public string Key;
            public int Val;
        }

        [TestMethod]
        public void ObjectList_serialize_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson(
                new[]
                {
                    new _List { Key = "whatever", Val = 123 },
                    new _List { Key = "indeed", Val = 456 }
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("[{\"Val\":123,\"Key\":\"whatever\"},{\"Val\":456,\"Key\":\"indeed\"}]", json);
        }

        [TestMethod]
        public void IReadOnlyList_serialize_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson(
                (IReadOnlyList<_List>)new[]
                {
                    new _List { Key = "whatever", Val = 123 },
                    new _List { Key = "indeed", Val = 456 }
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"Key\":\"whatever\",\"Val\":123},{\"Key\":\"indeed\",\"Val\":456}]", json);
        }

        public class InnerLists
        {
            public class _WithList
            {
                public List<int> List;
            }

            public _WithList WithList;
        }

        [TestMethod]
        public void InnerLists_serialize_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson(
                new InnerLists
                {
                    WithList = new InnerLists._WithList
                    {
                        List = new List<int> { 1, 2, 3 }
                    }
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"WithList\":{\"List\":[1,2,3]}}", json);
        }
    }
}
