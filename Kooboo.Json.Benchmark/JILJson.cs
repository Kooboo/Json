using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public class JILJson : Base, IJson
    {
        public override string ToJson<T>(T obj)
        {
            return Jil.JSON.Serialize(obj);
        }

        public override T ToObject<T>(string json)
        {
            return Jil.JSON.Deserialize<T>(json);
        }

    }
}
