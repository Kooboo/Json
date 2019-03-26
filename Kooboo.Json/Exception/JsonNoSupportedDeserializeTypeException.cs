using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     不受支持的解析类型
    ///     Unsupported parse type
    /// </summary>
    public class JsonNoSupportedDeserializeTypeException : Exception
    {
        internal JsonNoSupportedDeserializeTypeException(string msg) : base("Json Deserialize Unsupported type : " + msg)
        {

        }

        internal JsonNoSupportedDeserializeTypeException(Type t) : base("Json Deserialize Unsupported type : " + t.Name)
        {

        }

        internal JsonNoSupportedDeserializeTypeException(Type t, string msg) : base("Json Deserialize Unsupported type : " + t.Name + " , " + msg)
        {

        }
    }
}
