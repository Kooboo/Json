using System;
using System.Collections.Generic;
using JsonValidatorTool;
using Kooboo.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class EnumsFormatTest
    {
        enum Enums
        {
            A = 1,
            B = 2
        }

        enum Enums2 : sbyte
        {
            A = -1,
            B = 22
        }

        [TestMethod]
        public void Enums_serialize_should_be_correct_format()
        {

            var json = JsonSerializer.ToJson(Enums.A);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1", json);

            json = JsonSerializer.ToJson(Enums.B);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("2", json);

            json = JsonSerializer.ToJson(Enums2.A);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("-1", json);
            Assert.AreEqual("255", json);

            json = JsonSerializer.ToJson(Enums2.B);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreNotEqual("22", json);
            Assert.AreEqual("5654", json);

            JsonSerializerOption option = new JsonSerializerOption { IsEnumNum = false };
            json = JsonSerializer.ToJson(Enums.A, option);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"A\"", json);

            json = JsonSerializer.ToJson(Enums.B, option);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"B\"", json);

            json = JsonSerializer.ToJson(Enums2.A, option);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"A\"", json);

            json = JsonSerializer.ToJson(Enums2.B, option);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"B\"", json);
        }

        enum EnumDictionaryKeys
        {
            A = 3,
            B = 4,
            C = 11,
            D = 28
        }

        [TestMethod]
        public void EnumDictionaryKeys_serialize_should_be_correct_format()
        {
            var option = new JsonSerializerOption { IsEnumNum = false };
            var json = JsonSerializer.ToJson(new Dictionary<EnumDictionaryKeys, string>
            {
                { EnumDictionaryKeys.A, "hello" },
                { EnumDictionaryKeys.B, "world" },
                { EnumDictionaryKeys.C, "fizz" },
                { EnumDictionaryKeys.D, "buzz" },
            }, option);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"A\":\"hello\",\"B\":\"world\",\"C\":\"fizz\",\"D\":\"buzz\"}", json);
        }

        [Flags]
        public enum ApiRoles : byte
        {
           A=1,
            B=2,
            C=4,
            All=A|B|C
        }

        [TestMethod]
        public void EnumFlags_with_different_options_serialize_should_be_correct_format()
        {
            ApiRoles roles = ApiRoles.All;
            string data = JsonSerializer.ToJson(roles,new JsonSerializerOption(){IsEnumNum=false});
            Assert.AreEqual("\"All\"", data);

            data = JsonSerializer.ToJson(roles, new JsonSerializerOption() { IsEnumNum = true });
            Assert.AreEqual("7", data);

            roles = ApiRoles.A | ApiRoles.B;
            data = JsonSerializer.ToJson(roles, new JsonSerializerOption() { IsEnumNum = false });
            Assert.AreEqual("\"A, B\"", data);

            data = JsonSerializer.ToJson(roles, new JsonSerializerOption() { IsEnumNum = true });
            Assert.AreEqual("3", data);

        }

        private class Enumerables
        {
            public IEnumerable<int> A;
            public Dictionary<int, IEnumerable<int>> B;
            public List<IEnumerable<double>> C;
            public IEnumerable<IEnumerable<string>> D;
        }

        [TestMethod]
        public void Enumerables_serialize_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson(
                new Enumerables
                {
                    A = new[] { 1, 2, 3 },
                    B = new Dictionary<int, IEnumerable<int>> { { 1, new[] { 2, 3 } }, { 2, new[] { 4, 5 } } },
                    C = new List<IEnumerable<double>> { new[] { 1.1, 2.2, 3.3 }, new[] { 4.4, 5.5, 6.6 } },
                    D = new[] { new[] { "hello", "world" }, new[] { "foo", "bar" } }
                });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"A\":[1,2,3],\"B\":{\"1\":[2,3],\"2\":[4,5]},\"C\":[[1.1,2.2,3.3],[4.4,5.5,6.6]],\"D\":[[\"hello\",\"world\"],[\"foo\",\"bar\"]]}", json);
        }
    }
}
