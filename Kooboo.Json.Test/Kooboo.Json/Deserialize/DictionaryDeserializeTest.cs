using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System.Collections.Generic;
using JsonValidatorTool;
using System;
using System.Data;
using System.Collections.Concurrent;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class DictionaryDeserializeTest
    {
        [TestMethod]
        public void NormalDictionaryString_deserialize_to_IDictionary_should_be_correct()
        {
            var str = "{\"1\":2, \"3\":4, \"5\": 6}";
            var res = JsonSerializer.ToObject<IDictionary<int, int>>(str);
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(2, res[1]);
            Assert.AreEqual(4, res[3]);
            Assert.AreEqual(6, res[5]);

            str = "{\"1\":\"hello world\",\"" + ulong.MaxValue + "\":\"fizz buzz\"}";
            var res2 = JsonSerializer.ToObject<IDictionary<ulong, string>>(str);
            Assert.AreEqual("hello world", res2[1]);
            Assert.AreEqual("fizz buzz", res2[ulong.MaxValue]);
        }

        [TestMethod]
        public void Null_deserialize_should_be_correct()
        {
            var str = "{\"A\": null}";
            var res = JsonSerializer.ToObject<Dictionary<string, string>>(str);
            Assert.IsNotNull(res);
            Assert.IsNull(res["A"]);
        }

        enum _DictionaryEnumKeys1 : byte
        {
            A,
            B
        }

        [TestMethod]
        public void DictionaryString_EnumKeys_deserialize_should_be_correct()
        {
            var str = "{\"A\":\"hello world\"}";
            var res = JsonSerializer.ToObject<Dictionary<_DictionaryEnumKeys1, string>>(str);
            Assert.AreEqual("hello world", res[_DictionaryEnumKeys1.A]);
        }

        [TestMethod]
        public void DictionaryString_NumberKeys_deserialize_should_be_correct()
        {
            var str = "{\"1\":\"hello world\",\"2\":\"fizz buzz\"}";
            var res = JsonSerializer.ToObject<Dictionary<byte, string>>(str);
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("hello world", res[1]);
            Assert.AreEqual("fizz buzz", res[2]);

            str = "{\"-1\":\"hello world\"}";
            var sres = JsonSerializer.ToObject<Dictionary<sbyte, string>>(str);
            Assert.AreEqual("hello world", sres[-1]);
        }

        [TestMethod]
        public void IReadOnlyDictionaryString_NumberKeys_deserialize_should_be_correct()
        {
            var str = "{\"1\":\"hello world\",\"2\":\"fizz buzz\"}";
            var res = JsonSerializer.ToObject<IReadOnlyDictionary<byte, string>>(str);
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("hello world", res[1]);
            Assert.AreEqual("fizz buzz", res[2]);

            str = "{\"-1\":\"hello world\"}";
            var sres = JsonSerializer.ToObject<IReadOnlyDictionary<sbyte, string>>(str);
            Assert.AreEqual("hello world", sres[-1]);
        }

        [TestMethod]
        public void DictionaryString_DynamicMembers_deserialize_should_be_correct()
        {
            const string json = @"{
                  ""index.analysis.analyzer.stem.tokenizer"" : ""standard"",
                  ""index.analysis.analyzer.exact.filter.0"" : ""lowercase"",
                  ""index.refresh_interval"" : ""1s"",
                  ""index.analysis.analyzer.exact.type"" : ""custom"",
                  ""test-dummy-obj"": { ""hello"": 123 }
	        }";

            var dyn = JsonSerializer.ToObject<Dictionary<string, dynamic>>(json);
            Assert.IsNotNull(dyn);
            Assert.AreEqual(5, dyn.Count);
            Assert.AreEqual("standard", (string)dyn["index.analysis.analyzer.stem.tokenizer"]);
            Assert.AreEqual("lowercase", (string)dyn["index.analysis.analyzer.exact.filter.0"]);
            Assert.AreEqual("1s", (string)dyn["index.refresh_interval"]);
            Assert.AreEqual("custom", (string)dyn["index.analysis.analyzer.exact.type"]);
            Assert.IsNotNull(dyn["test-dummy-obj"]);
            var testDummyObj = dyn["test-dummy-obj"];

            var count = 0;
            foreach (var kv in testDummyObj)
            {
                var key = kv.Key;
                var val = kv.Value;
                count++;

                Assert.AreEqual("hello", (string)key);
                Assert.AreEqual(123, (int)val);
            }

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void KeyValuePair_deserialize_should_be_correct()
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("key1", "value1"));
            list.Add(new KeyValuePair<string, string>("key2", "value2"));

            string json = JsonSerializer.ToJson(list);
            Assert.IsTrue(JsonValidator.IsValid(json));

            List<KeyValuePair<string, string>> result = JsonSerializer.ToObject<List<KeyValuePair<string, string>>>(json);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("key1", result[0].Key);
            Assert.AreEqual("value1", result[0].Value);
            Assert.AreEqual("key2", result[1].Key);
            Assert.AreEqual("value2", result[1].Value);

            IList<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("123", "2017-05-19T11:00:59")
            };

            json = JsonSerializer.ToJson(values);
            Assert.IsTrue(JsonValidator.IsValid(json));

            IList<KeyValuePair<string, string>> v1 = JsonSerializer.ToObject<IList<KeyValuePair<string, string>>>(json);

            Assert.AreEqual("123", v1[0].Key);
            Assert.AreEqual("2017-05-19T11:00:59", v1[0].Value);
        }

        class Person
        {
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
            public DateTime LastModified { get; set; }
            public string Department { get; set; }
        }

        class WagePerson : Person
        {
            public decimal HourlyWage { get; set; }
        }

        [TestMethod]
        public void ComplexKeyValuePair_deserialize_should_be_correct()
        {
            DateTime dateTime = new DateTime(2000, 12, 1, 23, 1, 1, DateTimeKind.Utc);

            List<KeyValuePair<string, WagePerson>> list = new List<KeyValuePair<string, WagePerson>>();
            list.Add(new KeyValuePair<string, WagePerson>("key1", new WagePerson
            {
                BirthDate = dateTime,
                Department = "Department1",
                LastModified = dateTime,
                HourlyWage = 1
            }));
            list.Add(new KeyValuePair<string, WagePerson>("key2", new WagePerson
            {
                BirthDate = dateTime,
                Department = "Department2",
                LastModified = dateTime,
                HourlyWage = 2
            }));

            string json = JsonSerializer.ToJson(list);
            Assert.IsTrue(JsonValidator.IsValid(json));

            List<KeyValuePair<string, WagePerson>> result = JsonSerializer.ToObject<List<KeyValuePair<string, WagePerson>>>(json);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("key1", result[0].Key);
            Assert.AreEqual(1, result[0].Value.HourlyWage);
            Assert.AreEqual("key2", result[1].Key);
            Assert.AreEqual(2, result[1].Value.HourlyWage);
        }

        T Convert<T>(T value)
        {
            return JsonSerializer.ToObject<T>(JsonSerializer.ToJson(value));
        }

        [TestMethod]
        public void ConcurrentDictionary_deserialize_should_be_correct()
        {
            var cd = new ConcurrentDictionary<int, int>();

            cd.TryAdd(1, 100);
            cd.TryAdd(2, 200);
            cd.TryAdd(3, 300);

            var conv = Convert(cd);
            Assert.AreEqual(100, conv[1]);
            Assert.AreEqual(200, conv[2]);
            Assert.AreEqual(300, conv[3]);

            cd = null;
            Assert.IsNull(Convert(cd));
        }
    }
}
