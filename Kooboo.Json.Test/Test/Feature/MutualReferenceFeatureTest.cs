using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class MutualReferenceFeatureTest
    {
        class A
        {
            public B b;
        }
        class B
        {
            public A a;
        }
        class C
        {
            public A a1;
            public A a2;
        }
        class D
        {
            public D a;
            public D b;
            public D c;
            public D d;
        }

        [TestMethod]
        public void MutualReferenceObject_use_option_null_serialize_should_be_format_correct()
        {
            A a = new A();
            B b = new B();
            a.b = b;
            b.a = a;
            JsonSerializerOption option = new JsonSerializerOption
            {
                ReferenceLoopHandling = JsonReferenceHandlingEnum.Null
            };
            string json = JsonSerializer.ToJson(a, option);
            Assert.AreEqual(
                "{\"b\":{\"a\":null}}"
                , json);
        }

        [TestMethod]
        public void MutualReferenceObject_use_option_empty_serialize_should_be_format_correct()
        {
            A a = new A();
            B b = new B();
            a.b = b;
            b.a = a;
            JsonSerializerOption option = new JsonSerializerOption
            {
                ReferenceLoopHandling = JsonReferenceHandlingEnum.Empty
            };
            string json = JsonSerializer.ToJson(a, option);
            Assert.AreEqual(
                "{\"b\":{\"a\":{}}}"
                , json);
        }

        [TestMethod]
        public void MutualReferenceObject_use_option_remove_serialize_should_be_format_correct()
        {
            A a = new A();
            B b = new B();
            a.b = b;
            b.a = a;
            JsonSerializerOption option = new JsonSerializerOption
            {
                ReferenceLoopHandling = JsonReferenceHandlingEnum.Remove
            };
            string json = JsonSerializer.ToJson(a, option);
            Assert.AreEqual(
                "{\"b\":{}}"
                , json);
        }

        [TestMethod]
        public void MutualReferenceArray_use_option_should_be_format_correct()
        {
            ArrayList array = new ArrayList();
            ArrayList child = new ArrayList();

            array.Add(child);
            array.Add(child);
            array.Add(array);

            JsonSerializerOption option = new JsonSerializerOption
            {
                ReferenceLoopHandling = JsonReferenceHandlingEnum.Null
            };
            string json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[],null]"
                , json);

            option.ReferenceLoopHandling = JsonReferenceHandlingEnum.Remove;
            json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[]]"
                , json);

            option.ReferenceLoopHandling = JsonReferenceHandlingEnum.Empty;
            json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[],[]]"
                , json);

            Dictionary<string, object> dic = new Dictionary<string, object>
            {
                { "s", "s" }
            };
            dic.Add("f", dic);
            array.Add(dic);

            json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[],[],{\"s\":\"s\",\"f\":{}}]"
                , json);

            option.ReferenceLoopHandling = JsonReferenceHandlingEnum.Remove;

            json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[],{\"s\":\"s\"}]"
                , json);

            dic.Add("xs", "sx");

            json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[],{\"s\":\"s\",\"xs\":\"sx\"}]"
                , json);

            array.Add(child);

            json = JsonSerializer.ToJson(array, option);
            Assert.AreEqual(
                "[[],[],{\"s\":\"s\",\"xs\":\"sx\"},[]]"
                , json);
        }

        [TestMethod]
        public void MutualReferenceObject_under_remove_state_should_be_format_without_extra_comma()
        {
            D d = new D();
            d.a = new D();
            d.b = new D();
            d.c = new D();
            d.d = new D();
            JsonSerializerOption option =
                new JsonSerializerOption
                {
                    ReferenceLoopHandling = JsonReferenceHandlingEnum.Remove,
                    IsIgnoreValueNull = true
                };

            string json;

            d.d = d;
            /*
             k:a,
             k:b,
             k:c,
             k:d---
             */
            json = JsonSerializer.ToJson(d, option);
            Assert.AreEqual(
                "{\"a\":{},\"b\":{},\"c\":{}}", json);

            d.a = d;
            /*
             k:a---,
             k:b,
             k:c,
             k:d---
             */
            json = JsonSerializer.ToJson(d, option);
            Assert.AreEqual(
                "{\"b\":{},\"c\":{}}", json);

            d.b = d; ;
            /*
             k:a---,
             k:b---,
             k:c,
             k:d---
             */
            json = JsonSerializer.ToJson(d, option);
            Assert.AreEqual(
                "{\"c\":{}}", json);
        }
    }
}