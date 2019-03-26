using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Serializer
{
    internal class SpecialConditions : DefaultJsonFormatter
    {
        #region pregenerated metas
        internal static MethodInfo _WriteEnum = typeof(SpecialConditions).GetMethod(nameof(WriteEnum), BindingFlags.NonPublic | BindingFlags.Static);
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteDynamic(object value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.Writer.Append("null");
                return;
            }

            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
            {
                if (handler.SerializeStacks.Contains(value))
                {
                    if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Null)
                        handler.Writer.Append("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.Writer.Append("{}");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveDictionaryKey(handler.Writer);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            var t = value.GetType();
            handler.Writer.Append("{");
            bool isFirst = true;
            foreach (var item in t.GetProperties())
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.Writer.Append(",");
                handler.Writer.Append("\"");
                handler.Writer.Append(item.Name);
                handler.Writer.Append("\"");
                handler.Writer.Append(":");
                SerializerObjectJump.GetThreadSafetyJumpAction(item.PropertyType)(item.GetValue(value), handler);
            }
            handler.Writer.Append("}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AvoidTypes(object typeVar, JsonSerializerHandler handler)
        {
            handler.Writer.Append(typeVar == null ? "null" : "{}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteEnum(Enum value, JsonSerializerHandler handler)
        {
            if (handler.Option.IsEnumNum)
                PrimitiveNormal.WriteValue(value.GetHashCode(), handler);
            else
            {
                handler.Writer.Append("\"");
                handler.Writer.Append(value.ToString());
                handler.Writer.Append("\"");
            }
        }

        #region Array
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteCollection(IEnumerable value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.Writer.Append("null");
                return;
            }

            var t = value.GetType();
            if (t.IsGenericType)
            {
                var serializerAction = SerializerObjectJump.GetThreadSafetyJumpAction(t);
                serializerAction(value, handler);
            }
            else
            {
                bool isMultidimensionalArray = false;
                Array ary = null;
                if (t.IsArray)
                {
                    ary = (Array)value;
                    if (ary.Rank > 1)
                        isMultidimensionalArray = true;
                }
                if (isMultidimensionalArray)
                    WriteMultidimensionalArray(ary, handler);
                else
                    WriteIEnumerable(value, handler);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteIEnumerable(IEnumerable value, JsonSerializerHandler handler)
        {
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
            {
                if (handler.SerializeStacks.Contains(value))
                {
                    if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Null)
                        handler.Writer.Append("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.Writer.Append("[]");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveArrayItem(handler.Writer);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            handler.Writer.Append("[");
            bool isFirst = true;
            foreach (var obj in value)
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.Writer.Append(",");
                PrimitiveNormal.WriteValue(obj, handler);
            }
            handler.Writer.Append("]");
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
                handler.SerializeStacks.Pop();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteMultidimensionalArray(Array value, JsonSerializerHandler handler)
        {
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
            {
                if (handler.SerializeStacks.Contains(value))
                {
                    if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Null)
                        handler.Writer.Append("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.Writer.Append("[]");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveArrayItem(handler.Writer);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }
            var action = SerializerObjectJump.GetThreadSafetyJumpAction(value.GetType().GetElementType());
            handler.Writer.Append("[");
            var enumerator = value.GetEnumerator();
            MultidimensionalWrite(enumerator, handler, value, action, value.GetLength(0), 0);
            handler.Writer.Append("]");
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
                handler.SerializeStacks.Pop();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MultidimensionalWrite(IEnumerator enumerator, JsonSerializerHandler handler, Array value, Action<object, JsonSerializerHandler> action, int leng, int level)
        {
            for (int i = 0; i < leng; i++)
            {
                if (level != value.Rank - 1)
                    handler.Writer.Append("[");

                if (level == value.Rank - 1)
                {
                    enumerator.MoveNext();
                    action(enumerator.Current, handler);
                    if (i != leng - 1)
                        handler.Writer.Append(",");
                    continue;
                }

                int localLevel = level;
                ++localLevel;
                MultidimensionalWrite(enumerator, handler, value, action, value.GetLength(localLevel), localLevel);

                handler.Writer.Append("]");

                if (i != leng - 1)
                    handler.Writer.Append(",");
            }

        }
        #endregion

        #region Dictionary
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteDictionary(IDictionary value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.Writer.Append("null");
                return;
            }
            var t = value.GetType();
            if (t.IsGenericType)
            {
                var serializerAction = SerializerObjectJump.GetThreadSafetyJumpAction(t);
                serializerAction(value, handler);
            }
            else
                WriteIDictionary(value, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteIDictionary(IDictionary value, JsonSerializerHandler handler)
        {
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
            {
                if (handler.SerializeStacks.Contains(value))
                {
                    if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Null)
                        handler.Writer.Append("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.Writer.Append("{}");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveDictionaryKey(handler.Writer);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            bool isFirst = true;
            handler.Writer.Append("{");
            foreach (DictionaryEntry item in value)
            {
                var keyType = item.Key.GetType();
                if (keyType.IsWrongKey())
                    continue;

                if (isFirst)
                    isFirst = false;
                else
                    handler.Writer.Append(",");

                if (keyType.IsPrimitive || (keyType.IsEnum && handler.Option.IsEnumNum))//string,primitive,enum,guid,datetime
                    handler.Writer.Append("\"");

                PrimitiveNormal.WriteValue(item.Key, handler);

                if (keyType.IsPrimitive || (keyType.IsEnum && handler.Option.IsEnumNum))
                    handler.Writer.Append("\"");

                handler.Writer.Append(":");
                PrimitiveNormal.WriteValue(item.Value, handler);
            }
            handler.Writer.Append("}");
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
                handler.SerializeStacks.Pop();
        }
        #endregion

    }
}
