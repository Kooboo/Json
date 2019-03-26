using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    public  interface IJson
    {
        string ToJson<T>(T obj);

        T ToObject<T>(string json);
    }
}
