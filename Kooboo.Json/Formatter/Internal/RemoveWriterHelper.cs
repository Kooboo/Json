using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kooboo.Json.Serializer
{
    internal class RemoveWriterHelper
    {
        internal static MethodInfo _RemoveDictionaryKey = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveDictionaryKey), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(StringBuilder)}, null);
        internal static void RemoveDictionaryKey(StringBuilder sb)
        {
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

                    if (i - 1 > 0)
                    {
                        if (sb[i - 1] == ',')
                        {
                            startIndex = startIndex - 1;
                            leng = leng + 1;
                        }
                    }
                    sb.Remove(startIndex, leng);
                    return;
                }
            }

        }

        internal static MethodInfo _RemoveArrayItem = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveArrayItem), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(StringBuilder) }, null);
        internal static void RemoveArrayItem(StringBuilder sb)
        {
            if (sb.Length <= 1)
                return;
            if (sb[sb.Length - 1] == '[')
                return;
            if (sb[sb.Length - 1] == ':')
                RemoveDictionaryKey(sb);
            else
                sb.Remove(sb.Length - 1, 1);

        }

        internal static MethodInfo _RemoveKeyAndAddIndex = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveKeyAndAddIndex), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(StringBuilder), typeof(Stack<int>) }, null);
        internal static void RemoveKeyAndAddIndex(StringBuilder sb, Stack<int> commaIndexs)
        {
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

        internal static MethodInfo _RemoveLastComma = typeof(RemoveWriterHelper).GetMethod(nameof(RemoveLastComma), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(StringBuilder) }, null);
        internal static void RemoveLastComma(StringBuilder sb)
        {
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
        }

        internal static MethodInfo _DeleteComma = typeof(RemoveWriterHelper).GetMethod(nameof(DeleteComma), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(StringBuilder), typeof(Stack<int>) }, null);
        internal static void DeleteComma(StringBuilder sb, Stack<int> lists)
        {
            if (lists.Count > 0)
            {
                RemoveLastComma(sb);

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
    }
}
