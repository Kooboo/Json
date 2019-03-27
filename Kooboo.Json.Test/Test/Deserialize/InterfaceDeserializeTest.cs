using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class InterfaceDeserializeTest
    {
        public interface INterface1
        {
            int A { get; set; }
            string B { get; set; }
        }

        public interface INterface2 : INterface1
        {
            double C { get; set; }
        }

        [TestMethod]
        public void Interface_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<INterface1>("{\"A\":1234, \"B\": \"hello world\"}");
            Assert.IsNotNull(res);
            Assert.AreEqual(1234, res.A);
            Assert.AreEqual("hello world", res.B);

            var res2 = JsonSerializer.ToObject<INterface2>("{\"A\":1234, \"B\": \"hello world\", \"C\": 3.14159}");
            Assert.IsNotNull(res2);
            Assert.AreEqual(1234, res2.A);
            Assert.AreEqual("hello world", res2.B);
            Assert.AreEqual(3.14159, res2.C);
        }

        public interface INormal
        {
            string this[string i] { set; }

            string Name { get; set; }

            int Age { get; set; }
        }
        public interface ICustom<T> : INormal
        {
            T this[T i] { get; set; }

            new T Name { get; set; }

            T get_Item();

            void F();
        }

        [TestMethod]
        public void Interface_under_the_special_condition_deserialize_should_be_correct()
        {
            {
                var c = Json.Deserialize.InterfaceImplementation<ICustom<string>>.Proxy;
                var obj = (ICustom<string>)Activator.CreateInstance(c);
                obj.Name = "3";
                obj.Age = 1;
            }
            {
                var c = Json.Deserialize.InterfaceImplementation<ICustom<int>>.Proxy;
                var obj = (ICustom<int>)Activator.CreateInstance(c);
                obj.Name = 3;
                obj.Age = 1;
            }
        }
    }
}
