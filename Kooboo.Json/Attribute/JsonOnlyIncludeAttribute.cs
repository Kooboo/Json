using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     序列化仅包含此元素,和Ignore相反
    ///     Serialization contains only this element, as opposed to Ignore
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonOnlyIncludeAttribute : Attribute
    {
    }
}
