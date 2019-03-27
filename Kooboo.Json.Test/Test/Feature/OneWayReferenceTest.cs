using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class OneWayReferenceTest
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
        public void OneWayReferenceObject_serialize_should_be_format_correct()
        {
            C c = new C();
            c.a1 = new A();
            c.a2 = c.a1;
            {
                JsonSerializerOption option = new JsonSerializerOption
                {
                    ReferenceLoopHandling = JsonReferenceHandlingEnum.None
                };
                string json = JsonSerializer.ToJson(c, option);
                Assert.AreEqual(
                    "{\"a1\":{\"b\":null},\"a2\":{\"b\":null}}"
                    , json);
            }

            {
                JsonSerializerOption option = new JsonSerializerOption
                {
                    ReferenceLoopHandling = JsonReferenceHandlingEnum.Remove
                };
                string json = JsonSerializer.ToJson(c, option);
                Assert.AreEqual(
                    "{\"a1\":{\"b\":null},\"a2\":{\"b\":null}}"
                    , json);
            }
        }
    }
}