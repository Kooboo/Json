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
                handler.WriteString("null");
                return;
            }

            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
            {
                if (handler.SerializeStacks.Contains(value))
                {
                    if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Null)
                        handler.WriteString("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.WriteString("{}");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveDictionaryKey(handler);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            var t = value.GetType();
            handler.WriteString("{");
            bool isFirst = true;
            foreach (var item in t.GetProperties())
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");
                handler.WriteString("\"");
                handler.WriteString(item.Name);
                handler.WriteString("\"");
                handler.WriteString(":");
                SerializerObjectJump.GetThreadSafetyJumpAction(item.PropertyType)(item.GetValue(value), handler);
            }
            handler.WriteString("}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AvoidTypes(object typeVar, JsonSerializerHandler handler)
        {
            handler.WriteString(typeVar == null ? "null" : "{}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteEnum(Enum value, JsonSerializerHandler handler)
        {
            if (handler.Option.IsEnumNum)
                PrimitiveNormal.WriteValue(value.GetHashCode(), handler);
            else
            {
                handler.WriteString("\"");
                handler.WriteString(value.ToString());
                handler.WriteString("\"");
            }
        }

        #region Array
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteCollection(IEnumerable value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.WriteString("null");
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
                        handler.WriteString("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.WriteString("[]");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveArrayItem(handler);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            handler.WriteString("[");
            bool isFirst = true;
            foreach (var obj in value)
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");
                PrimitiveNormal.WriteValue(obj, handler);
            }
            handler.WriteString("]");
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
                        handler.WriteString("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.WriteString("[]");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveArrayItem(handler);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }
            var action = SerializerObjectJump.GetThreadSafetyJumpAction(value.GetType().GetElementType());
            handler.WriteString("[");
            var enumerator = value.GetEnumerator();
            MultidimensionalWrite(enumerator, handler, value, action, value.GetLength(0), 0);
            handler.WriteString("]");
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
                handler.SerializeStacks.Pop();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MultidimensionalWrite(IEnumerator enumerator, JsonSerializerHandler handler, Array value, Action<object, JsonSerializerHandler> action, int leng, int level)
        {
            for (int i = 0; i < leng; i++)
            {
                if (level != value.Rank - 1)
                    handler.WriteString("[");

                if (level == value.Rank - 1)
                {
                    enumerator.MoveNext();
                    action(enumerator.Current, handler);
                    if (i != leng - 1)
                        handler.WriteString(",");
                    continue;
                }

                int localLevel = level;
                ++localLevel;
                MultidimensionalWrite(enumerator, handler, value, action, value.GetLength(localLevel), localLevel);

                handler.WriteString("]");

                if (i != leng - 1)
                    handler.WriteString(",");
            }

        }
        #endregion

        #region Dictionary
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteDictionary(IDictionary value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.WriteString("null");
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
                        handler.WriteString("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.WriteString("{}");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveDictionaryKey(handler);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            bool isFirst = true;
            handler.WriteString("{");
            foreach (DictionaryEntry item in value)
            {
                var keyType = item.Key.GetType();
                if (keyType.IsWrongKey())
                    continue;

                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");

                if (keyType.IsPrimitive || (keyType.IsEnum && handler.Option.IsEnumNum))//string,primitive,enum,guid,datetime
                    handler.WriteString("\"");

                PrimitiveNormal.WriteValue(item.Key, handler);

                if (keyType.IsPrimitive || (keyType.IsEnum && handler.Option.IsEnumNum))
                    handler.WriteString("\"");

                handler.WriteString(":");
                PrimitiveNormal.WriteValue(item.Value, handler);
            }
            handler.WriteString("}");
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
                handler.SerializeStacks.Pop();
        }
        #endregion

    }
}
