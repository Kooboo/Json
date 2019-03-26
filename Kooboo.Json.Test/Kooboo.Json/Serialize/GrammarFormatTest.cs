using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    public class BoolPoco
    {
        public bool Val { get; set; }
        public int Id { get; set; }
    }

    public class SimplePoco
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class Cyclical
    {
        public int Foo;

        public Cyclical Next;
    }

    //    2.  JSON Grammar

    //A JSON text is a sequence of tokens.The set of tokens includes six
    //structural characters, strings, numbers, and three literal names.

    //A JSON text is a serialized value.Note that certain previous
    //specifications of JSON constrained a JSON text to be an object or an



    //Bray                         Standards Track                    [Page 4]

    //RFC 7159                          JSON March 2014


    //array.Implementations that generate only objects or arrays where a
    //JSON text is called for will be interoperable in the sense that all
    //implementations will accept these as conforming JSON texts.

    //JSON-text = ws value ws

    //These are the six structural characters:

    //begin-array     = ws % x5B ws  ;[left square bracket

    //begin-object    = ws %x7B ws; { left curly bracket

    //    end-array       = ws %x5D ws; ] right square bracket

    //    end-object      = ws %x7D ws;
    //}
    //right curly bracket

    //name-separator  = ws %x3A ws; : colon

    //value-separator = ws %x2C ws; , comma

    //Insignificant whitespace is allowed before or after any of the six
    //structural characters.

    //ws = *(
    //%x20 /              ; Space
    //%x09 /              ; Horizontal tab
    //%x0A /              ; Line feed or New line
    //%x0D )              ; Carriage return

    [TestClass]
    public class GrammarFormatTest
    {
        [TestMethod]
        public void Text_should_be_correct_format()
        {
            //JSON-text = ws value ws
            var json = JsonSerializer.ToJson("normal text");
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"normal text\"", json);

            //%x20 /              ; Space
            json = JsonSerializer.ToJson("  normal text  ");
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"  normal text  \"", json);

            //%x09 /              ; Horizontal tab
            json = JsonSerializer.ToJson("  normal text ");
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"  normal text \"", json);

            //%x0A /              ; Line feed or New line
            json = JsonSerializer.ToJson(@"
            normal text
            ");
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\r\\n            normal text\\r\\n            \"", json);

            //%x0D )              ; Carriage return
            json = JsonSerializer.ToJson(@"
normal text");
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"\\r\\nnormal text\"", json);
        }

        //begin-array     = ws % x5B ws  ;[left square bracket
        //    end-array       = ws %x5D ws; ] right square bracket
        [TestMethod]
        public void Array_should_be_correct_format()
        {
            var json = JsonSerializer.ToJson(new List<int> { 1,0,-1 });
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[1,0,-1]", json);

            //begin-array     = ws % x5B ws  ;[left square bracket
            Assert.IsTrue(json.StartsWith("["));

            //end-array       = ws %x5D ws; ] right square bracket
            Assert.IsTrue(json.EndsWith("]"));
        }

        //begin-object    = ws %x7B ws; { left curly bracket

        //    end-object      = ws %x7D ws;
        //}
        //right curly bracket

        //name-separator  = ws %x3A ws; : colon

        //value-separator = ws %x2C ws; , comma
        [TestMethod]
        public void Object_should_be_correct_format()
        {
            var data = new SimplePoco
            {
                Name = "abc",
                Id = 999
            };
            var json = JsonSerializer.ToJson(data);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("{\"Name\":\"abc\",\"Id\":999}", json);

            //begin-object    = ws %x7B ws; { left curly bracket
            Assert.IsTrue(json.StartsWith("{"));

            //end-array       = ws %x7D ws;} right curly bracket
            Assert.IsTrue(json.EndsWith("}"));

            //name-separator  = ws %x3A ws; : colon
            //value - separator = ws % x2C ws; , comma
            var splitarray = json.Split('\"');
            Assert.AreEqual(":", splitarray[2]);
            Assert.AreEqual(",", splitarray[4]);
        }
    }
}
