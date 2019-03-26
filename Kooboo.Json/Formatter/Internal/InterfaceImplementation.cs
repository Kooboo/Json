using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Kooboo.Json.Deserialize
{
    internal class InterfaceImplementation<TInterface>
    {
        public static readonly Type Proxy;

        static InterfaceImplementation()
        {
            try
            {
                var iType = typeof(TInterface);

                Random r = new Random();
                var className = iType.Name + "Impl" + r.Next(int.MinValue, int.MaxValue).ToString();
                while (AssemblyBuilderContainer.TypeNames.Contains(className))
                    className = iType.Name + "Impl" + r.Next(int.MinValue, int.MaxValue).ToString();

                var typeBuilder =
                    AssemblyBuilderContainer.ModBuilder.DefineType(
                       className,
                      TypeAttributes.Class | TypeAttributes.Sealed,
                        typeof(object),
                        new[] { iType }
                    );

                var allMembers = iType.GetAllInterfaceMembers();

                List<MethodInfo> propertyInfos = new List<MethodInfo>();

                foreach (var prop in allMembers.OfType<PropertyInfo>())
                {
                    var propType = ReturnType(prop);

                    var propBuilder = typeBuilder.DefineProperty(prop.Name, prop.Attributes, propType, Type.EmptyTypes);

                    var iGetter = prop.GetMethod;
                    var iSetter = prop.SetMethod;
                    if (iGetter != null)
                        propertyInfos.Add(iGetter);
                    if (iSetter != null)
                        propertyInfos.Add(iSetter);

                    if (prop.Name == "Item")
                    {
                        if (iGetter != null)
                        {
                            var accessor = iGetter.Attributes;
                            accessor &= ~MethodAttributes.Abstract;
                            var methBuilder = typeBuilder.DefineMethod(iGetter.Name, accessor, iGetter.ReturnType, iGetter.GetParameters().Select(e => e.ParameterType).ToArray());
                            var il = methBuilder.GetILGenerator();
                            il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                            il.Emit(OpCodes.Throw);
                            propBuilder.SetGetMethod(methBuilder);
                        }
                        if (iSetter != null)
                        {
                            var accessor = iSetter.Attributes;
                            accessor &= ~MethodAttributes.Abstract;
                            var methBuilder = typeBuilder.DefineMethod(iSetter.Name, accessor, iSetter.ReturnType, iSetter.GetParameters().Select(e => e.ParameterType).ToArray());
                            var il = methBuilder.GetILGenerator();
                            il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                            il.Emit(OpCodes.Throw);
                            propBuilder.SetSetMethod(methBuilder);
                        }
                        continue;
                    }


                    Func<FieldInfo> getBackingField;
                    {
                        FieldInfo backingField = null;
                        getBackingField =
                            () =>
                            {
                                if (backingField == null)
                                {
                                    backingField = typeBuilder.DefineField("_" + prop.Name + "_" + Guid.NewGuid(), ReturnType(prop), FieldAttributes.Private);
                                }

                                return backingField;
                            };
                    }

                    if (iGetter != null)
                    {
                        var accessor = iGetter.Attributes;
                        accessor &= ~MethodAttributes.Abstract;

                        var methBuilder = typeBuilder.DefineMethod(iGetter.Name, accessor, propType, Type.EmptyTypes);
                        var il = methBuilder.GetILGenerator();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, getBackingField());
                        il.Emit(OpCodes.Ret);
                        propBuilder.SetGetMethod(methBuilder);
                    }

                    if (iGetter != null || iSetter != null)
                    {
                        var accessor = iSetter != null ? iSetter.Attributes : MethodAttributes.Private;
                        var name = iSetter != null ? iSetter.Name : "set_" + prop.Name;

                        accessor &= ~MethodAttributes.Abstract;

                        var methBuilder = typeBuilder.DefineMethod(name, accessor, typeof(void), new[] { propType });
                        var il = methBuilder.GetILGenerator();

                        if (iGetter != null)
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Stfld, getBackingField());
                            il.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ret);
                        }

                        propBuilder.SetSetMethod(methBuilder);
                    }
                }

                foreach (var method in allMembers.OfType<MethodInfo>().Except(propertyInfos))
                {
                    var methBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, method.ReturnType, method.GetParameters().Select(e => e.ParameterType).ToArray());
                    if (method.IsGenericMethod)
                    {
                        methBuilder.DefineGenericParameters(method.GetGenericArguments().Select(e => e.Name).ToArray());
                    }
                    var il = methBuilder.GetILGenerator();
                    il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                    il.Emit(OpCodes.Throw);

                    typeBuilder.DefineMethodOverride(methBuilder, method);
                }

                var proxyInfo = typeBuilder.CreateTypeInfo();

                Proxy = proxyInfo.AsType();

                AssemblyBuilderContainer.TypeNames.Add(className);
            }
            catch (TypeLoadException)
            {
                throw new JsonNoSupportedDeserializeTypeException(typeof(TInterface), "Interface access properties must be public");
            }
            catch {
                throw new JsonNoSupportedDeserializeTypeException(typeof(TInterface));
            }
        }

        static Type ReturnType(MemberInfo m)
        {
            var asField = m as FieldInfo;
            var asProp = m as PropertyInfo;
            return
                asField != null ? asField.FieldType : asProp.PropertyType;
        }
    }

    static class AssemblyBuilderContainer
    {
        private static readonly AssemblyBuilder AsmBuilder;

        internal static readonly ModuleBuilder ModBuilder;

        internal static List<string> TypeNames = new List<string>();

        static AssemblyBuilderContainer()
        {
            AsmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("KoobooInterfaceTypeProxies"), AssemblyBuilderAccess.Run);
            ModBuilder = AsmBuilder.DefineDynamicModule("InterfaceProxies");
        }
    }


}
