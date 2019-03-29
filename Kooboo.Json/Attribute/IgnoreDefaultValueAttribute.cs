using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     当被标记元素的值为默认值时,序列化时将忽略其元素
    ///     When the value of the tagged element is the default, its element is ignored when serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                   AttributeTargets.Struct)]
    public class IgnoreDefaultValueAttribute : Attribute
    {
    }
}