using JsonValidatorTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class InitialFeatureTest
    {
        class A
        {
            public int age;
        }
        [TestMethod]
        public void InitialUpperFeature_should_be_work_correct()
        {
            var a = new A { age = 0 };
            var json = JsonSerializer.ToJson(a, new JsonSerializerOption { JsonCharacterRead = JsonCharacterReadStateEnum.InitialUpper });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Age\":0}", json);


            var obj = JsonSerializer.ToObject<A>(json, new JsonDeserializeOption() { JsonCharacterReadState = JsonCharacterReadStateEnum.InitialLower });
            Assert.AreEqual(obj.age, 0);
        }



        class B
        {
            public int Age;
        }
        [TestMethod]
        public void InitialLowwerFeature_should_be_work_correct()
        {
            var b = new B { Age = 0 };
            var json = JsonSerializer.ToJson(b, new JsonSerializerOption { JsonCharacterRead = JsonCharacterReadStateEnum.InitialLower });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"age\":0}", json);


            var obj = JsonSerializer.ToObject<B>(json, new JsonDeserializeOption() { JsonCharacterReadState = JsonCharacterReadStateEnum.InitialUpper });
            Assert.AreEqual(obj.Age, 0);
        }
    }
}