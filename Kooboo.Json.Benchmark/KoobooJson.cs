using BenchmarkDotNet.Attributes;
using Kooboo.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public class KoobooJson : Base, IJson
    {
        static JsonSerializerOption ss = new Kooboo.Json.JsonSerializerOption() { DatetimeFormat = Kooboo.Json.DatetimeFormatEnum.Microsoft };
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
