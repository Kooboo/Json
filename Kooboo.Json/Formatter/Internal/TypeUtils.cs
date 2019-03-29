using Kooboo.Json.Deserialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kooboo.Json
{
    internal enum ReadMemberStateEnum
    {
        All,
        CanRead,
        CanWrite
    }
    internal static class TypeUtils
    {
        internal static bool IsAnonymousType(this Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
             && type.Name.Contains("AnonymousType")
             && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
             && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        internal static List<KeyValuePair<string, MemberExtension>> GetModelMembers(this Type type, ReadMemberStateEnum readMemberStateEnum = ReadMemberStateEnum.All)
        {
            List<KeyValuePair<string, MemberExtension>> list = new List<KeyValuePair<string, MemberExtension>>();
            List<KeyValuePair<string, MemberExtension>> jsonOnlyIncludeLists = new List<KeyValuePair<string, MemberExtension>>();
            Type ignoreAttribute = typeof(IgnoreKeyAttribute);
            List<PropertyInfo> pors = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            if (type.IsInterface)
            {
                Type[] interfaces = type.GetInterfaces();
                if (interfaces.Length > 0)
                {
                    foreach (var item in interfaces)
                    {
                        pors.AddRange(item.GetProperties());
                    }
                }
            }
            foreach (var item in pors)
            {
                if (item.IsDefined(ignoreAttribute, false))
                    continue;
                if (item.GetIndexParameters().Length > 0)
                    continue;
                if (readMemberStateEnum == ReadMemberStateEnum.CanRead)
                {
                    if (!item.CanRead)
                        continue;
                }
                else if (readMemberStateEnum == ReadMemberStateEnum.CanWrite)
                {
                    if (!item.CanWrite)
                        continue;
                }
                var mem = new MemberExtension(item);

                //alias
                AliasAttribute alias = item.GetCustomAttribute<AliasAttribute>(false);
                var name = alias != null ? alias._name : mem.Name;

                //order
                JsonOrderAttribute order = item.GetCustomAttribute<JsonOrderAttribute>(false);
                mem.OrderNum = order?._orderNum ?? ushort.MaxValue + 1;

                //jsonOnlyIncludeAttribute
                JsonOnlyIncludeAttribute jsonMember = item.GetCustomAttribute<JsonOnlyIncludeAttribute>(false);
                if (jsonMember != null)
                    jsonOnlyIncludeLists.Add(new KeyValuePair<string, MemberExtension>(name, mem));

                list.Add(new KeyValuePair<string, MemberExtension>(name, mem));
            }

            FieldInfo[] fils = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in fils)
            {
                if (item.IsDefined(ignoreAttribute, false))
                    continue;
                var mem = new MemberExtension(item);

                //alias
                AliasAttribute alias = item.GetCustomAttribute<AliasAttribute>(false);
                var name = alias != null ? alias._name : mem.Name;

                //order
                JsonOrderAttribute order = item.GetCustomAttribute<JsonOrderAttribute>(false);
                mem.OrderNum = order?._orderNum ?? ushort.MaxValue + 1;

                //jsonOnlyIncludeAttribute
                JsonOnlyIncludeAttribute jsonMember = item.GetCustomAttribute<JsonOnlyIncludeAttribute>(false);
                if (jsonMember != null)
                    jsonOnlyIncludeLists.Add(new KeyValuePair<string, MemberExtension>(name, mem));

                list.Add(new KeyValuePair<string, MemberExtension>(name, mem));
            }

            if (jsonOnlyIncludeLists.Count > 0)
            {
                jsonOnlyIncludeLists = jsonOnlyIncludeLists.OrderBy(e => e.Value.OrderNum).ToList();
                return jsonOnlyIncludeLists;
            }

            list = list.OrderBy(e => e.Value.OrderNum).ToList();
            return list;
        }

        internal static CharTries GetCharTries(this Type type)
        {
            CharTries charTries = new CharTries() { IsPeak = true };
            type.GetModelMembers(ReadMemberStateEnum.All).ForEach(
                e =>
                    charTries.Insert(e.Key, e.Value)
                );
            return charTries;
        }

        internal static List<Type> GetTypeAndBaseTypes(this Type type)
        {
            List<Type> types = new List<Type>();
            if (type != null)
            {
                types.Add(type);
                types.AddRange(GetBaseTypes(type));
            }
            return types;
        }

        internal static List<Type> GetBaseTypes(this Type type)
        {
            List<Type> types = new List<Type>();
            if (type != null)
            {
                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    types.Add(type.BaseType);
                    types.AddRange(GetBaseTypes(type.BaseType));
                }
            }

            return types;
        }

        internal static List<MemberInfo> GetAllInterfaceMembers(this Type type)
        {
            if (!type.IsInterface) throw new Exception("Expected interface, found: " + type);
            var pending = new Stack<Type>();
            pending.Push(type);
            var ret = new List<MemberInfo>();
            while (pending.Count > 0)
            {
                var current = pending.Pop();

                ret.AddRange(current.GetMembers());

                if (current.BaseType != null)
                {
                    pending.Push(current.BaseType);
                }

                foreach (var x in current.GetInterfaces())
                {
                    pending.Push(x);
                }
            }
            return ret;
        }

        internal static ConstructorInfo GetValueTypeCtor(this Type type, ref List<Expression> args)
        {

            var cs = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in cs)
            {
                var pars = item.GetParameters();
                JsonDeserializeCtorAttribute jsonDeserializeCtorAttribute = item.GetCustomAttribute<JsonDeserializeCtorAttribute>();
                if (jsonDeserializeCtorAttribute != null)
                {
                    args = new List<Expression>();

                    for (int i = 0; i < jsonDeserializeCtorAttribute._args.Length; i++)
                    {
                        args.Add(Expression.Constant(jsonDeserializeCtorAttribute._args[i], pars[i].ParameterType));
                    }
                    return item;
                }
            }
            return null;
        }

        internal static ConstructorInfo GetClassCtor(this Type type, ref List<Expression> args)
        {
            var cs = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo attrCtor = null, publicNoArgsCtor = null, privateNoArgsCtor = null, publicArgsCtor = null, privateArgsCtor = null;
            List<Expression> attrCtorArgs = null; ParameterInfo[] publicArgsCtorParas = null, privateArgsCtorParas = null;
            foreach (var item in cs)
            {
                var pars = item.GetParameters();
                JsonDeserializeCtorAttribute jsonDeserializeCtorAttribute = item.GetCustomAttribute<JsonDeserializeCtorAttribute>();
                if (jsonDeserializeCtorAttribute != null)
                {
                    attrCtorArgs = new List<Expression>();
                    attrCtor = item;

                    for (int i = 0; i < jsonDeserializeCtorAttribute._args.Length; i++)
                    {
                        attrCtorArgs.Add(Expression.Constant(jsonDeserializeCtorAttribute._args[i], pars[i].ParameterType));
                    }

                    break;
                }

                if (item.IsPublic)
                {
                    if (pars.Length == 0)
                    {
                        publicNoArgsCtor = item;
                    }
                    else if (publicArgsCtor == null || pars.Length < publicArgsCtor.GetParameters().Length)
                    {
                        publicArgsCtor = item;
                        publicArgsCtorParas = pars;
                    }
                }
                else if (item.IsPrivate)
                {
                    if (pars.Length == 0)
                    {
                        privateNoArgsCtor = item;
                    }
                    else if (privateArgsCtor == null || pars.Length < privateArgsCtor.GetParameters().Length)
                    {
                        privateArgsCtor = item;
                        privateArgsCtorParas = pars;
                    }
                }
            }


            if (attrCtor != null)
            {
                args = attrCtorArgs;
                return attrCtor;
            }
            if (publicNoArgsCtor != null)
            {
                args = null;
                return publicNoArgsCtor;
            }
            if (privateNoArgsCtor != null)
            {
                args = null;
                return privateNoArgsCtor;
            }
            if (publicArgsCtor != null)
            {
                args = new List<Expression>();
                for (int i = 0; i < publicArgsCtorParas.Length; i++)
                {
                    object value = null;
                    var paraType = publicArgsCtorParas[i].ParameterType;
                    if (paraType.IsValueType)
                        value = Activator.CreateInstance(paraType);
                    args.Add(Expression.Constant(value, paraType));
                }
                return publicArgsCtor;
            }
            else //(privateArgsCtor != null)
            {
                args = new List<Expression>();
                for (int i = 0; i < publicArgsCtorParas.Length; i++)
                {
                    object value = null;
                    var paraType = publicArgsCtorParas[i].ParameterType;
                    if (paraType.IsValueType)
                        value = Activator.CreateInstance(paraType);
                    args.Add(Expression.Constant(value, paraType));
                }
                return privateArgsCtor;
            }
        }

        internal static ConstructorInfo GetCtorByParameterInterfaceType(this Type type, params Type[] ctorParaTypes)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var ctorParas = ctor.GetParameters();
                if (ctorParas.Length == ctorParaTypes.Length)
                {
                    bool isTrue = true;
                    for (int i = 0; i < ctorParaTypes.Length; i++)
                    {
                        if (ctorParas[i].ParameterType != ctorParaTypes[i])
                        {
                            isTrue = false;
                            break;
                        }

                    }
                    if (isTrue)
                        return ctor;
                }
            }
            return null;
        }

        internal static ConstructorInfo GetDefaultNoArgCtorOrAppointTypeCtor(this Type type, Type ctorParaTypes = null)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var ctorParas = ctor.GetParameters();
                if (ctorParas.Length == 0)
                    return ctor;//no args
                if (ctorParaTypes != null && ctorParas.Length == 1)
                {
                    if (ctorParas[0].ParameterType == ctorParaTypes)
                        return ctor;
                }
            }
            return null;
        }

        internal static ConstructorInfo GetAppointTypeCtor(this Type type, Type ctorParaTypes)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var ctorParas = ctor.GetParameters();
                if (ctorParas.Length == 1 && ctorParas[0].ParameterType == ctorParaTypes)
                {
                    return ctor;
                }
            }
            return null;
        }

        internal static bool HasEmptyCtor(this Type type)
        {
            if (type.IsInterface)
                return false;
            foreach (var ctor in type.GetConstructors())
            {
                var ctorParas = ctor.GetParameters();
                if (ctorParas.Length == 0)
                    return true;
            }
            return false;
        }

        internal static bool IsWrongKey(this Type keyType)
        {
            if (!keyType.IsPrimitive && !keyType.IsEnum && keyType != typeof(string) && keyType != typeof(Guid) && keyType != typeof(DateTime))
                return true;
            return false;
        }


        internal static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
