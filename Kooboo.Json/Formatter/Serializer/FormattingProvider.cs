using System;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Serializer
{
    internal class FormattingProvider<T>
    {
        internal static Action<T, JsonSerializerHandler> Get = FormatterFind<T>.Find();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(T type, JsonSerializerHandler handler) => Get(type, handler);
    }
}
