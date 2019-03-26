using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Json
{
    internal class BootTable
    {
        internal Dictionary<Type, MethodInfo> DefaultSameTypes = new Dictionary<Type, MethodInfo>();
        internal List<KeyValuePair<Type, MethodInfoOrder>> DefaultImplementedInterfaces = new List<KeyValuePair<Type, MethodInfoOrder>>();
        internal List<KeyValuePair<Type, MethodInfoOrder>> DefaultImplementedBaseType = new List<KeyValuePair<Type, MethodInfoOrder>>();

        internal HashSet<Type> DefaultAvoidTypes;
        internal List<Type> ExpressionBuildTypes = new List<Type>();

        public BootTable(BootTableTypeEnum bootTableTypeEnum)
        {
            Type normalBaseType = null;
            Type expressBaseTypes = null;
            switch (bootTableTypeEnum)
            {
                case BootTableTypeEnum.DeserializeResolve:
                    normalBaseType = typeof(Deserialize.DefaultJsonResolve);
                    expressBaseTypes = typeof(Deserialize.ExpressionJsonResolve);
                    break;
                case BootTableTypeEnum.SerializerLogic:
                    normalBaseType = typeof(Serializer.DefaultJsonFormatter);
                    expressBaseTypes = typeof(Serializer.ExpressionJsonFormatter);
                    break;
            }
            var normalMethods = new List<MethodInfo>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType == expressBaseTypes)
                    ExpressionBuildTypes.Add(type);
                if (type.BaseType == normalBaseType)
                    normalMethods.AddRange(type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic));
            }

            foreach (var item in normalMethods)
            {
                var atr = item.GetCustomAttribute<FuncLableAttribute>();
                if (atr == null)
                    continue;

                Type t = null;
                switch (bootTableTypeEnum)
                {
                    case BootTableTypeEnum.DeserializeResolve:
                        t = item.ReturnType;
                        break;
                    case BootTableTypeEnum.SerializerLogic:
                        t = item.GetParameters()[0].ParameterType;
                        break;
                }

                switch (atr._Type)
                {
                    case FuncType.SameType:
                        DefaultSameTypes.Add(t, item);
                        break;
                    case FuncType.BaseType:
                        DefaultImplementedBaseType.Add(new KeyValuePair<Type, MethodInfoOrder>(t, new MethodInfoOrder(item, atr._Priority)));
                        break;
                    case FuncType.Interface:
                        DefaultImplementedInterfaces.Add(new KeyValuePair<Type, MethodInfoOrder>(t, new MethodInfoOrder(item, atr._Priority)));
                        break;
                }
            }
            DefaultImplementedBaseType = DefaultImplementedBaseType.OrderBy(e => e.Value.Priority).ToList();
            DefaultImplementedInterfaces = DefaultImplementedInterfaces.OrderBy(e => e.Value.Priority).ToList();


            DefaultAvoidTypes = new HashSet<Type>() {
                typeof(Process),
                typeof(Delegate),
                typeof(Thread),
                typeof(Task)
            };
        }
    }
}
