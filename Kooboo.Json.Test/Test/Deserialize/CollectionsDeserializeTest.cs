using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.Json;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Concurrent;
using System;
using JsonValidatorTool;
using System.Linq;

namespace Kooboo.Json.Test
{
    [TestClass]
    public class CollectionsDeserializeTest
    {
        [TestMethod]
        public void ObservableCollection_deserialize_should_be_correct()
        {
            ObservableCollection<int> oc = new ObservableCollection<int>(new List<int>() { 33 });
            var json = JsonSerializer.ToJson(oc);
            Assert.IsTrue(JsonValidator.IsValid(json));

            var obj = JsonSerializer.ToObject<ObservableCollection<int>>(json);
            Assert.AreEqual(oc.First(),obj.First());
        }

        [TestMethod]
        public void BitArray_deserialize_should_be_correct()
        {
            BitArray bit = new BitArray(new bool[] { true, false, true, false });
            var json = JsonSerializer.ToJson(bit);
            Assert.IsTrue(JsonValidator.IsValid(json));

            var obj = JsonSerializer.ToObject<BitArray>(json);
            Assert.IsTrue(obj[0]);
            Assert.IsFalse(obj[1]);
            Assert.IsTrue(obj[2]);
            Assert.IsFalse(obj[3]);
            Assert.AreEqual(4,obj.Length);
        }

        [TestMethod]
        public void ReadOnlyDictionary_deserialize_should_be_correct()
        {
            IReadOnlyDictionary<string, string> obj = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() { { "f", "f" } });
            var json = JsonSerializer.ToJson(obj);
            var obj2 = JsonSerializer.ToObject<IReadOnlyDictionary<string, string>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("f",obj2["f"]);
        }

        [TestMethod]
        public void ArraySegment_deserialize_should_be_correct()
        {
            ArraySegment<int> ast = new ArraySegment<int>(new int[] { 1 });
            var json = JsonSerializer.ToJson(ast);
            var obj = JsonSerializer.ToObject<ArraySegment<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(1, obj.First());
        }

        [TestMethod]
        public void Hashtable_deserialize_should_be_correct()
        {
            Hashtable hash = new Hashtable
            {
                { "1", "1" }
            };
            var json = JsonSerializer.ToJson(hash);
            var obj = JsonSerializer.ToObject<Hashtable>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.IsTrue(obj.ContainsValue("1"));
            Assert.IsTrue(obj.ContainsKey("1"));
        }

        [TestMethod]
        public void SortedList_deserialize_should_be_correct()
        {
            SortedList sortedList = new SortedList();
            sortedList.Add("1", "1");
            var json = JsonSerializer.ToJson(sortedList);
            var obj = JsonSerializer.ToObject<SortedList>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.IsTrue(obj.ContainsValue("1"));
            Assert.IsTrue(obj.ContainsKey("1"));
        }

        [TestMethod]
        public void ArrayList_deserialize_should_be_correct()
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(1);
            var json = JsonSerializer.ToJson(arrayList);
            var obj = JsonSerializer.ToObject<ArrayList>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual((long)1,obj[0]);
        }

        [TestMethod]
        public void Stack_deserialize_should_be_correct()
        {
            Stack<string> stack = new Stack<string>();
            stack.Push("1");
            var json = JsonSerializer.ToJson(stack);
            var obj = JsonSerializer.ToObject<Stack<string>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1", obj.First());
        }

        [TestMethod]
        public void Queue_deserialize_should_be_correct()
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue("1");
            var json = JsonSerializer.ToJson(queue);
            var obj = JsonSerializer.ToObject<Queue<string>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1", obj.First());
        }

        [TestMethod]
        public void ConcurrentBag_deserialize_should_be_correct()
        {
            ConcurrentBag<int> cBag = new ConcurrentBag<int> {3};
            var json = JsonSerializer.ToJson(cBag);
            var obj = JsonSerializer.ToObject<ConcurrentBag<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(3, obj.First());
        }

        [TestMethod]
        public void ConcurrentQueue_deserialize_should_be_correct()
        {
            ConcurrentQueue<int> cQueue = new ConcurrentQueue<int>();
            cQueue.Enqueue(1);
            var json = JsonSerializer.ToJson(cQueue);
            var obj = JsonSerializer.ToObject<ConcurrentQueue<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(1, obj.First());
        }

        [TestMethod]
        public void ConcurrentStack_deserialize_should_be_correct()
        {
            ConcurrentStack<int> cStack = new ConcurrentStack<int>();
            cStack.Push(1);
            var json = JsonSerializer.ToJson(cStack);
            var obj = JsonSerializer.ToObject<ConcurrentStack<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(1, obj.First());
        }

        [TestMethod]
        public void SortedDictionary_deserialize_should_be_correct()
        {
            SortedDictionary<string, string> sortDic = new SortedDictionary<string, string> {{"1", "1"}};
            var json = JsonSerializer.ToJson(sortDic);
            var obj = JsonSerializer.ToObject<SortedDictionary<string, string>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1", obj["1"]);
        }

        [TestMethod]
        public void ConcurrentDictionary_deserialize_should_be_correct()
        {
            ConcurrentDictionary<string, string> cDic = new ConcurrentDictionary<string, string>();
            cDic.TryAdd("1", "1");
            var json = JsonSerializer.ToJson(cDic);
            var obj = JsonSerializer.ToObject<ConcurrentDictionary<string, string>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual("1", obj["1"]);
        }

        [TestMethod]
        public void HashSet_deserialize_should_be_correct()
        {
            HashSet<int> hashset = new HashSet<int>();
            hashset.Add(1);
            var json = JsonSerializer.ToJson(hashset);
            var obj = JsonSerializer.ToObject<HashSet<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(1, obj.First());
        }

        [TestMethod]
        public void SortedSet_deserialize_should_be_correct()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            sortedSet.Add(1);
            var json = JsonSerializer.ToJson(sortedSet);
            var obj = JsonSerializer.ToObject<SortedSet<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(1, obj.First());
        }

        [TestMethod]
        public void LinkedList_deserialize_should_be_correct()
        {
            LinkedList<int> linkS = new LinkedList<int>();
            ICollection<int> sdf = (ICollection<int>)linkS;
            sdf.Add(3);

            linkS.AddFirst(1);
            var json = JsonSerializer.ToJson(linkS);
            var obj = JsonSerializer.ToObject<LinkedList<int>>(json);

            Assert.IsTrue(JsonValidator.IsValid(json));
            Assert.AreEqual(1, obj.First());
        }

        class KnownAutoTypes
        {
            public ICollection<string> Collection { get; set; }
            public IList<string> List { get; set; }
            public IDictionary<string, string> Dictionary { get; set; }
            public ISet<string> Set { get; set; }
            public IReadOnlyCollection<string> ReadOnlyCollection { get; set; }
            public IReadOnlyList<string> ReadOnlyList { get; set; }
            public IReadOnlyDictionary<string, string> ReadOnlyDictionary { get; set; }
        }

        [TestMethod]
        public void Collections_Interface_deserialize_should_be_correct()
        {
            var c = new KnownAutoTypes
            {
                Collection = new List<string> { "Collection value!" },
                List = new List<string> { "List value!" },
                Dictionary = new Dictionary<string, string>
                {
                    {"Dictionary key!", "Dictionary value!"}
                },
                ReadOnlyCollection = new ReadOnlyCollection<string>(new[] { "Read Only Collection value!" }),
                ReadOnlyList = new ReadOnlyCollection<string>(new[] { "Read Only List value!" }),
                Set = new HashSet<string> { "Set value!" },
                ReadOnlyDictionary = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
                {
                    {"Read Only Dictionary key!", "Read Only Dictionary value!"}
                })
            };

            var json = JsonSerializer.ToJson(c);
            Assert.IsTrue(JsonValidator.IsValid(json));

            var collection = JsonSerializer.ToObject<KnownAutoTypes>(json);
            Assert.AreEqual("Collection value!", collection.Collection.First());
            Assert.AreEqual("List value!", collection.List.First());
            Assert.AreEqual("Dictionary value!", collection.Dictionary["Dictionary key!"]);
            Assert.AreEqual("Read Only Collection value!", collection.ReadOnlyCollection.First());
            Assert.AreEqual("Read Only List value!", collection.ReadOnlyList.First());
            Assert.AreEqual("Set value!", collection.Set.Take(1).ToArray()[0]);
            Assert.AreEqual("Read Only Dictionary value!", collection.ReadOnlyDictionary["Read Only Dictionary key!"]);
        }
    }
}
