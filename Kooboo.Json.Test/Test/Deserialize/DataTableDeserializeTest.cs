using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System;

namespace Kooboo.Json.Test
{

    [TestClass]
    public class DataTableDeserializeTest
    {
        [TestMethod]
        public void DataTable_simple_deserialize_should_be_correct()
        {
            var str = "[{\"PackageId\":\"Kooboo\",\"Version\":\"11.0.1\",\"ReleaseDate\":\"2018-02-17T00:00:00Z\"},{\"PackageId\":\"Kooboo\",\"Version\":\"10.0.3\",\"ReleaseDate\":\"2017-06-18T00:00:00Z\"}]";
            var res = JsonSerializer.ToObject<DataTable>(str);
            Assert.AreEqual(2, res.Rows.Count);
            Assert.AreEqual("Kooboo", res.Rows[0][0]);
            Assert.AreEqual("11.0.1", res.Rows[0][1]);
            Assert.AreEqual("2018-02-17T00:00:00Z", res.Rows[0][2]);
        }

        [TestMethod]
        public void DataTable_dbnull_deserialize_should_be_correct()
        {
            const string json = @"["
                                + @"{""item"":""shirt"",""price"":49.99},"
                                + @"{""item"":""pants"",""price"":54.99},"
                                + @"{""item"":""shoes"",""price"":null}]";
            var table = JsonSerializer.ToObject<DataTable>(json);
            Assert.AreEqual("shirt", table.Rows[0]["item"]);
            Assert.AreEqual("pants", table.Rows[1]["item"]);
            Assert.AreEqual("shoes", table.Rows[2]["item"]);
            Assert.AreEqual(49.99, (double)table.Rows[0]["price"], 0.01);
            Assert.AreEqual(54.99, (double)table.Rows[1]["price"], 0.01);
            Assert.AreEqual(DBNull.Value, table.Rows[2]["price"]);
        }

        [TestMethod]
        public void DataTable_complex_deserialize_should_be_correct()
        {
            var json = @"[
  {
    ""id"": 0,
    ""item"": ""item 0"",
    ""DataTableCol"": [
      {
        ""NestedStringCol"": ""0!""
      }
    ],
    ""ArrayCol"": [
      0
    ],
    ""DateCol"": ""2000-12-29T00:00:00Z""
  },
  {
    ""id"": 1,
    ""item"": ""item 1"",
    ""DataTableCol"": [
      {
        ""NestedStringCol"": ""1!""
      }
    ],
    ""ArrayCol"": [
      1
    ],
    ""DateCol"": ""2000-12-29T00:00:00Z""
  }
]";

            DataTable deserializedDataTable = JsonSerializer.ToObject<DataTable>(json);
            Assert.IsNotNull(deserializedDataTable);

            Assert.AreEqual(string.Empty, deserializedDataTable.TableName);
            Assert.AreEqual(5, deserializedDataTable.Columns.Count);
            Assert.AreEqual("id", deserializedDataTable.Columns[0].ColumnName);
            //Assert.AreEqual(typeof(long), deserializedDataTable.Columns[0].DataType);
            Assert.AreEqual("item", deserializedDataTable.Columns[1].ColumnName);
            //Assert.AreEqual(typeof(string), deserializedDataTable.Columns[1].DataType);
            Assert.AreEqual("DataTableCol", deserializedDataTable.Columns[2].ColumnName);
            //Assert.AreEqual(typeof(DataTable), deserializedDataTable.Columns[2].DataType);
            Assert.AreEqual("ArrayCol", deserializedDataTable.Columns[3].ColumnName);
            //Assert.AreEqual(typeof(long[]), deserializedDataTable.Columns[3].DataType);
            Assert.AreEqual("DateCol", deserializedDataTable.Columns[4].ColumnName);
            //Assert.AreEqual(typeof(DateTime), deserializedDataTable.Columns[4].DataType);

            Assert.AreEqual(2, deserializedDataTable.Rows.Count);
        }
    }
}
