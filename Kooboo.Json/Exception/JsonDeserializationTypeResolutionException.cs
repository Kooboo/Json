using Kooboo.Json.Deserialize;
using System;
using System.Reflection;

namespace Kooboo.Json
{
    /// <summary>
    ///     类型解析错误
    ///     Json Deserialization TypeResolution Exception
    /// </summary>
    public class JsonDeserializationTypeResolutionException : Exception
    {
        #region pregenerated metas
        internal static ConstructorInfo _JsonDeserializationTypeResolutionExceptionMsgCtor =
            typeof(JsonDeserializationTypeResolutionException).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);

        internal static ConstructorInfo _JsonDeserializationTypeResolutionExceptionCtor =
            typeof(JsonDeserializationTypeResolutionException).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(JsonReader), typeof(Type) }, null);
        #endregion


        internal JsonDeserializationTypeResolutionException() : base("Json deserialization type parsing error")
        {
        }

        internal JsonDeserializationTypeResolutionException(string msg) : base(
            $"Json deserialization type parsing error : {msg}")
        {
        }

        internal JsonDeserializationTypeResolutionException(Type t) : base(
            $"Json deserialization {t.Name} type parsing error")
        {
        }

        internal JsonDeserializationTypeResolutionException(JsonReader reader, Type t) : base(
            $"Json deserialization {t.Name} type parsing error ,An error occurred on symbol {reader.Length - reader.Remaining-1} , it's {reader.Json[reader.Length - reader.Remaining-1]}")
        {
        }

        internal JsonDeserializationTypeResolutionException(JsonReader reader, Type t, string msg) : base(
            $"Json deserialization {t.Name} type parsing error ,An error occurred on symbol {reader.Length - reader.Remaining-1} , {msg}")
        {
        }

        internal JsonDeserializationTypeResolutionException(Type t, string msg) : base(
            $"Json deserialization {t.Name} type parsing error , {msg}")
        {
        }
    }
}
