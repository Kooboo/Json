using System;
using System.Reflection;

namespace Kooboo.Json
{
    internal class MemberExtension
    {
        internal Type Type { get; set; }
        internal string Name { get; set; }
        internal MemberInfo MemberInfo { get; set; }
        internal FieldInfo FieldInfo { get; set; }
        internal PropertyInfo PropertyInfo { get; set; }
        internal bool IsProperty { get; set; }
        internal int OrderNum { get; set; }

        internal MemberExtension(PropertyInfo pro)
        {
            MemberInfo = pro;
            PropertyInfo = pro;
            Name = pro.Name;
            IsProperty = true;
            Type = pro.PropertyType;
        }

        internal MemberExtension(FieldInfo pro)
        {
            MemberInfo = pro;
            FieldInfo = pro;
            Name = pro.Name;
            IsProperty = false;
            Type = pro.FieldType;
        }
    }
}
