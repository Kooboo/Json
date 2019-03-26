namespace Kooboo.Json
{
    /// <summary>
    ///     对Model中的字符读取状态：默认、首字母大写、首字母小写、忽略大小写
    ///     Read status for characters in the Model: Default, Initial uppercase, Initial lowercase, Ignore case
    /// </summary>
    public enum JsonCharacterReadStateEnum
    {
        /// <summary>
        /// 默认
        /// Default
        /// </summary>
        None,
        /// <summary>
        /// 首字母大写
        /// Initial uppercase
        /// </summary>
        InitialUpper,
        /// <summary>
        /// 首字母小写
        /// Initial lowercase
        /// </summary>
        InitialLower,
        /// <summary>
        /// 仅反序列化时使用,忽略大小写
        /// Used only for deserialization ,Ignore case
        /// </summary>
        IgnoreCase

    }
}
