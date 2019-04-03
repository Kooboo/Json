using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kooboo.Json
{
    /// <summary>
    ///     提供了用于在序列化中的一些配置
    ///     Provides some configuration for serialization
    /// </summary>
    public class JsonSerializerHandler
    {
        #region pregenerated metas
        internal static readonly FieldInfo _Option = typeof(JsonSerializerHandler).GetField(nameof(Option));
        internal static readonly FieldInfo _SerializeStacks = typeof(JsonSerializerHandler).GetField(nameof(SerializeStacks), BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo _CommaIndexLists = typeof(JsonSerializerHandler).GetField(nameof(CommaIndexLists), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static readonly MethodInfo _WriteStringInvoke = typeof(Action<string>).GetMethod("Invoke");
        internal static readonly FieldInfo _WriteString = typeof(JsonSerializerHandler).GetField(nameof(WriteString));
        #endregion

        internal JsonSerializerHandler(StringBuilder stringBuilder) {
            this.stringBuilder = stringBuilder;
            WriteString = str => this.stringBuilder.Append(str);
            WriteChars = (chars, start, length) => this.stringBuilder.Append(chars, start, length);
            WriteChar = @char => this.stringBuilder.Append(@char);
            WriteLong = @long => this.stringBuilder.Append(@long);
        }

        internal JsonSerializerHandler(StreamWriter streamWriter)
        {
            this.streamWriter = streamWriter;
            WriteString = str => this.streamWriter.Write(str);
            WriteChars = (chars, start, length) => this.streamWriter.Write(chars, start, length);
            WriteChar = @char => this.streamWriter.Write(@char);
            WriteLong = @long => this.streamWriter.Write(@long);
        }

        internal StringBuilder stringBuilder;

        internal StreamWriter streamWriter;

        /// <summary>
        /// Write Long
        /// </summary>
        public Action<long> WriteLong;

        /// <summary>
        /// Write Char
        /// </summary>
        public Action<char> WriteChar;

        /// <summary>
        /// Write String
        /// </summary>
        public Action<string> WriteString;

        /// <summary>
        /// Write Chars
        /// </summary>
        public Action<char[], int, int> WriteChars;

        /// <summary>
        /// Json Serializer Option
        /// </summary>
        public JsonSerializerOption Option = null;

        /// <summary>
        /// Used for infinite loop interreferences
        /// </summary>
        internal Stack<object> SerializeStacks = new Stack<object>();
      
        /// <summary>
        /// Used for JsonRefernceHandlingEnum. Remove case, customtype comma to delete
        /// </summary>
        internal Stack<int> CommaIndexLists = new Stack<int>();

        internal new string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
