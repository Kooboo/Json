
namespace Kooboo.Json
{
    /// <summary>
    ///     发生循环引用时的处理方式枚举
    ///     Enumeration of processing methods when circular references occur
    /// </summary>
    public enum JsonReferenceHandlingEnum
    {
        /// <summary>
        /// 默认,不处理
        /// Default
        /// </summary>
        None,
        /// <summary>
        /// 将值处理为null
        /// Processing the value to null
        /// </summary>
        Null,
        /// <summary>
        /// 删除该值
        /// Delete this value
        /// </summary>
        Remove,
        /// <summary>
        /// 返回空(Keyvalue =>{}，Array=>[])
        /// Return empty
        /// </summary>
        Empty
    }
}
