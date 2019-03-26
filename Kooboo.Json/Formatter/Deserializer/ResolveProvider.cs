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
                T result = Get(ref reader, handler);
                reader.ReadEnd();
                return result;
            }
        }

        internal static T InvokeGet(ref JsonReader reader, JsonDeserializeHandler jsonDeserializeHandler)
        {
            return Get(ref reader, jsonDeserializeHandler);
        }
    }
    internal delegate T ResolveDelegate<T>(ref JsonReader jsonReader, JsonDeserializeHandler handler);
}
