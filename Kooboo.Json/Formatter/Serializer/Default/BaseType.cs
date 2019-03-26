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
                handler.Writer.Append("null");
            else
            {
                handler.Writer.Append("{\"Message\":");
                PrimitiveNormal.WriteValue(value.Message, handler);
                handler.Writer.Append(",\"Source\":");
                PrimitiveNormal.WriteValue(value.Source, handler);
                handler.Writer.Append(",\"StackTrace\":");
                PrimitiveNormal.WriteValue(value.StackTrace, handler);
                handler.Writer.Append(",\"HelpLink\":");
                PrimitiveNormal.WriteValue(value.HelpLink, handler);
                handler.Writer.Append("}");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.BaseType)]
        internal static void WriteValue(Type value, JsonSerializerHandler handler)
        {
            if (value == null)
                handler.Writer.Append("null");
            else
                PrimitiveNormal.WriteValue(value.AssemblyQualifiedName, handler);
        }
    }
}
