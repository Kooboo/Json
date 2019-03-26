using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class DateTimeFormatTest
    {
        [TestMethod]
        public void DateTime_serialize_should_be_correct_format()
        {
            var dt = new DateTime(2014, 08, 08, 14, 04, 01, 426, DateTimeKind.Utc);
            dt = new DateTime(dt.Ticks + 5339, DateTimeKind.Utc);
            var obj = new {Birthday = dt};
            var json = JsonSerializer.ToJson(dt);
            var jsonobj = JsonSerializer.ToJson(obj);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("\"2014-08-08T14:04:01.4265339Z\"", json);
            Assert.IsTrue(JsonValidator.IsValid(jsonobj));
            Assert.AreEqual("{\"Birthday\":\"2014-08-08T14:04:01.4265339Z\"}", jsonobj);
        }


        [TestMethod]
        public void TimeSpans_serialize_should_be_correct_format()
        {
            var rand = new Random();
            var timeSpans = new List<TimeSpan>();

            for (var i = 0; i < 1000; i++)
            {
                var d = rand.Next(10675199 - 1);
                var h = rand.Next(24);
                var m = rand.Next(60);
                var s = rand.Next(60);
                var ms = rand.Next(1000);

                var ts = new TimeSpan(d, h, m, s, ms);
                if (rand.Next(2) == 0)
                {
                    ts = ts.Negate();
                }

                timeSpans.Add(ts);
            }

            timeSpans.Add(TimeSpan.MaxValue);
            timeSpans.Add(TimeSpan.MinValue);
            timeSpans.Add(default(TimeSpan));
            var option = new JsonSerializerOption {TimespanFormat = TimespanFormatEnum.Microsoft};
            foreach (var ts in timeSpans)
            {
                var streamJson = JsonSerializer.ToJson(ts, option);

                var stringJson = JsonSerializer.ToJson(ts, option);

                Assert.IsTrue(JsonValidator.IsValid(streamJson));
                Assert.IsTrue(JsonValidator.IsValid(stringJson));

                Assert.IsTrue(streamJson.StartsWith("\""));
                Assert.IsTrue(streamJson.EndsWith("\""));
                Assert.IsTrue(stringJson.StartsWith("\""));
                Assert.IsTrue(stringJson.EndsWith("\""));

                var dotNetStr = ts.ToString();

                streamJson = streamJson.Trim('"');
                stringJson = stringJson.Trim('"');

                if (dotNetStr.IndexOf('.') != -1) dotNetStr = dotNetStr.TrimEnd('0');
                if (streamJson.IndexOf('.') != -1) streamJson = streamJson.TrimEnd('0');
                if (stringJson.IndexOf('.') != -1) stringJson = stringJson.TrimEnd('0');
                Assert.AreEqual(dotNetStr, streamJson);
                Assert.AreEqual(dotNetStr, stringJson);
            }
        }
    }
}
