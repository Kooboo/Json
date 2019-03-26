using System;
using System.Reflection;

namespace Kooboo.Json
{
    /// <summary>
    ///     对Model的值格式化器特性,可标注于字段，属性，结构或类上,(字段/属性) 优先级大于 (结构/类)
    ///     Value formatter features of Model can be labeled on fields, properties, structures or classes with priority of
    ///     (fields/properties) over (structures/classes)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public abstract class ValueFormatAttribute : Attribute
    {
        #region pregenerated metas
        internal static MethodInfo _WriteValueFormat = typeof(ValueFormatAttribute).GetMethod(nameof(WriteValueFormat)); 
        internal static MethodInfo _ReadValueFormat = typeof(ValueFormatAttribute).GetMethod(nameof(ReadValueFormat));
        #endregion

        /// <summary>
        ///     序列化时 - Model的Value的格式化器
        ///     Serialization time - Value formatter for Model
        /// </summary>
        /// <param name="value">需要被格式化的源元素数据,Source element data that needs to be formatted</param>
        /// <param name="type">值的类型,The type of the value</param>
        /// <param name="handler">用于提供一些配置选项,Used to provide some configuration options</param>
        /// <param name="isValueFormat">决定最终是否进行值格式化,Determines whether the value is ultimately formatted</param>
        /// <returns>格式化后的结果,Formatted results</returns>
        public virtual string WriteValueFormat(object value,Type type, JsonSerializerHandler handler, out bool isValueFormat)
        {
            isValueFormat = false;
            return null;
        }

        /// <summary>
        ///     反序列化时 - Model的Value的格式化器
        ///     When deserializing - Value formatter for Model
        /// </summary>
        /// <param name="value">从Json字符串中读取的匹配字符串,Matched strings read from Json strings</param>
        /// <param name="type">值的类型,The type of the value</param>
        /// <param name="handler">用于提供一些配置选项,Used to provide some configuration options</param>
        /// <param name="isValueFormat">决定最终是否进行值格式化,Determines whether the value is ultimately formatted</param>
        /// <returns>格式化后的结果,Formatted results</returns>
        public virtual object ReadValueFormat(string value,Type type, JsonDeserializeHandler handler, out bool isValueFormat)
        {
            isValueFormat = false;
            return null;
        }

    }
}
