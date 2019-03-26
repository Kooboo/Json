using System;
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
        internal static readonly MethodInfo _GlobalKeyFormatInvoke = typeof(Func<string,Type, string>).GetMethod("Invoke");
        internal static readonly FieldInfo _GlobalKeyFormat = typeof(JsonDeserializeOption).GetField(nameof(GlobalKeyFormat));
        internal static readonly MethodInfo _GlobalValueFormatInvoke = typeof(JsonDeserializeGlobalValueFormatDelegate).GetMethod("Invoke");
        internal static readonly FieldInfo _GlobalValueFormat = typeof(JsonDeserializeOption).GetField(nameof(GlobalValueFormat));
        #endregion

        /// <summary>
        ///     对Model的全局Key格式化器
        ///     Read the first letter of Key in Model: default, capitalization, lowercase
        /// </summary>
        public Func<string,Type, string> GlobalKeyFormat;

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
    }
}
