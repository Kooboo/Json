using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class DictionaryFormatTest
    {
        [TestMethod]
        public void Dictionary_serialize_should_be_correct_format()
        {
            var data =
                new
                {
                    A =
                        new Dictionary<string, string>
                        {
                            {"hello", "world"},
                            {"fizz", null},
                            {"foo", "bar"},
                            {"init", "d"},
                            {"dev", null}
                        },
                    B = (IDictionary<string, string>)
                        new Dictionary<string, string>
                        {
                            {"hello", "world"},
                            {"fizz", null},
                            {"foo", "bar"},
                            {"init", "d"},
                            {"dev", null}
                        }
                };
            var json = JsonSerializer.ToJson(data);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"A\":{\"hello\":\"world\",\"fizz\":null,\"foo\":\"bar\",\"init\":\"d\",\"dev\":null},\"B\":{\"hello\":\"world\",\"fizz\":null,\"foo\":\"bar\",\"init\":\"d\",\"dev\":null}}", json);

            json = JsonSerializer.ToJson(
                new Dictionary<string, string>
                {
                    { "hello\nworld", "fizz\0buzz" },
                    { "\r\t\f\n", "\0\0\0\0\0\0\0\0\0\0" },
                    { "\0", "\b\b\b\b\b" }
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(@"{""hello\nworld"":""fizz\u0000buzz"",""\r\t\f\n"":""\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000"",""\u0000"":""\b\b\b\b\b""}", json);
        }

        [TestMethod]
        public void IReadOnlyDictionary_serialize_should_be_correct_format()
        {
            var res = JsonSerializer.ToJson(
                (IReadOnlyDictionary<string, int>) new Dictionary<string, int>
                {
                    {"hello world", 123},
                    {"fizz buzz", 456},
                    {"indeed", 789}
                }
            );
            Assert.IsTrue(JsonValidator.IsValid(res));
            Assert.AreEqual("{\"hello world\":123,\"fizz buzz\":456,\"indeed\":789}", res);
        }

        [TestMethod]
        public void DictionaryEncoding_serialize_should_be_correct_format()
        {
            var res = JsonSerializer.ToJson(
                new Dictionary<string, string>
                {
                    { "hello\nworld", "fizz\0buzz" },
                    { "\r\t\f\n", "\0\0\0\0\0\0\0\0\0\0" },
                    { "\0", "\b\b\b\b\b" }
                }
            );
            Assert.IsTrue(JsonValidator.IsValid(res));
            Assert.AreEqual(@"{""hello\nworld"":""fizz\u0000buzz"",""\r\t\f\n"":""\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000"",""\u0000"":""\b\b\b\b\b""}", res);
        }

        [TestMethod]
        public void TypeLimitDictionary_serialize_should_be_correct_format()
        {
            var res = JsonSerializer.ToJson(
                new Dictionary<short, string>
                {
                    { 1, "hello" },
                    { 2, "world" },
                    { 3, null },
                    { short.MinValue, "foo" },
                    { short.MaxValue, "bar" }
                }
            );
            Assert.IsTrue(JsonValidator.IsValid(res));
            Assert.AreEqual("{\"1\":\"hello\",\"2\":\"world\",\"3\":null,\"-32768\":\"foo\",\"32767\":\"bar\"}", res);
        }

        private class SimplePoco
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void NestedDictionary_serialize_should_be_correct_format()
        {
            var items = new Dictionary<string, Dictionary<string, SimplePoco>>
            {
                {
                    "a", new Dictionary<string, SimplePoco>
                    {
                        {"a", new SimplePoco {Id = 1, Name = "a"}},
                        {"b", new SimplePoco {Id = 2, Name = "b"}}
                    }
                }
            };
            var json = JsonSerializer.ToJson(items);
            Assert.IsTrue(JsonValidator.IsValid(json));
            var expectedJson = "{\"a\":{\"a\":{\"Id\":1,\"Name\":\"a\"},\"b\":{\"Id\":2,\"Name\":\"b\"}}}";
            Assert.AreEqual(expectedJson, json);
        }
    }
}
