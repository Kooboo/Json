using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     被标记的元素,在序列化或反序列化时时将被忽略
    ///     Marked elements are ignored when serialized or deserialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreKeyAttribute : Attribute
    {
    }
}
