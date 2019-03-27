using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using JsonValidatorTool;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class DataTableFormatTest
    {
        [TestMethod]
        public void DataTable_serialize_should_be_correct_format()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PackageId", typeof(string));
            dt.Columns.Add("Version", typeof(string));
            dt.Columns.Add("ReleaseDate", typeof(DateTime));

            dt.Rows.Add("Kooboo", "11.0.1", new DateTime(2018, 2, 17));
            dt.Rows.Add("Kooboo", "10.0.3", new DateTime(2017, 6, 18));

            string json = JsonSerializer.ToJson(dt);
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"PackageId\":\"Kooboo\",\"Version\":\"11.0.1\",\"ReleaseDate\":\"2018-02-17T00:00:00Z\"},{\"PackageId\":\"Kooboo\",\"Version\":\"10.0.3\",\"ReleaseDate\":\"2017-06-18T00:00:00Z\"}]", json);
        }

        [TestMethod]
        public void DataTable_with_nullvalue_serialize_should_be_correct_format()
        {
            DataTable dt = new DataTable();
            List<Type> types = new List<Type>
            {
                typeof(TimeSpan),
                typeof(char[]),
                typeof(Type),
                typeof(object),
                typeof(byte[]),
                typeof(Uri),
                typeof(Guid)
            };

            foreach (var ss in types)
            {
                dt.Columns.Add(ss.Name, ss);
            }

            dt.Rows.Add(types.Select(t => (object)null).ToArray());

            var json = JsonSerializer.ToJson(dt);
            Assert.IsTrue(JsonValidator.IsValid(json));
           Assert.AreEqual("[{\"TimeSpan\":null,\"Char[]\":null,\"Type\":null,\"Object\":null,\"Byte[]\":null,\"Uri\":null,\"Guid\":null}]", json);
        }

        [TestMethod]
        public void DataTable_with_value_serialize_should_be_correct_format()
        {
            DataTable dt = new DataTable();
            Dictionary<Type, object> types = new Dictionary<Type, object>
            {
                [typeof(TimeSpan)] = TimeSpan.Zero,
                [typeof(char[])] = new char[] { 'a', 'b', 'c' },
                [typeof(Type)] = typeof(string),
                [typeof(object)] = new object(),
                [typeof(byte[])] = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                [typeof(Uri)] = new Uri("http://localhost"),
                [typeof(Guid)] = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)
            };

            foreach (var ss in types)
            {
                dt.Columns.Add(ss.Key.Name, ss.Key);
            }

            dt.Rows.Add(types.Select(t => t.Value).ToArray());

            var json = JsonSerializer.ToJson(dt,new JsonSerializerOption(){TimespanFormat=TimespanFormatEnum.Microsoft});
            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("[{\"TimeSpan\":\"00:00:00\",\"Char[]\":[\"a\",\"b\",\"c\"],\"Type\":\"System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\"Object\":{},\"Byte[]\":[1,2,3,4,5,6,7,8],\"Uri\":\"http://localhost\",\"Guid\":\"00000001-0002-0003-0405-060708090a0b\"}]", json);
        }
    }
}
