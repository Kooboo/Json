using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;

namespace Kooboo.Json.Test
{

    [TestClass]
    public class BytesDeserializeTest
    {
        [TestMethod]
        public void Byte_deserialize_should_be_correct()
        {
            var str = byte.MaxValue.ToString();
            var res = JsonSerializer.ToObject<byte>(str);
            Assert.AreEqual(byte.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<byte>(str);
            Assert.AreEqual((byte)0, res);

            str = byte.MinValue.ToString();
            res = JsonSerializer.ToObject<byte>(str);
            Assert.AreEqual(byte.MinValue, res);
        }

        [TestMethod]
        public void SByte_deserialize_should_be_correct()
        {
            var str = sbyte.MaxValue.ToString();
            var res = JsonSerializer.ToObject<sbyte>(str);
            Assert.AreEqual(sbyte.MaxValue, res);

            str = "0";
            res = JsonSerializer.ToObject<sbyte>(str);
            Assert.AreEqual((sbyte)0, res);

            str = sbyte.MinValue.ToString();
            res = JsonSerializer.ToObject<sbyte>(str);
            Assert.AreEqual(sbyte.MinValue, res);
        }

        [TestMethod]
        public void OverflowByte_deserialize_should_not_be_correct()
        {
            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<byte>("257");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<sbyte>("128");
            });

            Assert.ThrowsException<OverflowException>(() =>
            {
                JsonSerializer.ToObject<sbyte>("-129");
            });
        }

        class ByteArrayClass
        {
            public byte[] ByteArray { get; set; }
            public byte[] NullByteArray { get; set; }
        }

        [TestMethod]
        public void ByteArray_Base64_deserialize_should_be_correct()
        {
            string json = "{\"ByteArray\":\"VGhpcyBpcyBzb21lIHRlc3QgZGF0YSEhIQ==\",\"NullByteArray\":null}";
            var TestData = System.Text.Encoding.UTF8.GetBytes("This is some test data!!!");
            ByteArrayClass byteArrayClass = JsonSerializer.ToObject<ByteArrayClass>(json);

            CollectionAssert.AreEquivalent(TestData, byteArrayClass.ByteArray);
            Assert.AreEqual(null, byteArrayClass.NullByteArray);
        }

        [TestMethod]
        public void ByteArray_deserialize_should_be_correct()
        {
            string json = "{\"ByteArray\": [0, 1, 2, 3],\"NullByteArray\": null}";
            ByteArrayClass c = JsonSerializer.ToObject<ByteArrayClass>(json);
            Assert.IsNotNull(c.ByteArray);
            Assert.AreEqual(4, c.ByteArray.Length);
            CollectionAssert.AreEquivalent(new byte[] { 0, 1, 2, 3 }, c.ByteArray);
        }
    }
}
