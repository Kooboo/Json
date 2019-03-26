using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Json;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class DateTimeDeserializeTest
    {
        [TestMethod]
        public void DateTimeString_deserialize_should_be_correct()
        {
            var str = "\"1989-01-31\"";
            var res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 0, 0, 0, DateTimeKind.Local),res);
        }

        [TestMethod]
        public void DateTimeString_no_dash_deserialize_should_be_correct()
        {
            var str = "\"19890131\"";
            var res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 0, 0, 0, DateTimeKind.Local), res);

            str = "\"19890131T12,5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 30, 0, DateTimeKind.Local), res);

            str = "\"19890131T12.5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 30, 0, DateTimeKind.Local), res);

            str = "\"19890131T12:34\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 0, DateTimeKind.Local), res);

            str = "\"19890131T12:34:56\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 56, DateTimeKind.Local), res);

            str = "\"19890131T12:34:56,5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 56, 500, DateTimeKind.Local), res);

            str = "\"19890131T12:34:56.5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 56, 500, DateTimeKind.Local), res);

            str = "\"2004366\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(2004, 12, 31, 0, 0, 0, DateTimeKind.Local), res);

            str = "\"19890131T12.5+0123\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            var shouldMatch = new DateTimeOffset(1989, 01, 31, 12, 30, 0, new TimeSpan(01, 23, 00));
            Assert.AreEqual(shouldMatch.UtcDateTime, res);
        }

        [TestMethod]
        public void DateTimeString_with_dash_deserialize_should_be_correct()
        {
            var str = "\"1989-01-31T12\"";
            var res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 0, 0, DateTimeKind.Local), res);

            str = "\"1989-01-31T12,5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 30, 0, DateTimeKind.Local), res);

            str = "\"1989-01-31T12.5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 30, 0, DateTimeKind.Local), res);

            str = "\"1989-01-31T12:34\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 0, DateTimeKind.Local), res);

            str = "\"1989-01-31T12:34:56\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 56, DateTimeKind.Local), res);

            str = "\"1989-01-31T12:34:56,5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 56, 500, DateTimeKind.Local), res);

            str = "\"1989-01-31T12:34:56.5\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1989, 01, 31, 12, 34, 56, 500, DateTimeKind.Local), res);

            str = "\"1989-01-31T12-11:45\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            var shouldMatch = new DateTimeOffset(1989, 01, 31, 12, 0, 0, new TimeSpan(11, 45, 0).Negate());
            Assert.AreEqual(shouldMatch.UtcDateTime, res);

            str = "\"1900-01-01 12:30z\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1900, 01, 01, 12, 30, 0, DateTimeKind.Utc), res);

            str = "\"1900-01-01 12:30z\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(1900, 01, 01, 12, 30, 0, DateTimeKind.Utc), res);

            str = "\"2004-366\"";
            res = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(new DateTime(2004, 12, 31, 0, 0, 0, DateTimeKind.Local), res);
        }

        [TestMethod]
        public void ISO8601TimeSpan_deserialize_should_be_correct()
        {
            var rand = new Random();
            var timeSpans = new List<string>();

            for (var i = 0; i < 1000; i++)
            {
                var y = rand.Next(10000);
                var m = rand.Next(100);

                var str = "P" + y + "Y" + m + "M";
                if (rand.Next(2) == 0)
                {
                    str = "-" + str;
                }

                timeSpans.Add(str);
            }

            foreach (var str in timeSpans)
            {
                var shouldMatch = System.Xml.XmlConvert.ToTimeSpan(str);
                var ts = JsonSerializer.ToObject<TimeSpan>("\"" + str + "\"");

                Assert.AreEqual(shouldMatch.Ticks, ts.Ticks);
            }
        }

        [TestMethod]
        public void ISO8601TimeSpan_Weeks_deserialize_should_be_correct()
        {
            var rand = new Random();
            var timeSpans = new List<Tuple<int, string>>();

            for (var i = 0; i < 1000; i++)
            {
                var w = rand.Next(10000);

                var str = "P" + w + "W";
                if (rand.Next(2) == 0)
                {
                    w = -w;
                    str = "-" + str;
                }

                timeSpans.Add(Tuple.Create(w, str));
            }

            foreach (var t in timeSpans)
            {
                var w = t.Item1;
                var str = t.Item2;
                var ts = JsonSerializer.ToObject<TimeSpan>("\"" + str + "\"");

                Assert.AreEqual(w, ts.TotalDays / 7);
            }
        }

        [TestMethod]
        public void MicrosoftTimeSpan_deserialize_should_be_correct()
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
            timeSpans.AddRange(Enumerable.Range(1, 1000).Select(n => new TimeSpan(TimeSpan.MaxValue.Ticks - n)));
            timeSpans.Add(TimeSpan.MinValue);
            timeSpans.AddRange(Enumerable.Range(1, 1000).Select(n => new TimeSpan(TimeSpan.MinValue.Ticks + n)));
            timeSpans.Add(default(TimeSpan));

            foreach (var ts1 in timeSpans)
            {
                var json = JsonSerializer.ToJson(ts1);
                var ts2 = JsonSerializer.ToObject<TimeSpan>(json);
                Assert.AreEqual(Math.Round(ts1.TotalMilliseconds), Math.Round(ts2.TotalMilliseconds));
                Assert.AreEqual(Math.Round(ts1.TotalSeconds), Math.Round(ts2.TotalSeconds));
            }
        }

        [TestMethod]
        public void DateTimeFraction_deserialize_should_be_correct()
        {
            var date = new DateTime(21, DateTimeKind.Utc);
            const string str = "\"0001-01-01T00:00:00.0000021001Z\"";

            var result = JsonSerializer.ToObject<DateTime>(str);
            Assert.AreEqual(date.Ticks, result.Ticks);
        }

        //[TestMethod]
        //public void DateTimeOffset_deserialize_should_be_correct()
        //{
        //    var offset = new DateTimeOffset(new DateTime(21, DateTimeKind.Utc), TimeSpan.Zero);
        //    var str = JsonSerializer.ToJson(offset);

        //    var result = JsonSerializer.ToObject<DateTimeOffset>(str);
        //    Assert.AreEqual(offset.Ticks, result.Ticks);
        //}

        [TestMethod]
        public void Special_date_format_deserialize_should_be_correct()
        {
            string jsonDate = "\"\\/Date(1490452166591)\\/\"";
            var date= Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(jsonDate);
            var result = JsonSerializer.ToObject<DateTime>(jsonDate);
            Assert.AreEqual(date, result);
        }
    }
}
