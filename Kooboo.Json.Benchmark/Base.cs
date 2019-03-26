using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 100)]
    public abstract class Base
    {
        public abstract string ToJson<T>(T obj);

        public abstract T ToObject<T>(string json);

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
            ToObject<Entity[]>(Jsons.ArrayJson);
        }

        [Benchmark]
        public void JsonToDictionary()
        {
            ToObject<Dictionary<string, Entity>>(Jsons.DictionaryJson);
        }

        [Benchmark]
        public void JsonToList()
        {
            ToObject<List<Entity>>(Jsons.ListJson);
        }

        [Benchmark]
        public void JsonToEntity()
        {
            ToObject<Entity>(Jsons.EntityJson);
        }
    }
}
