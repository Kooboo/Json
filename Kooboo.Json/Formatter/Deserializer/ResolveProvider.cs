using System.IO;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Deserialize
{
    internal class ResolveProvider<T>
    {
        internal static ResolveDelegate<T> Get = ResolveFind<T>.Find();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe T Convert(string json, JsonDeserializeHandler handler)
        {
            fixed (char* c = json)
            {
                JsonReader reader = new JsonReader(json, c);
                T result = Get(reader, handler);
                reader.ReadEnd();
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe T Convert(StreamReader streamReader, JsonDeserializeHandler handler)
        {
            //peek() => call  ReadBuffer(); =>call internal origin stream  byte[] copy local byte[] -> default utf8 convert -> generate  char[]
            streamReader.Peek();
            char[] buf = StreamOperate.GetStreamReaderCharBuffer(streamReader);
            int len = StreamOperate.GetStreamReaderCharLen(streamReader);
            fixed (char* c = buf)
            {
                JsonReader reader = new JsonReader(buf,len, c);
                T result = Get(reader, handler);
                reader.ReadEnd();
                return result;
            }
        }

        internal static T InvokeGet(JsonReader reader, JsonDeserializeHandler jsonDeserializeHandler)
        {
            return Get(reader, jsonDeserializeHandler);
        }
    }
    internal delegate T ResolveDelegate<T>(JsonReader jsonReader, JsonDeserializeHandler handler);
}
