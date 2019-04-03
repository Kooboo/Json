using System;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Serializer
{
    internal  class BaseTypeNormal: DefaultJsonFormatter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.BaseType)]
        internal static void WriteValue(Exception value, JsonSerializerHandler handler)
        {
            if (value == null)
                handler.WriteString("null");
            else
            {
                handler.WriteString("{\"Message\":");
                PrimitiveNormal.WriteValue(value.Message, handler);
                handler.WriteString(",\"Source\":");
                PrimitiveNormal.WriteValue(value.Source, handler);
                handler.WriteString(",\"StackTrace\":");
                PrimitiveNormal.WriteValue(value.StackTrace, handler);
                handler.WriteString(",\"HelpLink\":");
                PrimitiveNormal.WriteValue(value.HelpLink, handler);
                handler.WriteString("}");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.BaseType)]
        internal static void WriteValue(Type value, JsonSerializerHandler handler)
        {
            if (value == null)
                handler.WriteString("null");
            else
                PrimitiveNormal.WriteValue(value.AssemblyQualifiedName, handler);
        }
    }
}
