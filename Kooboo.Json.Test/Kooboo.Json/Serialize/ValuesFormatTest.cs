using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;
using System;

namespace Kooboo.Json.Test
{
    //    3.  Values

    //    A JSON value MUST be an object, array, number, or string, or one of
    //    the following three literal names:

    //    false null true

    //    The literal names MUST be lowercase.No other literal names are
    //    allowed.

    //    value = false / null / true / object / array / number / string

    //    false = %x66.61.6c.73.65   ; false

    //null  = %x6e.75.6c.6c      ; null

    //true  = %x74.72.75.65      ; true

    //Bray Standards Track[Page 5]

    //RFC 7159                          JSON March 2014

    [TestClass]
    public class ValuesFormatTest
    {
        //    value = object
        [TestMethod]
        public void Values_object_should_be_an_object()
        {
            var obj = new SimplePoco
            {
                Name = "武汉abc",
                Id = 2
            };
            var json = JsonSerializer.ToJson(new { val = obj });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"val\":{\"Name\":\"武汉abc\",\"Id\":2}}", json);
            object newobj = JsonSerializer.ToObject<object>(json);
            var type = ((Json.JObject)newobj)["val"].GetType().Name;
            Assert.IsTrue(type == "JObject");
        }

        //    value = array
        [TestMethod]
        public void Values_array_should_be_an_array()
        {
            var array = new List<int> { 1, 0, -1 };
            var json = JsonSerializer.ToJson(new { val = array });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"val\":[1,0,-1]}", json);
            object newobj = JsonSerializer.ToObject<object>(json);
            var type = ((Json.JObject)newobj)["val"].GetType().Name;
            Assert.IsTrue(type == "JArray");
        }

        private static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        //    value = number
        [TestMethod]
        public void Values_number_should_be_an_number()
        {
            var i = 15;
            var json = JsonSerializer.ToJson(new { val = i });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"val\":15}", json);
            object newobj = JsonSerializer.ToObject<object>(json);
            Assert.IsTrue(IsNumeric(((JObject)newobj)["val"].ToString()));

            var f = 22.113f;
            json = JsonSerializer.ToJson(new { val = f });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"val\":22.113}", json);
            newobj = JsonSerializer.ToObject<object>(json);
            Assert.IsTrue(IsNumeric(((JObject)newobj)["val"].ToString()));

            var m = 10.01m;
            json = JsonSerializer.ToJson(new { val = m });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"val\":10.01}", json);
            newobj = JsonSerializer.ToObject<object>(json);
            Assert.IsTrue(IsNumeric(((JObject)newobj)["val"].ToString()));

            var e = 2.01e2;
            json = JsonSerializer.ToJson(new { val = e });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"val\":201}", json);
            newobj = JsonSerializer.ToObject<object>(json);
            Assert.IsTrue(IsNumeric(((JObject)newobj)["val"].ToString()));
        }

        //    value = string
        [TestMethod]
        public void Values_string_should_be_an_string()
        {
            var str = "武汉abc";
            var obj = new SimplePoco
            {
                Name = str,
                Id = 2
            };
            var json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"武汉abc\",\"Id\":2}", json);
            var newobj = JsonSerializer.ToObject<SimplePoco>(json);
            Assert.AreEqual(newobj.Name, str);
        }

        //    value = null
        [TestMethod]
        public void Values_null_should_be_an_null()
        {
            var obj = new SimplePoco
            {
                Name = null,
                Id = 3
            };
            var json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":null,\"Id\":3}", json);
            var newobj = JsonSerializer.ToObject<SimplePoco>(json);
            Assert.IsTrue(newobj.Name is null);
        }

        //    value = false / true
        [TestMethod]
        public void Values_bool_should_be_an_bool()
        {
            var obj = new BoolPoco
            {
                Val = true,
                Id = 4
            };
            var json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Val\":true,\"Id\":4}", json);
            var newobj = JsonSerializer.ToObject<BoolPoco>(json);
            Assert.IsTrue(newobj.Val);

            obj = new BoolPoco
            {
                Val = false,
                Id = 5
            };
            json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Val\":false,\"Id\":5}", json);
            newobj = JsonSerializer.ToObject<BoolPoco>(json);
            Assert.IsTrue(newobj.Val == false);
        }

        [TestMethod]
        public void DBNullValue_serialize_should_be_an_null()
        {
            var value = DBNull.Value;
            var json = JsonSerializer.ToJson(value);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("null", json);
        }
    }
}
