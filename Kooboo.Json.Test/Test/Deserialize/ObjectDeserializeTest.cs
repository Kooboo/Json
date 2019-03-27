using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class ObjectDeserializeTest
    {
        class _Objects
        {
            public int A;
            public string B { get; set; }
        }

        [TestMethod]
        public void NullString_deserialize_to_define_object_should_be_null()
        {
            var str = "null";
            var obj = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void NullString_deserialize_to_object_should_be_null()
        {
            var str = "null";
            var obj = JsonSerializer.ToObject<object>(str);
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void EmptyObjectString_deserialize_to_define_object_should_be_correct()
        {
            var str = "{}";
            var val = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual(default(int), val.A);
            Assert.IsNull(val.B);
        }

        [TestMethod]
        public void EmptyObjectString_deserialize_to_object_should_be_correct()
        {
            var str = "{}";
            var val = JsonSerializer.ToObject<object>(str);
            Assert.IsNotNull(val);
        }

        [TestMethod]
        public void NormalObjectString_deserialize_to_object_should_be_correct()
        {
            var str = "{\"A\": 456, \"B\": \"hello\"}";
            var val = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual("hello", val.B);
            Assert.AreEqual(456, val.A);
        }

        [TestMethod]
        public void SkipMembersObjectString_deserialize_to_object_should_be_correct()
        {
            var str = "{\"A\": 123}";
            var val = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual(123, val.A);

            str = "{\"B\": \"hello\"}";
            val = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual("hello", val.B);
        }

        [TestMethod]
        public void ObjectString_with_space_deserialize_to_object_should_be_correct()
        {
            var str = "   {  \"B\"    :   \"hello\"    ,    \"A\"   :   456   }  ";
            var val = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual("hello", val.B);
            Assert.AreEqual(456, val.A);
        }

        [TestMethod]
        public void ObjectString_with_order_deserialize_to_object_should_be_correct()
        {
            var str = "{\"B\": \"hello\", \"A\": 456}";
            var val = JsonSerializer.ToObject<_Objects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual("hello", val.B);
            Assert.AreEqual(456, val.A);
        }

        class _RecursiveObjects
        {
            public string A;
            public _RecursiveObjects B;
        }

        [TestMethod]
        public void RecursiveObjectsString_deserialize_to_RecursiveObject_should_be_correct()
        {
            var str = "{\"A\": \"hello world\", \"B\": { \"A\": \"foo bar\", \"B\": {\"A\": \"fizz buzz\"}}}";
            var val = JsonSerializer.ToObject<_RecursiveObjects>(str);
            Assert.IsNotNull(val);
            Assert.AreEqual("hello world", val.A);
            Assert.AreEqual("foo bar", val.B.A);
            Assert.AreEqual("fizz buzz", val.B.B.A);
            Assert.IsNull(val.B.B.B);
        }

        class DataMemberName
        {
            public string Plain { get; set; }

            [Alias("FakeName")]
            public string RealName { get; set; }

            [Alias("NotSoSecretName")]
            public int SecretName { get; set; }
        }

        [TestMethod]
        public void AliasNameObjectString_deserialize_should_be_correct()
        {
            var str = "{\"Plain\":\"hello world\",\"FakeName\":\"Really RealName\",\"NotSoSecretName\":314159}";
            var obj = JsonSerializer.ToObject<DataMemberName>(str);
            Assert.IsNotNull(obj);
            Assert.AreEqual("hello world", obj.Plain);
            Assert.AreEqual("Really RealName", obj.RealName);
            Assert.AreEqual(314159, obj.SecretName);
        }

        static T _EmptyAnonymousObject<T>(T example, string str)
        {
            return JsonSerializer.ToObject<T>(str);
        }

        [TestMethod]
        public void EmptyAnonymousObject_deserialize_should_be_correct()
        {
            var ex = new { };
            var obj = _EmptyAnonymousObject(ex, "null");
            Assert.IsNull(obj);

            obj = _EmptyAnonymousObject(ex, "{}");
            Assert.IsNotNull(obj);
        }

      public  interface IRange<T>
        {
            T First { get; set; }
            T Last { get; set; }
        }

        class Range<T> : IRange<T>
        {
            public T First { get; set; }
            public T Last { get; set; }
        }

        class NullInterfaceTestClass
        {
            public virtual Guid Id { get; set; }
            public virtual int? Year { get; set; }
            public virtual string Company { get; set; }
            public virtual IRange<decimal> DecimalRange { get; set; }
            public virtual IRange<int> IntRange { get; set; }
            public virtual IRange<decimal> NullDecimalRange { get; set; }
        }

        [TestMethod]
        public void NullValueObjectString_deserialize_should_be_correct()
        {
            NullInterfaceTestClass initial = new NullInterfaceTestClass
            {
                Company = "Company!",
                DecimalRange = new Range<decimal> { First = 0, Last = 1 },
                Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11),
                IntRange = new Range<int> { First = int.MinValue, Last = int.MaxValue },
                Year = 2010,
                NullDecimalRange = null
            };

            string json = JsonSerializer.ToJson(initial);

            Assert.AreEqual("{\"Id\":\"00000001-0002-0003-0405-060708090a0b\",\"Year\":2010,\"Company\":\"Company!\",\"DecimalRange\":{\"First\":0,\"Last\":1},\"IntRange\":{\"First\":-2147483648,\"Last\":2147483647},\"NullDecimalRange\":null}", json);

            NullInterfaceTestClass deserialized = JsonSerializer.ToObject<NullInterfaceTestClass>(
                json);

            Assert.AreEqual("Company!", deserialized.Company);
            Assert.AreEqual(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11), deserialized.Id);
            Assert.AreEqual(0, deserialized.DecimalRange.First);
            Assert.AreEqual(1, deserialized.DecimalRange.Last);
            Assert.AreEqual(int.MinValue, deserialized.IntRange.First);
            Assert.AreEqual(int.MaxValue, deserialized.IntRange.Last);
            Assert.AreEqual(null, deserialized.NullDecimalRange);
            Assert.AreEqual(2010, deserialized.Year);
        }

        [TestMethod]
        public void ExpandoObject_deserialize_should_be_correct()
        {
            var json = "{\"Foo\":123,\"Bar\":\"hello\"}";

            dynamic obj = JsonSerializer.ToObject<ExpandoObject>(json);
            Assert.AreEqual(123, obj.Foo);
            Assert.AreEqual("hello", obj.Bar);

            IDictionary<string, object> obj2 = JsonSerializer.ToObject<ExpandoObject>(json);
            Assert.AreEqual(123l, obj2["Foo"]);
            Assert.AreEqual("hello", obj2["Bar"]);
        }

        class Student
        {
            public Student()
            {
                Name = "DefaultName";
                Age = 0;
            }

            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void LazyObject_deserialize_should_be_correct()
        {
            Lazy<Student> stu = new Lazy<Student>();
            stu.Value.Name = "Tom";
            stu.Value.Age = 21;
            var json = JsonSerializer.ToJson(stu);
            var lazystu = JsonSerializer.ToObject<Lazy<Student>>(json);
            Assert.AreEqual("Tom", lazystu.Value.Name);
            Assert.AreEqual(21, lazystu.Value.Age);
        }
    }
}
