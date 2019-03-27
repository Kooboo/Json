using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    //    8.  String and Character Issues

    //8.1.  Character Encoding

    //   JSON text SHALL be encoded in UTF-8, UTF-16, or UTF-32.  The default
    //   encoding is UTF-8, and JSON texts that are encoded in UTF-8 are
    //   interoperable in the sense that they will be read successfully by the
    //   maximum number of implementations; there are many implementations
    //   that cannot successfully read texts in other encodings(such as
    //   UTF-16 and UTF-32).

    //   Implementations MUST NOT add a byte order mark to the beginning of a
    //   JSON text.In the interests of interoperability, implementations
    //   that parse JSON texts MAY ignore the presence of a byte order mark
    //   rather than treating it as an error.

    //8.2.  Unicode Characters

    //   When all the strings represented in a JSON text are composed entirely
    //   of Unicode characters[UNICODE] (however escaped), then that JSON
    //   text is interoperable in the sense that all software implementations
    //   that parse it will agree on the contents of names and of string
    //   values in objects and arrays.

    //   However, the ABNF in this specification allows member names and
    //   string values to contain bit sequences that cannot encode Unicode
    //   characters; for example, "\uDEAD" (a single unpaired UTF-16
    //   surrogate).  Instances of this have been observed, for example, when
    //   a library truncates a UTF-16 string without checking whether the
    //   truncation split a surrogate pair.  The behavior of software that
    //   receives JSON texts containing such values is unpredictable; for
    //   example, implementations might return different values for the length
    //   of a string value or even suffer fatal runtime exceptions.

    //8.3.  String Comparison

    //   Software implementations are typically required to test names of
    //   object members for equality.Implementations that transform the
    //   textual representation into sequences of Unicode code units and then
    //   perform the comparison numerically, code unit by code unit, are
    //   interoperable in the sense that implementations will agree in all
    //   cases on equality or inequality of two strings.For example,
    //   implementations that compare strings with escaped characters
    //   unconverted may incorrectly find that "a\\b" and "a\u005Cb" are not
    //   equal.

    //Bray Standards Track[Page 9]

    //RFC 7159                          JSON March 2014

    [TestClass]
    public class StringAndCharacterIssuesTest
    {
        //   JSON text SHALL be encoded in UTF-8, UTF-16, or UTF-32.  The default
        //   encoding is UTF-8, and JSON texts that are encoded in UTF-8 are
        //   interoperable in the sense that they will be read successfully by the
        //   maximum number of implementations; there are many implementations
        //   that cannot successfully read texts in other encodings(such as
        //   UTF-16 and UTF-32).
        [TestMethod]
        public void Json_text_with_different_encoding_should_be_interoperated()
        {
            var json = JsonSerializer.ToJson(new { Email = "san@abc.com", Name = "san" });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Email\":\"san@abc.com\",\"Name\":\"san\"}", json);

            //encoding without bom
            var utf32 = new UTF32Encoding(false, false);
            var utf16 = new UnicodeEncoding(false, false);
            var utf8 = new UTF8Encoding(false);
            var utf32Bytes = utf32.GetBytes(json);
            var utf16Bytes = utf16.GetBytes(json);

            //Direct reading of other byte streams, failed
            var utf32String = utf8.GetString(utf32Bytes);
            var utf16String = utf8.GetString(utf16Bytes);
            Assert.AreNotEqual(utf32String, json);
            Assert.AreNotEqual(utf16String, json);

            //Convert byte stream, successful
            var utf8Bytes = Encoding.Convert(utf16, utf8, utf16Bytes);
            utf16String = utf8.GetString(utf8Bytes);
            Assert.AreEqual(json, utf16String);

            utf8Bytes = Encoding.Convert(utf32, utf8, utf32Bytes);
            utf32String = utf8.GetString(utf8Bytes);
            Assert.AreEqual(json, utf32String);
        }

        //   Implementations MUST NOT add a byte order mark to the beginning of a
        //   JSON text.In the interests of interoperability, implementations
        //   that parse JSON texts MAY ignore the presence of a byte order mark
        //   rather than treating it as an error.

        public static byte[] GetBytesWithoutBom(string path, Encoding enc)
        {
            return File.ReadAllBytes(path).Skip(enc.GetPreamble().Length).ToArray();
        }

        [TestMethod]
        public void Json_text_encoding_should_be_without_bom_and_ignore_bom_when_parse()
        {
            var json = JsonSerializer.ToJson(new { Email = "san@abc.com", Name = "san" });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Email\":\"san@abc.com\",\"Name\":\"san\"}", json);

            //write file without bom
            File.WriteAllText("nobom.json", json, new UTF8Encoding(false));
            //write file with bom
            File.WriteAllText("bom.json", json, Encoding.UTF8);

            //read bom file then ignore bom
            var nobom = Encoding.UTF8.GetString(GetBytesWithoutBom("bom.json", Encoding.UTF8));
            Assert.AreEqual(json, nobom);

            //read no bom file
            nobom = File.ReadAllText("nobom.json", Encoding.UTF8);
            Assert.AreEqual(json, nobom);
        }

        //   When all the strings represented in a JSON text are composed entirely
        //   of Unicode characters[UNICODE] (however escaped), then that JSON
        //   text is interoperable in the sense that all software implementations
        //   that parse it will agree on the contents of names and of string
        //   values in objects and arrays.
        [TestMethod]
        public void Json_text_with_unicode_entirely_should_be_parse_interoperable_in_all_implementations()
        {
            var objs = new List<SimplePoco>
            {
                new SimplePoco {Name = "\u0098\u0099", Id = 3},
                new SimplePoco {Name = "\u1f33\u1f32", Id = 2},
                new SimplePoco {Name = "\u5fee\u5fef", Id = 1}
            };
            var json = JsonSerializer.ToJson(objs);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"Name\":\"\u0098\u0099\",\"Id\":3},{\"Name\":\"ἳἲ\",\"Id\":2},{\"Name\":\"忮忯\",\"Id\":1}]", json);

            var newobjs = JsonSerializer.ToObject<List<SimplePoco>>(json);

            Assert.IsTrue(objs[0].Name== newobjs[0].Name);
            Assert.IsTrue(objs[1].Name == newobjs[1].Name);
            Assert.IsTrue(objs[2].Name == newobjs[2].Name);
        }

        //   However, the ABNF in this specification allows member names and
        //   string values to contain bit sequences that cannot encode Unicode
        //   characters; for example, "\uDEAD" (a single unpaired UTF-16
        //   surrogate).  Instances of this have been observed, for example, when
        //   a library truncates a UTF-16 string without checking whether the
        //   truncation split a surrogate pair.  The behavior of software that
        //   receives JSON texts containing such values is unpredictable; for
        //   example, implementations might return different values for the length
        //   of a string value or even suffer fatal runtime exceptions.

        [TestMethod]
        public void Json_text_with_unpaired_uft16_should_be_become_unpredictable()
        {
            var json = JsonSerializer.ToJson(new { Email = "\uDEAD", Name = "\uDEAD" });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Email\":\"\udead\",\"Name\":\"\udead\"}", json);
            var utf16 = new UnicodeEncoding(false, false);
            var utf8 = new UTF8Encoding(false);
            var utf16Bytes = utf16.GetBytes(json);
            var utf16String = utf8.GetString(utf16Bytes);
            Assert.IsTrue(utf16String.Length != json.Length);
        }

        //   Software implementations are typically required to test names of
        //   object members for equality.Implementations that transform the
        //   textual representation into sequences of Unicode code units and then
        //   perform the comparison numerically, code unit by code unit, are
        //   interoperable in the sense that implementations will agree in all
        //   cases on equality or inequality of two strings.For example,
        //   implementations that compare strings with escaped characters
        //   unconverted may incorrectly find that "a\\b" and "a\u005Cb" are not
        //   equal.
        [TestMethod]
        public void Json_text_escaped_unicode_and_origin_compare_should_be_equal()
        {
            var json1 = JsonSerializer.ToJson(new { Email = "a\\b" });
            var json2 = JsonSerializer.ToJson(new { Email = "a\u005Cb" });
            Assert.IsTrue(JsonValidator.IsValid(json1));
            Assert.IsTrue(JsonValidator.IsValid(json2));
            Assert.AreEqual(json1,json2);
        }
    }
}
