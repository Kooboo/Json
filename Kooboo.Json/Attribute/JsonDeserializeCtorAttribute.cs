using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     对Model进行Json反序列化时指定一个构造函数
    ///     Specify a constructor for Json deserialization of Model
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class JsonDeserializeCtorAttribute : Attribute
    {
        internal object[] _args;

        /// <summary>
        ///     反序列化时的指定构造函数以及参数，args必须和构造函数参数匹配
        ///     Deserializing the specified constructor and parameters, args must match the constructor parameters
        /// </summary>
        /// <param name="args">该构造函数的参数,The parameters of the constructor</param>
        public JsonDeserializeCtorAttribute(params object[] args)
        {
            _args = args;
        }
    }
}
