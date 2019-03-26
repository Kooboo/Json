using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;
using RichardSzalay.MockHttp;

namespace Kooboo.Json.Test
{
    //    11.  IANA Considerations

    //    The MIME media type for JSON text is application/json.

    //    Type name:  application

    //    Subtype name:  json

    //    Required parameters:  n/a

    //    Optional parameters:  n/a

    //    Encoding considerations:  binary

    //    Security considerations:  See[RFC7159], Section 12.

    //Interoperability considerations:  Described in [RFC7159]

    //Published specification:  [RFC7159]

    //Applications that use this media type:

    //JSON has been used to exchange data between applications written
     
    //in all of these programming languages: ActionScript, C, C#,
    //Clojure, ColdFusion, Common Lisp, E, Erlang, Go, Java, JavaScript,
    //Lua, Objective CAML, Perl, PHP, Python, Rebol, Ruby, Scala, and
    //Scheme.

    //Bray Standards Track[Page 10]

    //RFC 7159                          JSON March 2014

    //Additional information:

    //Magic number(s): n/a
    //File extension(s): .json
    //Macintosh file type code(s): TEXT

    //Person & email address to contact for further information:

    //IESG
    //<iesg@ietf.org>

    //Intended usage:  COMMON

    //Restrictions on usage:  none

    //Author:

    //Douglas Crockford
    //<douglas @crockford.com>


    //Change controller:

    //IESG
    //<iesg@ietf.org>

    //Note:  No "charset" parameter is defined for this registration.
    //Adding one really has no effect on compliant recipients.
 

     [TestClass]
    public class IANAIssueTest
    {
        //    The MIME media type for JSON text is application/json.
        //Applications that use this media type:

        //JSON has been used to exchange data between applications written

        //in all of these programming languages: ActionScript, C, C#,
        //Clojure, ColdFusion, Common Lisp, E, Erlang, Go, Java, JavaScript,
        //Lua, Objective CAML, Perl, PHP, Python, Rebol, Ruby, Scala, and
        //Scheme.

        [TestMethod]
        public async Task Json_text_use_json_mime_and_use_get_request_should_be_exchange_data_correctly()
        {
            var json1 = JsonSerializer.ToJson(new { Email = "san@abc.com", Name = "san" });
            Assert.IsTrue(JsonValidator.IsValid(json1));
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/user/*").Respond("application/json", json1);
            var client = mockHttp.ToHttpClient();
            var response = await client.GetAsync("http://localhost/api/user/1234");
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
            var json2 = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(JsonValidator.IsValid(json2));
            Assert.AreEqual(json1, json2);
        }

        [TestMethod]
        public async Task Json_text_use_json_mime_and_use_post_request_should_be_exchange_data_correctly()
        {
            var json1 = JsonSerializer.ToJson(new { Email = "san@abc.com", Name = "san" });
            Assert.IsTrue(JsonValidator.IsValid(json1));
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/user/*").Respond("application/json", json1);
            var client = mockHttp.ToHttpClient();
            var response = await client.PostAsync("http://localhost/api/user/1234", null);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
            var json2 = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(JsonValidator.IsValid(json2));
            Assert.AreEqual(json1, json2);
        }
    }
}
