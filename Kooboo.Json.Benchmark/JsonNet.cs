using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public class JsonNet : Base, IJson
    {
        public override string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public override T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
