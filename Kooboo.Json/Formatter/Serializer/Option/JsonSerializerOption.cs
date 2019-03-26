using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kooboo.Json
{
    /// <summary>
    ///     序列化全局配置器
    ///     Serializer global configuration's options
    /// </summary>
    public class JsonSerializerOption
    {
        #region pregenerated metas
        internal static readonly FieldInfo _JsonCharacterRead = typeof(JsonSerializerOption).GetField(nameof(JsonCharacterRead));
        internal static readonly FieldInfo _ReferenceLoopHandling = typeof(JsonSerializerOption).GetField(nameof(ReferenceLoopHandling));
        internal static readonly FieldInfo _IsIgnoreValueNull = typeof(JsonSerializerOption).GetField(nameof(IsIgnoreValueNull));
        internal static readonly FieldInfo _IgnoreKeys = typeof(JsonSerializerOption).GetField(nameof(IgnoreKeys));
        internal static readonly FieldInfo _IsEnumNum = typeof(JsonSerializerOption).GetField(nameof(IsEnumNum));

        internal static readonly MethodInfo _GlobalKeyFormatInvoke = typeof(Func<string,Type, JsonSerializerHandler, string>).GetMethod("Invoke");
        internal static readonly FieldInfo _GlobalKeyFormat = typeof(JsonSerializerOption).GetField(nameof(GlobalKeyFormat));
        internal static readonly MethodInfo _GlobalValueFormatInvoke = typeof(JsonSerializerGlobalValueFormatDelegate).GetMethod("Invoke");
        internal static readonly FieldInfo _GlobalValueFormat = typeof(JsonSerializerOption).GetField(nameof(GlobalValueFormat));
        #endregion

        /// <summary>
        /// Datetime格式化枚举，默认ISO8601
        /// Datetime formatted enumeration, default ISO8601
        /// </summary>
        public DatetimeFormatEnum DatetimeFormat = DatetimeFormatEnum.ISO8601;

        /// <summary>
        /// Timespan格式化枚举，默认ISO8601
        /// Timespan formatted enumeration, default ISO8601
        /// </summary>
        public TimespanFormatEnum TimespanFormat = TimespanFormatEnum.ISO8601;

        /// <summary>
        /// 枚举是否被序列化为数字，true -> 数字, false -> 字符
        /// Enumeration is serialized into numbers, true - > numbers, false - > characters
        /// </summary>
        public bool IsEnumNum = true;

        /// <summary>
        /// byte[]数组是否按照base64格式来序列化, true -> base64 , false -> array
        /// Is the byte [] array serialized in Base64 format
        /// </summary>
        public bool IsByteArrayFormatBase64 = false;
       
        /// <summary>
        /// 对Model中的Key的首字母写入状态：默认，大写，小写
        /// Write the first letter of Key in Model: default, capitalization, lowercase
        /// </summary>
        public JsonCharacterReadStateEnum JsonCharacterRead = JsonCharacterReadStateEnum.None;

        /// <summary>
        /// 对实例因互相引用而导致的无限循环的情况的处理
        /// Handling of infinite loops caused by cross-references of instances
        /// </summary>
        public JsonReferenceHandlingEnum ReferenceLoopHandling = JsonReferenceHandlingEnum.None;
      
        /// <summary>
        /// 是否忽略Model中原本值为null的对象(不包括由忽略互引用导致的null）
        /// Whether to ignore null objects in Model (excluding null caused by ignoring mutual references)
        /// </summary>
        public bool IsIgnoreValueNull = false;

        /// <summary>
        /// 对Model中要忽略写入的Key
        /// Key to ignore writing in Model
        /// </summary>
        public List<string> IgnoreKeys;
   
        /// <summary>
        /// 对Model的全局Key格式化器
        /// Global Key formatter for Model
        /// </summary>
        public Func<string,Type, JsonSerializerHandler, string> GlobalKeyFormat;

        /// <summary>
        /// 对Model的全局Value格式化器
        /// Global Value Formatter for Model
        /// </summary>
        public JsonSerializerGlobalValueFormatDelegate GlobalValueFormat;

    }

}
