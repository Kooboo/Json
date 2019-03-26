using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public class NetJson1 : /*Base, */IJson
    {
        public /*override*/ string ToJson<T>(T obj)
        {
            return NetJSON.NetJSON.Serialize(obj);
        }

        public /*override*/ T ToObject<T>(string json)
        {
            return NetJSON.NetJSON.Deserialize<T>(json);
        }
        [Benchmark]
        public void ArrayToJson()
        {
            ToJson(Jsons.Array);
        }

        [Benchmark]
        public void DictionaryToJson()
        {
            ToJson(Jsons.Dictionary);
        }

        [Benchmark]
        public void ListToJson()
        {
            ToJson(Jsons.List);
        }

        [Benchmark]
        public void EntityToJson()
        {
            ToJson(Jsons.Entity);
        }

        [Benchmark]
        public void JsonToArray()
        {
            ToObject<AA[]>(Jsons.ArrayJson);
        }

        [Benchmark]
        public void JsonToDictionary()
        {
            ToObject<Dictionary<string, AA>>(Jsons.DictionaryJson);
        }

        [Benchmark]
        public void JsonToList()
        {
            ToObject<List<AA>>(Jsons.ListJson);
        }

        [Benchmark]
        public void JsonToEntity()
        {
            ToObject<AA>(Jsons.EntityJson);
        }
    }
}
