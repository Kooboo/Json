using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class GenericsFormatTest
    {
        public class Wrap<T>
        {
            public T Value { get; set; }
        }

        [TestMethod]
        public void Generics_serialize_should_be_correct_format()
        {
            var str = JsonSerializer.ToJson<Wrap<byte>>(null);
            Assert.IsTrue(JsonValidator.IsValid(str));
            Assert.AreEqual("null", str);

            str = JsonSerializer.ToJson<Wrap<sbyte>>(null);
            Assert.IsTrue(JsonValidator.IsValid(str));
            Assert.AreEqual("null", str);

            str = JsonSerializer.ToJson(new Wrap<int> { Value = -123 });
            Assert.IsTrue(JsonValidator.IsValid(str));
            Assert.AreEqual("{\"Value\":-123}", str);
        }

        [TestMethod]
        public void Values_in_an_array_should_not_be_the_same_type()
        {
            var list = new ArrayList {new Wrap<int> {Value = -123}, (Wrap<byte>) null};
            var json = JsonSerializer.ToJson(list);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"Value\":-123},null]", json);
        }
    }
}
