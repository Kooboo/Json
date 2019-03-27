using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;
using System;

namespace Kooboo.Json.Test
{
    //    4.  Objects

    //    An object structure is represented as a pair of curly brackets
    //    surrounding zero or more name/value pairs(or members).  A name is a
    //    string.  A single colon comes after each name, separating the name
    //    from the value.A single comma separates a value from a following
    //    name.The names within an object SHOULD be unique.

    //    object = begin - object[member * (value - separator member ) ]
    //end-object

    //member = string name-separator value

    //An object whose names are all unique is interoperable in the sense
    //that all software implementations receiving that object will agree on
    //the name-value mappings.When the names within an object are not
    //unique, the behavior of software that receives such an object is
    //unpredictable.Many implementations report the last name/value pair
    //only.Other implementations report an error or fail to parse the
    //object, and some implementations report all of the name/value pairs,
    //including duplicates.

    //JSON parsing libraries have been observed to differ as to whether or
    //not they make the ordering of object members visible to calling
    //software.  Implementations whose behavior does not depend on member
    //ordering will be interoperable in the sense that they will not be
    //affected by these differences.

    [TestClass]
    public class ObjectFormatTest
    {
        //    An object structure is represented as a pair of curly brackets
        //    surrounding zero or more name/value pairs(or members).  A name is a
        //    string.  A single colon comes after each name, separating the name
        //    from the value.A single comma separates a value from a following
        //    name.The names within an object SHOULD be unique.

        //    object = begin - object[member * (value - separator member ) ]
        //end-object

        //member = string name-separator value

        [TestMethod]
        public void Empty_object_should_be_serialize_to_correct_format()
        {
            var data = new {};
            var json = JsonSerializer.ToJson(data);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{}", json);
        }

        [TestMethod]
        public void Unassigned_object_should_be_serialize_to_correct_format()
        {
            var data = new SimplePoco();
            var json = JsonSerializer.ToJson(data);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":null,\"Id\":0}", json);
        }

        [TestMethod]
        public void Assigned_object_should_be_serialize_to_correct_format()
        {
            var data = new SimplePoco
            {
                Name = "武汉abc",
                Id = 1
            };
            var json = JsonSerializer.ToJson(data);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"武汉abc\",\"Id\":1}", json);
        }

        //JSON parsing libraries have been observed to differ as to whether or
        //not they make the ordering of object members visible to calling
        //software.  Implementations whose behavior does not depend on member
        //ordering will be interoperable in the sense that they will not be
        //affected by these differences.
        [TestMethod]
        public void Ordering_object_should_not_be_affect_format()
        {
            var positivedata = new SimplePoco
            {
                Name = "武汉abc",
                Id = 1
            };
            var positivejson= JsonSerializer.ToJson(positivedata);
            Assert.IsTrue(JsonValidator.IsValid(positivejson));

            var reversedata = new SimplePoco
            {
                Id = 1,
                Name = "武汉abc"
            };
            var reversejson = JsonSerializer.ToJson(reversedata);
            Assert.IsTrue(JsonValidator.IsValid(reversejson));

            Assert.AreEqual(positivejson, reversejson);
        }

        //An object whose names are all unique is interoperable in the sense
        //that all software implementations receiving that object will agree on
        //the name-value mappings.When the names within an object are not
        //unique, the behavior of software that receives such an object is
        //unpredictable.Many implementations report the last name/value pair
        //only.Other implementations report an error or fail to parse the
        //object, and some implementations report all of the name/value pairs,
        //including duplicates.

        [TestMethod]
        public void Dynamic_object_should_be_serialize_to_correct_format()
        {
            dynamic obj = new ExpandoObject();
            var json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{}", json);

            obj.Name = "zz";
            obj.Age = 10;
            json = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"zz\",\"Age\":10}", json);
        }

        [TestMethod]
        public void Cyclical_object_should_be_serialize_to_correct_format()
        {
            var json = JsonSerializer.ToJson(
                new Cyclical { Foo = 123, Next = new Cyclical { Foo = 456 } });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Foo\":123,\"Next\":{\"Foo\":456,\"Next\":null}}", json);

            json = JsonSerializer.ToJson(
                new[] { new Cyclical { Foo = 123, Next = new Cyclical { Foo = 456 } }, new Cyclical { Foo = 789 } });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"Foo\":123,\"Next\":{\"Foo\":456,\"Next\":null}},{\"Foo\":789,\"Next\":null}]", json);

            json = JsonSerializer.ToJson(
                new Dictionary<string, Cyclical> { { "hello", new Cyclical { Foo = 123, Next = new Cyclical { Foo = 456 } } }, { "world", new Cyclical { Foo = 789 } } });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"hello\":{\"Foo\":123,\"Next\":{\"Foo\":456,\"Next\":null}},\"world\":{\"Foo\":789,\"Next\":null}}", json);
        }

        [TestMethod]
        public void AnonymousType_should_be_serialize_to_correct_format()
        {
            var json = JsonSerializer.ToJson(
                new { Hoge = 100, Huga = true, Yaki = new { Rec = 1, T = 10 }, Nano = "nanoanno" });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(@"{""Hoge"":100,""Huga"":true,""Yaki"":{""Rec"":1,""T"":10},""Nano"":""nanoanno""}", json);
        }

        public class A
        {
        }

        public class B
        {
            public A A { get; set; }

            public virtual bool ShouldSerializeA()
            {
                return false;
            }
        }

        [TestMethod]
        public void Virtual_should_be_serialize_to_correct_format()
        {
            string json = JsonSerializer.ToJson(new B(),new JsonSerializerOption(){IsIgnoreValueNull=true});

            Assert.AreEqual("{}", json);
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
        public void LazyObject_should_be_serialize_to_correct_format()
        {
            Lazy<Student> stu = new Lazy<Student>();
            stu.Value.Name = "Tom";
            stu.Value.Age = 21;
            var json = JsonSerializer.ToJson(stu);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"Tom\",\"Age\":21}", json);
        }
    }
}
