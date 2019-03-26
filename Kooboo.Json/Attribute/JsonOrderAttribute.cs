using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     元素排序,标记于字段或属性上的特性
    ///     Element sort, a feature marked on a field or property
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonOrderAttribute: Attribute
    {
        internal ushort _orderNum { get; set; }
        /// <summary>
        ///     JsonOrder
        /// </summary>
        /// <param name="orderNum">Order number</param>
        public JsonOrderAttribute(ushort orderNum)
        {
            _orderNum = orderNum;
        }
    }
}
