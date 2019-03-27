using BenchmarkDotNet.Attributes;
using Kooboo.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public class KoobooJson : Base, IJson
    {
        public override string ToJson<T>(T obj)
        {
            return Kooboo.Json.JsonSerializer.ToJson(obj);
        }

        public override T ToObject<T>(string json)
        {
            return Kooboo.Json.JsonSerializer.ToObject<T>(json);
        }
    }
}
