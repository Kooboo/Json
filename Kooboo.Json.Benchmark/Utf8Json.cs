using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public class UTF8Json : Base, IJson
    {
        public override string ToJson<T>(T obj)
        {
            return Utf8Json.JsonSerializer.ToJsonString(obj);
        }

        public override T ToObject<T>(string json)
        {
            return Utf8Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}
