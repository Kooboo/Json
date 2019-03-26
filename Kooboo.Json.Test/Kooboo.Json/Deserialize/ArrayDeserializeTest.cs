using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using JsonValidatorTool;
using Kooboo.Json;
using Kooboo.Json.Deserialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class ArrayDeserializeTest
    {
        [TestMethod]
        public void Normal_array_deserialize_should_be_correct()
        {
            var ret = JsonSerializer.ToObject<int[]>("[0,1,2,3,4,5]");
            Assert.AreEqual(6, ret.Length);
            Assert.AreEqual(0, ret[0]);
            Assert.AreEqual(1, ret[1]);
            Assert.AreEqual(2, ret[2]);
            Assert.AreEqual(3, ret[3]);
            Assert.AreEqual(4, ret[4]);
            Assert.AreEqual(5, ret[5]);
        }

        [TestMethod]
        public void OneMember_array_deserialize_should_be_correct()
        {
            var ret = JsonSerializer.ToObject<int[]>("[0]");
            Assert.AreEqual(1, ret.Length);
            Assert.AreEqual(0, ret[0]);
        }

        [TestMethod]
        public void ListString_with_space_array_deserialize_should_be_correct()
        {
            var val = JsonSerializer.ToObject<List<int>>(" [ 1,2 ,3   ]    ");
            Assert.AreEqual(3, val.Count);
            Assert.AreEqual(1, val[0]);
            Assert.AreEqual(2, val[1]);
            Assert.AreEqual(3, val[2]);
        }

        [TestMethod]
        public void NullString_array_deserialize_should_be_correct()
        {
            var ret = JsonSerializer.ToObject<int[]>("null");
            Assert.IsNull(ret);
        }

        [TestMethod]
        public void EmptyString_array_deserialize_should_be_correct()
        {
            var ret = JsonSerializer.ToObject<int[]>("[]");
            Assert.AreEqual(0, ret.Length);
        }

        [TestMethod]
        public void EmptyString_with_space_array_deserialize_should_be_correct()
        {
            var ret = JsonSerializer.ToObject<int[]>("  [    ]  ");
            Assert.AreEqual(0, ret.Length);
        }

        class _ILists
        {
            public IList<byte> Bytes;
            public IList<IList<int>> IntsOfInts;
        }

        class _IReadOnlyLists
        {
            public IReadOnlyList<byte> Bytes;
            public IReadOnlyList<IReadOnlyList<int>> IntsOfInts;
        }

        class _IEnumerableMember
        {
            public IEnumerable<string> A { get; set; }
        }

        class _IReadOnlyListMember
        {
            public IReadOnlyList<string> A { get; set; }
        }

        class _ISets
        {
            public ISet<int> IntSet;
            public SortedSet<int> IntSortedSet;
            public HashSet<string> StringHashSet;
        }

        [TestMethod]
        public void IList_array_deserialize_should_be_correct()
        {
            var c = JsonSerializer.ToObject<IList<byte>>("[255,0,128]");

            var res = JsonSerializer.ToObject<_ILists>(
                "{\"Bytes\":[255,0,128],\"IntsOfInts\":[[1,2,3],[4,5,6],[7,8,9]]}");
            Assert.IsNotNull(res);

            Assert.IsNotNull(res.Bytes);
            Assert.AreEqual(3, res.Bytes.Count);
            Assert.AreEqual(255, res.Bytes[0]);
            Assert.AreEqual(0, res.Bytes[1]);
            Assert.AreEqual(128, res.Bytes[2]);

            Assert.IsNotNull(res.IntsOfInts);
            Assert.AreEqual(3, res.IntsOfInts.Count);
            Assert.IsNotNull(res.IntsOfInts[0]);
            Assert.AreEqual(3, res.IntsOfInts[0].Count);
            Assert.AreEqual(1, res.IntsOfInts[0][0]);
            Assert.AreEqual(2, res.IntsOfInts[0][1]);
            Assert.AreEqual(3, res.IntsOfInts[0][2]);
            Assert.AreEqual(3, res.IntsOfInts[1].Count);
            Assert.AreEqual(4, res.IntsOfInts[1][0]);
            Assert.AreEqual(5, res.IntsOfInts[1][1]);
            Assert.AreEqual(6, res.IntsOfInts[1][2]);
            Assert.AreEqual(3, res.IntsOfInts[2].Count);
            Assert.AreEqual(7, res.IntsOfInts[2][0]);
            Assert.AreEqual(8, res.IntsOfInts[2][1]);
            Assert.AreEqual(9, res.IntsOfInts[2][2]);
        }

        [TestMethod]
        public void IReadOnlyLists_array_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<_IReadOnlyLists>(
                "{\"Bytes\":[255,0,128],\"IntsOfInts\":[[1,2,3],[4,5,6],[7,8,9]]}");
            Assert.IsNotNull(res);

            Assert.IsNotNull(res.Bytes);
            Assert.AreEqual(3, res.Bytes.Count);
            Assert.AreEqual(255, res.Bytes[0]);
            Assert.AreEqual(0, res.Bytes[1]);
            Assert.AreEqual(128, res.Bytes[2]);

            Assert.IsNotNull(res.IntsOfInts);
            Assert.AreEqual(3, res.IntsOfInts.Count);
            Assert.IsNotNull(res.IntsOfInts[0]);
            Assert.AreEqual(3, res.IntsOfInts[0].Count);
            Assert.AreEqual(1, res.IntsOfInts[0][0]);
            Assert.AreEqual(2, res.IntsOfInts[0][1]);
            Assert.AreEqual(3, res.IntsOfInts[0][2]);
            Assert.AreEqual(3, res.IntsOfInts[1].Count);
            Assert.AreEqual(4, res.IntsOfInts[1][0]);
            Assert.AreEqual(5, res.IntsOfInts[1][1]);
            Assert.AreEqual(6, res.IntsOfInts[1][2]);
            Assert.AreEqual(3, res.IntsOfInts[2].Count);
            Assert.AreEqual(7, res.IntsOfInts[2][0]);
            Assert.AreEqual(8, res.IntsOfInts[2][1]);
            Assert.AreEqual(9, res.IntsOfInts[2][2]);
        }

        [TestMethod]
        public void IEnumerableMember_array_deserialize_should_be_correct()
        {
            var ress = JsonSerializer.ToObject<IEnumerable<string>>("[\"abcd\", \"efgh\"]");


            var res = JsonSerializer.ToObject<_IEnumerableMember>("{\"A\":[\"abcd\", \"efgh\"]}");
            Assert.IsNotNull(res);
            Assert.AreEqual(2, res.A.Count());
            Assert.AreEqual("abcd", res.A.ElementAt(0));
            Assert.AreEqual("efgh", res.A.ElementAt(1));
        }

        [TestMethod]
        public void IReadOnlyListMember_array_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<_IReadOnlyListMember>("{\"A\":[\"abcd\", \"efgh\"]}");
            Assert.IsNotNull(res);
            Assert.AreEqual(2, res.A.Count());
            Assert.AreEqual("abcd", res.A.ElementAt(0));
            Assert.AreEqual("efgh", res.A.ElementAt(1));
        }

        private List<T> AnonObjectByExample<T>(T example, string str)
        {
            return JsonSerializer.ToObject<List<T>>(str);
        }

        [TestMethod]
        public void NullsMember_array_deserialize_should_be_correct()
        {
            var example = new { A = 1 };
            var a = AnonObjectByExample(example, "[null, {\"A\":1234}, null]");
            Assert.AreEqual(3, a.Count);
            Assert.AreEqual(null, a[0]);
            Assert.AreEqual(1234, a[1].A);
            Assert.AreEqual(null, a[2]);
        }
      
        [TestMethod]
        public void AnonymousObject_array_deserialize_should_be_correct()
        {
            var example =
                new
                {
                    A = 1,
                    B = 1.0,
                    C = 1.0f,
                    D = 1.0m,
                    E = "",
                    F = 'a',
                    G = Guid.NewGuid(),
                    H = DateTime.UtcNow,
                    I = new[] { 1, 2, 3 }
                };

            const string str =
                "[{\"A\":1234, \"B\": 123.45, \"C\": 678.90, \"E\": \"hello world\", \"F\": \"c\", \"G\": \"EB29803F-A68D-4647-8512-5F0EE906CC90\", \"H\": \"1999-12-31\", \"I\": [1,2,3,4,5,6,7,8,9,10]}, {\"A\":1234, \"B\": 123.45, \"C\": 678.90, \"E\": \"hello world\", \"F\": \"c\", \"G\": \"EB29803F-A68D-4647-8512-5F0EE906CC90\", \"H\": \"1999-12-31\", \"I\": [1,2,3,4,5,6,7,8,9,10]}, {\"A\":1234, \"B\": 123.45, \"C\": 678.90, \"E\": \"hello world\", \"F\": \"c\", \"G\": \"EB29803F-A68D-4647-8512-5F0EE906CC90\", \"H\": \"1999-12-31\", \"I\": [1,2,3,4,5,6,7,8,9,10]}]";
            Assert.IsTrue(JsonValidator.IsValid(str));
            var res = AnonObjectByExample(example, str);
            Assert.IsNotNull(res);
            Assert.AreEqual(3, res.Count);
            var first = res[0];
            Assert.AreEqual(1234, first.A);
            Assert.AreEqual(123.45, first.B);
            Assert.AreEqual(678.90f, first.C);
            Assert.AreEqual(0m, first.D);
            Assert.AreEqual("hello world", first.E);
            Assert.AreEqual('c', first.F);
            Assert.AreEqual(Guid.Parse("EB29803F-A68D-4647-8512-5F0EE906CC90"), first.G);
            Assert.AreEqual(new DateTime(1999, 12, 31, 0, 0, 0, DateTimeKind.Utc), first.H);
            Assert.IsNotNull(first.I);
            Assert.AreEqual(10, first.I.Length);

            for (var i = 0; i < 10; i++) Assert.AreEqual(i + 1, first.I[i]);
        }

        [TestMethod]
        public void EmptyNested_array_deserialize_should_be_correct()
        {
            var jsonString2 = @"[{""col1"": []}]";

            var dt = JsonSerializer.ToObject<DataTable>(jsonString2);

            Assert.AreEqual(1, dt.Columns.Count);
            Assert.AreEqual(typeof(object), dt.Columns["col1"].DataType);

            var value = (List<object>)dt.Rows[0]["col1"];
            Assert.AreEqual(0, value.Count);
        }

        [TestMethod]
        public void ISet_array_deserialize_should_be_correct()
        {
            var res = JsonSerializer.ToObject<_ISets>(
                "{\"IntSet\":[17,23],\"StringHashSet\":[\"Hello\",\"Hash\",\"Set\"],\"IntSortedSet\":[1,2,3]}");
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.IntSet);
            Assert.AreEqual(2, res.IntSet.Count);
            Assert.IsTrue(res.IntSet.Contains(17));
            Assert.IsTrue(res.IntSet.Contains(23));

            Assert.IsNotNull(res.StringHashSet);
            Assert.AreEqual(3, res.StringHashSet.Count);
            Assert.IsTrue(res.StringHashSet.Contains("Hello"));
            Assert.IsTrue(res.StringHashSet.Contains("Hash"));
            Assert.IsTrue(res.StringHashSet.Contains("Set"));

            Assert.IsNotNull(res.IntSortedSet);
            Assert.AreEqual(3, res.IntSortedSet.Count);
            Assert.IsTrue(res.IntSortedSet.Contains(1));
            Assert.IsTrue(res.IntSortedSet.Contains(2));
            Assert.IsTrue(res.IntSortedSet.Contains(3));
        }

        T Convert<T>(T value)
        {
            return JsonSerializer.ToObject<T>(JsonSerializer.ToJson(value));
        }


        [TestMethod]
        public void MultiDObjectArray()
        {
            object[,] myOtherArray =
            {
                { new KeyValuePair<string, double>("my value", 0.8), "foobar" },
                { true, 0.4d },
                { 0.05f, 6 }
            };

            string myOtherArrayAsString = JsonSerializer.ToJson(myOtherArray);

            Assert.AreEqual(@"[[{""my value"":0.8},""foobar""],[true,0.4],[0.05,6]]", myOtherArrayAsString);

            object[,] myOtherResult = JsonSerializer.ToObject<object[,]>(myOtherArrayAsString);
            Assert.AreEqual(0.8, ((Dictionary<string, object>)myOtherResult[0, 0])["my value"]);
            Assert.AreEqual("foobar", myOtherResult[0, 1]);

            Assert.AreEqual(true, myOtherResult[1, 0]);
            Assert.AreEqual(0.4, myOtherResult[1, 1]);

            Assert.AreEqual(0.05, myOtherResult[2, 0]);
            Assert.AreEqual(6L, myOtherResult[2, 1]);
        }

        [TestMethod]
        public void MultiDimentional_array_deserialize_should_be_correct()
        {
            int dataI = 3, dataJ = 5, dataK = 10, dataL = 15;
            var two = new ValueTuple<int, int>[dataI, dataJ];
            var three = new ValueTuple<int, int, int>[dataI, dataJ, dataK];
            var four = new ValueTuple<int, int, int, int>[dataI, dataJ, dataK, dataL];

            for (int i = 0; i < dataI; i++)
            {
                for (int j = 0; j < dataJ; j++)
                {
                    two[i, j] = (i, j);
                    for (int k = 0; k < dataK; k++)
                    {
                        three[i, j, k] = (i, j, k);
                        for (int l = 0; l < dataL; l++)
                        {
                            four[i, j, k, l] = (i, j, k, l);
                        }
                    }
                }
            }

            var cTwo = Convert(two);
            var cThree = Convert(three);
            var cFour = Convert(four);

            Assert.AreEqual(cTwo.Length, two.Length);
            Assert.AreEqual(cThree.Length, three.Length);
            Assert.AreEqual(cFour.Length, four.Length);


            for (int i = 0; i < dataI; i++)
            {
                for (int j = 0; j < dataJ; j++)
                {
                    Assert.AreEqual(cTwo[i, j], two[i, j]);
                    for (int k = 0; k < dataK; k++)
                    {
                        Assert.AreEqual(cThree[i, j, k], three[i, j, k]);
                        for (int l = 0; l < dataL; l++)
                        {
                            Assert.AreEqual(cFour[i, j, k, l], four[i, j, k, l]);
                        }
                    }
                }
            }
        }
    }
}