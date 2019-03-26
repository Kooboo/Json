using System.Collections.Generic;
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
        #endregion


        /// <summary>
        /// Writer json container
        /// </summary>
        internal StringBuilder Writer = new StringBuilder();

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
            return Writer.ToString();
        }
    }
}
