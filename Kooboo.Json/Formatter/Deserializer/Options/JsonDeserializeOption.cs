using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kooboo.Json
{
    /// <summary>
    ///     反序列化全局配置项
    ///     Deserialized global configuration's options
    /// </summary>
    public class JsonDeserializeOption
    {
        #region pregenerated metas
        internal static readonly FieldInfo _JsonCharacterReadState = typeof(JsonDeserializeOption).GetField(nameof(JsonCharacterReadState));
        internal static readonly MethodInfo _GlobalKeyFormatInvoke = typeof(Func<string, Type, string>).GetMethod("Invoke");
        internal static readonly FieldInfo _GlobalKeyFormat = typeof(JsonDeserializeOption).GetField(nameof(GlobalKeyFormat));
        internal static readonly MethodInfo _GlobalValueFormatInvoke = typeof(JsonDeserializeGlobalValueFormatDelegate).GetMethod("Invoke");
        internal static readonly FieldInfo _GlobalValueFormat = typeof(JsonDeserializeOption).GetField(nameof(GlobalValueFormat));
        internal static readonly FieldInfo _IgnoreJsonKeys = typeof(JsonDeserializeOption).GetField(nameof(IgnoreJsonKeys));
        internal static readonly FieldInfo _IsIgnoreExtraKeysInJSON = typeof(JsonDeserializeOption).GetField(nameof(IsIgnoreExtraKeysInJSON));
        internal static readonly PropertyInfo _IgnoreJsonKeysHasValue = typeof(JsonDeserializeOption).GetProperty(nameof(IgnoreJsonKeysHasValue), BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly MethodInfo _IgnoreJsonKeyContains = typeof(HashSet<string>).GetMethod("Contains");
        #endregion

        /// <summary>
        ///     对Model的全局Key格式化器
        ///     Read the first letter of Key in Model: default, capitalization, lowercase
        /// </summary>
        public Func<string, Type, string> GlobalKeyFormat;

        /// <summary>
        ///     对Model的全局Value格式化器
        ///     Global Value Formatter for Model
        /// </summary>
        public JsonDeserializeGlobalValueFormatDelegate GlobalValueFormat;

        /// <summary>
        ///     对Model中的字符读取状态：默认、首字母大写、首字母小写、忽略大小写
        ///     Read status for characters in the Model: Default, Initial uppercase, Initial lowercase, Ignore case
        /// </summary>
        public JsonCharacterReadStateEnum JsonCharacterReadState = JsonCharacterReadStateEnum.None;

        /// <summary>
        ///     在对Model进行反序列化时,当JsonCharacterReadState为默认值时,忽略JSON字符中指定的Key
        ///     When deserializing a model, the key specified in the JSON character is ignored when the JsonCharacterReadState is the default
        /// </summary>
        public HashSet<string> IgnoreJsonKeys;

        /// <summary>
        ///     在对Model进行反序列化时,当JsonCharacterReadState为默认值时,是否忽略JSON字符中多余的Key
        ///     When deserializing the model, whether to ignore the extra key in the JSON character when JsonCharacterReadState is the default value
        /// </summary>
        public bool IsIgnoreExtraKeysInJSON;

        internal bool IgnoreJsonKeysHasValue => IgnoreJsonKeys != null && IgnoreJsonKeys.Count > 0;
    }
}
