using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class StringDeserializeTest
    {
        [TestMethod]
        public void control_and_unicode_characters_deserialize_should_be_correct()
        {

            var res = JsonSerializer.ToObject<string>("\"\"");
            Assert.AreEqual("", res);

            res = JsonSerializer.ToObject<string>("\"\\\\\"");
            Assert.AreEqual("\\", res);

            res = JsonSerializer.ToObject<string>("\"\\/\"");
            Assert.AreEqual("/", res);

            res = JsonSerializer.ToObject<string>("\"\\b\"");
            Assert.AreEqual("\b", res);

            res = JsonSerializer.ToObject<string>("\"\\f\"");
            Assert.AreEqual("\f", res);

            res = JsonSerializer.ToObject<string>("\"\\r\"");
            Assert.AreEqual("\r", res);

            res = JsonSerializer.ToObject<string>("\"\\n\"");
            Assert.AreEqual("\n", res);

            res = JsonSerializer.ToObject<string>("\"\\t\"");
            Assert.AreEqual("\t", res);

            res = JsonSerializer.ToObject<string>("\"\\f\"");
            Assert.AreEqual("\f", res);

            for (var i = 0; i <= 2048; i++)
            {
                var asStr = "\"\\u" + i.ToString("X4") + "\"";

                var c = JsonSerializer.ToObject<string>(asStr);

                var shouldBe = "" + (char)i;

              Assert.AreEqual(shouldBe, c);
            }
        }

        [TestMethod]
        public void null_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<string>("null");
            Assert.AreEqual(null, res);
        }

        string unicodeproduce(string unicode)
        {
            return char.ConvertFromUtf32(int.Parse(unicode, System.Globalization.NumberStyles.HexNumber));
        }

        [TestMethod]
        public void unicode_characters_deserialize_should_be_correct()
        {
            var data = unicodeproduce("5A601");
            var res = JsonSerializer.ToObject<string>("\"" + data + "\"");
            Assert.AreEqual(data, res);
        }

        [TestMethod]
        public void IllegalUTF16Char_deserialize_should_be_correct()
        {
            // Ok, this is a pain
            //   There are certain codepoints that are now valid unicode that char.ConvertFromUtf32 can't deal with
            //   What tripped this was \uD83D which is now an emoji, but is considered an illegal surrogate
            //   We have to deal with these somewhat gracefully, even if we can't really turn them into what they
            //   should be...

            var raw = JsonSerializer.ToObject<string>("\"\\uD83D\"");
            Assert.AreEqual(0xD83D, raw[0]);
        }

        [TestMethod]
        public void noncontrol_and_nonunicode_characters_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<string>("\"✿✲❈➹☀☂☁【】┱┲❣✚✪✣✤✥✦❉❥❦❧❃❂❁❀✄\"");
            Assert.AreEqual("✿✲❈➹☀☂☁【】┱┲❣✚✪✣✤✥✦❉❥❦❧❃❂❁❀✄", res);

            res = JsonSerializer.ToObject<string>("\"∑∏∪∩∈∵∴⊥∥∠⌒⊙√∟⊿㏒㏑%‰⅟½⅓⅕⅙\"");
            Assert.AreEqual("∑∏∪∩∈∵∴⊥∥∠⌒⊙√∟⊿㏒㏑%‰⅟½⅓⅕⅙", res);

            res = JsonSerializer.ToObject<string>("\"武汉app\"");
            Assert.AreEqual("武汉app", res);

            res = JsonSerializer.ToObject<string>("\"              \"");
            Assert.AreEqual("              ", res);

            res = JsonSerializer.ToObject<string>("\"1234567890abcdefghijklmnopqrstuvwxyz,.;+-!@#$%^&*()=_ABCDEFGHIJKLMNOPQWRSTUVWXYZ\"");
            Assert.AreEqual("1234567890abcdefghijklmnopqrstuvwxyz,.;+-!@#$%^&*()=_ABCDEFGHIJKLMNOPQWRSTUVWXYZ", res);
        }

        [TestMethod]
        public void TrueString_deserialize_should_be_correct()
        {
            var raw = JsonSerializer.ToObject<bool>("true");
            Assert.IsTrue(raw);
        }

        [TestMethod]
        public void FalseString_deserialize_should_be_correct()
        {
            var raw = JsonSerializer.ToObject<bool>("false");
            Assert.IsFalse(raw);
        }

        [TestMethod]
        public void GuidString_deserialize_should_be_correct()
        {
            var guid = Guid.NewGuid();
            var g = JsonSerializer.ToObject<Guid>("\"" + guid.ToString("d").ToLower() + "\"");
            Assert.AreEqual(guid, g);
        }
    }
}
