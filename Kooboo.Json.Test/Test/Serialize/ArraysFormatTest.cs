using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    //    5.  Arrays

    //    An array structure is represented as square brackets surrounding zero
    //    or more values(or elements).  Elements are separated by commas.

    //    array = begin-array[value * (value - separator value ) ] end-array

    //There is no requirement that the values in an array be of the same
    //type.

    [TestClass]
    public class ArraysFormatTest
    {
        //    An array structure is represented as square brackets surrounding zero
        //    or more values(or elements).  Elements are separated by commas.
        [TestMethod]
        public void Empty_array_should_be_serialize_to_correct_format()
        {
            var x = new int[,] { { }, { } };
            var json = Json.JsonSerializer.ToJson(x);

            Assert.IsTrue(JsonValidator.IsValid(json));

            Assert.AreNotEqual("[]", json);
            Assert.AreEqual("[[],[]]", json);

            var objs = new List<object>
            {
                new object(),
                new object(),
                new object(),
                new object()
            };
            json = Json.JsonSerializer.ToJson(objs);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{},{},{},{}]", json);
        }

        [TestMethod]
        public void Unassigned_array_should_be_serialize_to_correct_format()
        {
            var x = new int[2];
            var json =Json.JsonSerializer.ToJson(x);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[0,0]", json);
        }

        //    array = begin-array[value * (value - separator value ) ] end-array

        [TestMethod]
        public void Assigned_array_should_be_serialize_to_correct_format()
        {
            int[,] a = {
                {0, 1, 2, 3} ,
                {4, 5, 6, 7} ,
                {8, 9, 10, 11}
            };   

            var json =Json.JsonSerializer.ToJson(a);
            Assert.IsTrue(JsonValidator.IsValid(json));

            Assert.AreEqual("[[0,1,2,3],[4,5,6,7],[8,9,10,11]]", json);
        }

        [TestMethod]
        public void List_should_be_serialize_to_correct_format()
        {
            var list = new List<string>
            {
                "`1~!@#$%^&*()_+-={':[,]}|;.</>?",
                "\\u0123\\u4567\\u89AB\\uCDEF\\uabcd\\uef4A",
                "50 St. James Street",
                "http://www.JSON.org/",
                "// /* <!-- --",
                "# -- --> */\": \"",
                "{\"object with 1 member\":[\"array with 1 element\"]}",
                "&#34; \u0022 %22 0x22 034 &#x22;",
                "\\/\\\\\\\"\\uCAFE\\uBABE\\uAB98\\uFCDE\\ubcda\\uef4A\\b\\f\\n\\r\\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?"
            };
            var json = Json.JsonSerializer.ToJson(list);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[\"`1~!@#$%^&*()_+-={':[,]}|;.</>?\",\"\\\\u0123\\\\u4567\\\\u89AB\\\\uCDEF\\\\uabcd\\\\uef4A\",\"50 St. James Street\",\"http://www.JSON.org/\",\"// /* <!-- --\",\"# -- --> */\\\": \\\"\",\"{\\\"object with 1 member\\\":[\\\"array with 1 element\\\"]}\",\"&#34; \\\" %22 0x22 034 &#x22;\",\"\\\\/\\\\\\\\\\\\\\\"\\\\uCAFE\\\\uBABE\\\\uAB98\\\\uFCDE\\\\ubcda\\\\uef4A\\\\b\\\\f\\\\n\\\\r\\\\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?\"]", json);

            var digits = new List<int> { 0 + 1, 12 % 3, -9 / 9 };
            json = Json.JsonSerializer.ToJson(digits);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[1,0,-1]", json);

            var doubles = new List<double> { 2.99, 12 / 5.0, 88.10, 1.2E-2, 1.2e+2 };
            json = Json.JsonSerializer.ToJson(doubles);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[2.99,2.4,88.1,0.012,120]", json);

            var bools = new List<bool> { true, false, 3 > 1, 1 == 0 };
            json = Json.JsonSerializer.ToJson(bools);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[true,false,true,false]", json);

            var objs = new List<SimplePoco>
            {
                new SimplePoco {Name = "Sylvester", Id = 8},
                new SimplePoco {Name = "Whiskers", Id = 2},
                new SimplePoco {Name = "Sasha", Id = 14},
                new SimplePoco {Name = "", Id = -9}
            };
            json = Json.JsonSerializer.ToJson(objs);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"Name\":\"Sylvester\",\"Id\":8},{\"Name\":\"Whiskers\",\"Id\":2},{\"Name\":\"Sasha\",\"Id\":14},{\"Name\":\"\",\"Id\":-9}]", json);

            json = Json.JsonSerializer.ToJson(
                new[] { new Cyclical { Foo = 123, Next = new Cyclical { Foo = 456 } }, new Cyclical { Foo = 789 } });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"Foo\":123,\"Next\":{\"Foo\":456,\"Next\":null}},{\"Foo\":789,\"Next\":null}]", json);
        }

        //There is no requirement that the values in an array be of the same type.
        [TestMethod]
        public void Values_in_an_array_should_not_be_the_same_type()
        {
            var list = new ArrayList();
            list.Add("abc");
            list.Add(123);
            list.Add(new object());
            list.Add(true);
            list.Add(new SimplePoco { Name = "Sasha", Id = 14 });
            var json = Json.JsonSerializer.ToJson(list);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[\"abc\",123,{},true,{\"Name\":\"Sasha\",\"Id\":14}]", json);
        }

        [TestMethod]
        public void MultiDimention_array_should_be_serialize_to_correct_format()
        {
            int[,] f = new int[3, 3];
            var ll = Json.JsonSerializer.ToJson(f);
            Assert.IsTrue(JsonValidator.IsValid(ll));
            Assert.AreEqual("[[0,0,0],[0,0,0],[0,0,0]]", ll);

            int[][] ww = new int[3][];
            ww[0] = new int[3];
            ww[1] = new int[3];
            ww[2] = new int[3];
            var ll2 = Json.JsonSerializer.ToJson(ww);
            Assert.IsTrue(JsonValidator.IsValid(ll2));
            Assert.AreEqual("[[0,0,0],[0,0,0],[0,0,0]]", ll2);
        }
    }
}
