using System;

namespace Kooboo.Json
{
    /// <summary>
    ///     别名,标记于字段或属性上的特性
    ///     Alias,Characteristics marked on fields or property
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AliasAttribute : Attribute
    {
        internal string _name { get; set; }
        /// <summary>
        ///     Structural aliases
        /// </summary>
        /// <param name="name"></param>
        public AliasAttribute(string name)
        {
            _name = name;
        }
    }
}
