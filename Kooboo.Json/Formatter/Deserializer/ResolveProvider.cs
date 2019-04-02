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
            //stream is lazy,peek() =>call  ReadBuffer();
            streamReader.Peek();
            char[] buf = StreamOperate.StreamReader_CharBuffer(streamReader);
            int len = StreamOperate.StreamReader_CharLen(streamReader);
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
