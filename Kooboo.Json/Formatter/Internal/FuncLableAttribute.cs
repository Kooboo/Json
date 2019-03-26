using System;

namespace Kooboo.Json
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class FuncLableAttribute : Attribute
    {
        public FuncType _Type;
        public int _Priority;
        public FuncLableAttribute(FuncType funcType)
        {
            _Type = funcType;
        }

        public FuncLableAttribute(FuncType funcType, int priority)
        {
            _Type = funcType;
            _Priority = priority;
        }
    }
}
