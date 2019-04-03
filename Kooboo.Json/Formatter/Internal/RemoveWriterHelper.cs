using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kooboo.Json.Serializer
{
    internal class RemoveWriterHelper
    {
        internal static MethodInfo _RemoveDictionaryKey = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveDictionaryKey), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(JsonSerializerHandler) }, null);
        internal static void RemoveDictionaryKey(JsonSerializerHandler handler)
        {
            if (handler.stringBuilder != null)
            {
                if (handler.stringBuilder.Length < 3)
                    return;
                int startIndex = 0;
                int leng = -1;
                for (int i = handler.stringBuilder.Length - 2 - 1; i >= 0; i--)
                {
                    if (handler.stringBuilder[i] == '"')
                    {
                        startIndex = i;
                        leng = handler.stringBuilder.Length - i;

                        if (i - 1 > 0)
                        {
                            if (handler.stringBuilder[i - 1] == ',')
                            {
                                startIndex = startIndex - 1;
                                leng = leng + 1;
                            }
                        }
                        handler.stringBuilder.Remove(startIndex, leng);
                        return;
                    }
                }
            }
            else
            {
                int length = StreamOperate.GetStreamWriterCharLen(handler.streamWriter);
                if (length < 3)
                    return;
                char[] buf = StreamOperate.GetStreamWriterCharBuffer(handler.streamWriter);
                int startIndex = 0;
                int leng = -1;
                for (int i = length - 2 - 1; i >= 0; i--)
                {
                    if (buf[i] == '"')
                    {
                        startIndex = i;
                        leng = length - i;

                        if (i - 1 > 0)
                        {
                            if (buf[i - 1] == ',')
                            {
                                startIndex = startIndex - 1;
                                leng = leng + 1;
                            }
                        }
                        handler.streamWriter.Remove(startIndex, leng);
                        return;
                    }
                }
            }
        }

        internal static MethodInfo _RemoveArrayItem = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveArrayItem), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(JsonSerializerHandler) }, null);
        internal static void RemoveArrayItem(JsonSerializerHandler handler)
        {
            if (handler.stringBuilder != null)
            {
                if (handler.stringBuilder.Length <= 1)
                    return;
                if (handler.stringBuilder[handler.stringBuilder.Length - 1] == '[')
                    return;
                if (handler.stringBuilder[handler.stringBuilder.Length - 1] == ':')
                    RemoveDictionaryKey(handler);
                else
                    handler.stringBuilder.Remove(handler.stringBuilder.Length - 1, 1);
            }
            else
            {
                int length = StreamOperate.GetStreamWriterCharLen(handler.streamWriter);
                if (length <= 1)
                    return;
                char[] buf = StreamOperate.GetStreamWriterCharBuffer(handler.streamWriter);
                if (buf[length - 1] == '[')
                    return;
                if (buf[length - 1] == ':')
                    RemoveDictionaryKey(handler);
                else
                    handler.streamWriter.Remove(length - 1, 1);
            }
        }

        internal static MethodInfo _RemoveKeyAndAddIndex = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveKeyAndAddIndex), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(JsonSerializerHandler), typeof(Stack<int>) }, null);
        internal static void RemoveKeyAndAddIndex(JsonSerializerHandler handler, Stack<int> commaIndexs)
        {
            if (handler.stringBuilder != null)
            {
                StringBuilder sb = handler.stringBuilder;
                if (sb.Length < 3)
                    return;
                int startIndex = 0;
                int leng = -1;
                for (int i = sb.Length - 2 - 1; i >= 0; i--)
                {
                    if (sb[i] == '"')
                    {
                        startIndex = i;
                        leng = sb.Length - i;

                        sb.Remove(startIndex, leng);
                        commaIndexs.Push(sb.Length - 1);
                        return;
                    }
                }
            }
            else
            {
                int length = StreamOperate.GetStreamWriterCharLen(handler.streamWriter);
                if (length < 3)
                    return;
                int startIndex = 0;
                char[] buf = StreamOperate.GetStreamWriterCharBuffer(handler.streamWriter);
                int leng = -1;
                for (int i = length - 2 - 1; i >= 0; i--)
                {
                    if (buf[i] == '"')
                    {
                        startIndex = i;
                        leng = length - i;

                        handler.streamWriter.Remove(startIndex, leng);
                        commaIndexs.Push(length - leng - 1);
                        return;
                    }
                }
            }
        }

        internal static MethodInfo _RemoveLastComma = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveLastComma), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(JsonSerializerHandler) }, null);
        internal static void RemoveLastComma(JsonSerializerHandler handler)
        {
            if (handler.stringBuilder != null)
            {
                StringBuilder sb = handler.stringBuilder;
                if (sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                int length = StreamOperate.GetStreamWriterCharLen(handler.streamWriter);
                char[] buf = StreamOperate.GetStreamWriterCharBuffer(handler.streamWriter);
                if (buf[length - 1] == ',')
                    handler.streamWriter.Remove(length - 1, 1);
            }
        }

        internal static MethodInfo _DeleteComma = typeof(RemoveWriterHelper).GetMethod(nameof(DeleteComma), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(JsonSerializerHandler), typeof(Stack<int>) }, null);
        internal static void DeleteComma(JsonSerializerHandler handler, Stack<int> lists)
        {
            if (handler.stringBuilder != null)
            {
                StringBuilder sb = handler.stringBuilder;
                if (lists.Count > 0)
                {
                    RemoveLastComma(handler);

                    var lastCommaIndex = lists.Peek();

                    if (lastCommaIndex >= sb.Length - 1)
                    {
                        lists.Pop();
                    }
                }
                while (lists.Count > 0)
                {
                    var index = lists.Pop();
                    sb.Remove(index + 1, 1);
                }
            }
            else
            {
                if (lists.Count > 0)
                {
                    RemoveLastComma(handler);

                    var lastCommaIndex = lists.Peek();

                    if (lastCommaIndex >= StreamOperate.GetStreamWriterCharLen(handler.streamWriter) - 1)
                    {
                        lists.Pop();
                    }
                }
                while (lists.Count > 0)
                {
                    var index = lists.Pop();
                    handler.streamWriter.Remove(index + 1, 1);
                }
            }
        }
    }
}
