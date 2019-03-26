using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    //    7.  Strings

    //   The representation of strings is similar to conventions used in the C
    //   family of programming languages.A string begins and ends with
    //   quotation marks.All Unicode characters may be placed within the
    //   quotation marks, except for the characters that must be escaped:
    //   quotation mark, reverse solidus, and the control characters (U+0000
    //   through U+001F).

    //   Any character may be escaped.If the character is in the Basic
    //   Multilingual Plane(U+0000 through U+FFFF), then it may be
    //   represented as a six-character sequence: a reverse solidus, followed
    //   by the lowercase letter u, followed by four hexadecimal digits that
    //   encode the character's code point.  The hexadecimal letters A though
    //   F can be upper or lower case.  So, for example, a string containing
    //   only a single reverse solidus character may be represented as
    //   "\u005C".

    //   Alternatively, there are two-character sequence escape
    //   representations of some popular characters.So, for example, a
    //   string containing only a single reverse solidus character may be
    //   represented more compactly as "\\".

    //   To escape an extended character that is not in the Basic Multilingual
    //   Plane, the character is represented as a 12-character sequence,
    //   encoding the UTF-16 surrogate pair.  So, for example, a string
    //   containing only the G clef character (U+1D11E) may be represented as
    //   "\uD834\uDD1E".

    //      string = quotation-mark* char quotation-mark

    //      char = unescaped /
    //          escape(
    //              %x22 /          ; "    quotation mark  U+0022
    //              %x5C /          ; \    reverse solidus U+005C
    //              %x2F /          ; /    solidus U+002F
    //              %x62 /          ; b backspace       U+0008
    //              %x66 /          ; f form feed U+000C
    //              %x6E /          ; n line feed U+000A
    //              %x72 /          ; r carriage return U+000D
    //              %x74 /          ; t tab             U+0009
    //              %x75 4HEXDIG )  ; uXXXX U+XXXX

    //escape = % x5C; \

    //      quotation-mark = %x22      ; "

    //      unescaped = %x20-21 / %x23-5B / %x5D-10FFFF




    //Bray                         Standards Track[Page 8]


    //RFC 7159                          JSON March 2014

    [TestClass]
    public class StringFormatTest
    {
        [TestMethod]
        public void Control_and_unicode_characters_serialize_should_be_surround_with_quotation_marks_and_need_escape()
        {
            //   The representation of strings is similar to conventions used in the C
            //   family of programming languages.A string begins and ends with
            //   quotation marks.All Unicode characters may be placed within the
            //   quotation marks, except for the characters that must be escaped:
            //   quotation mark, reverse solidus, and the control characters (U+0000
            //   through U+001F).

            var str = "\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\u000A\u000B\u000C\u000D\u000E\u000F";
            var json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\u0000\\u0001\\u0002\\u0003\\u0004\\u0005\\u0006\\u0007\\b\\t\\n\\u000b\\f\\r\\u000e\\u000f\"", json);

            str = "\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\u0010\\u0011\\u0012\\u0013\\u0014\\u0015\\u0016\\u0017\\u0018\\u0019\\u001a\\u001b\\u001c\\u001d\\u001e\\u001f\"", json);

            //   Any character may be escaped.If the character is in the Basic
            //   Multilingual Plane(U+0000 through U+FFFF), then it may be
            //   represented as a six-character sequence: a reverse solidus, followed
            //   by the lowercase letter u, followed by four hexadecimal digits that
            //   encode the character's code point.  The hexadecimal letters A though
            //   F can be upper or lower case.  So, for example, a string containing
            //   only a single reverse solidus character may be represented as
            //   "\u005C".

            str = "\u005C\u005D\u005E\u005F";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\\\]^_\"", json);

            str = "\u8899\u7FFF";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"袙翿\"", json);

            //   Alternatively, there are two-character sequence escape
            //   representations of some popular characters.So, for example, a
            //   string containing only a single reverse solidus character may be
            //   represented more compactly as "\\".
            str = "\\";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\\\\"", json);

            str = "\"";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\\"\"", json);


            str = "hello\b\f\r\n\tworld";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"hello\\b\\f\\r\\n\\tworld\"", json);

            //   To escape an extended character that is not in the Basic Multilingual
            //   Plane, the character is represented as a 12-character sequence,
            //   encoding the UTF-16 surrogate pair.  So, for example, a string
            //   containing only the G clef character (U+1D11E) may be represented as
            //   "\uD834\uDD1E"

            str = "\uD834\uDD1E";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
        }

        [TestMethod]
        public void Noncontrol_and_Nonunicode_characters_serialize_should_be_surround_with_quotation_marks_and_not_need_escape()
        {

            //      string = quotation-mark* char quotation-mark
            var str = "1234567890abcdefghijklmnopqrstuvwxyz,.;+-!@#$%^&*()=_ABCDEFGHIJKLMNOPQWRSTUVWXYZ";
            var json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"1234567890abcdefghijklmnopqrstuvwxyz,.;+-!@#$%^&*()=_ABCDEFGHIJKLMNOPQWRSTUVWXYZ\"", json);

            str = "";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\"", json);

            str = "              ";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"              \"", json);

            str = "武汉app";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"武汉app\"", json);

            str = "∑∏∪∩∈∵∴⊥∥∠⌒⊙√∟⊿㏒㏑%‰⅟½⅓⅕⅙";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"∑∏∪∩∈∵∴⊥∥∠⌒⊙√∟⊿㏒㏑%‰⅟½⅓⅕⅙\"", json);

            str = "✿✲❈➹☀☂☁【】┱┲❣✚✪✣✤✥✦❉❥❦❧❃❂❁❀✄";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"✿✲❈➹☀☂☁【】┱┲❣✚✪✣✤✥✦❉❥❦❧❃❂❁❀✄\"", json);

            //      string = quotation-mark* char quotation-mark

            //      char = unescaped /
            //          escape(
            //              %x22 /          ; "    quotation mark  U+0022
            //              %x5C /          ; \    reverse solidus U+005C
            //              %x2F /          ; /    solidus U+002F
            //              %x62 /          ; b backspace       U+0008
            //              %x66 /          ; f form feed U+000C
            //              %x6E /          ; n line feed U+000A
            //              %x72 /          ; r carriage return U+000D
            //              %x74 /          ; t tab             U+0009
            //              %x75 4HEXDIG )  ; uXXXX U+XXXX

            str = "\"sup\b\t\f\n\r\0\"/\\\u2028\u2029";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\\"sup\\b\\t\\f\\n\\r\\u0000\\\"/\\\\\\u2028\\u2029\"", json);

            //      unescaped = %x20-21 / %x23-5B / %x5D-10FFFF
            str = "\u0020\u0021";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\" !\"", json);

            str = "\u0023\u005B";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"#[\"", json);

            str = "\u005D\u006B\u10FFF";
            json = JsonSerializer.ToJson(str);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"]kჿF\"", json);
        }
    }
}
