using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class BytesFormatTest
    {
        [TestMethod]
        public void Bytes_serialize_should_be_correct_format()
        {
            byte[] b = { 0x1, 0x2, 0x3, 0x4 };
            var json = JsonSerializer.ToJson(b);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[1,2,3,4]", json);

            json = JsonSerializer.ToJson('c');
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"c\"", json);
        }

        class ByteArrayClass
        {
            public byte[] ByteArray { get; set; }
            public byte[] NullByteArray { get; set; }
        }

        [TestMethod]
        public void ByteArrayClass_serialize_should_be_correct_format()
        {
            var TestData = System.Text.Encoding.UTF8.GetBytes("This is some test data!!!");
            ByteArrayClass byteArrayClass = new ByteArrayClass
            {
                ByteArray = TestData,
                NullByteArray = null
            };

            string json = JsonSerializer.ToJson(byteArrayClass);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"ByteArray\":[84,104,105,115,32,105,115,32,115,111,109,101,32,116,101,115,116,32,100,97,116,97,33,33,33],\"NullByteArray\":null}", json);
        }

        [TestMethod]
        public void ByteArrayClass_use_option_serialize_should_be_correct_format()
        {
           var TestData = System.Text.Encoding.UTF8.GetBytes("This is some test data!!!");
            ByteArrayClass byteArrayClass = new ByteArrayClass
            {
                ByteArray = TestData,
                NullByteArray = null
            };

            string json = JsonSerializer.ToJson(byteArrayClass,new JsonSerializerOption(){IsByteArrayFormatBase64=true});
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"ByteArray\":\"VGhpcyBpcyBzb21lIHRlc3QgZGF0YSEhIQ==\",\"NullByteArray\":null}", json);
        }
    }
}
