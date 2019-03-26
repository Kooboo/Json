using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Serializer
{
    internal class FormatterFind<T>
    {
        internal static Action<T, JsonSerializerHandler> Find()
        {
            Type t = typeof(T);
            return FindTypeIsDefaultImplemented(t) ?? FindTypeIsMeetDefaultCondition(t) ?? FindTypeIsDefaultImplementedBaseType(t) ?? FindTypeIsDefaultImplementedInterface(t) ?? DefaultResolve(t);
        }

        private static Action<T, JsonSerializerHandler> FindTypeIsDefaultImplemented(Type t)
        {
            if (SerializerBootTable.Table.DefaultSameTypes.TryGetValue(t, out MethodInfo value))
                return GenerateLambdaCall(value);
            return null;
        }

        private static Action<T, JsonSerializerHandler> FindTypeIsMeetDefaultCondition(Type t)
        {
            return IsLoadDynamic(t) ?? IsArray(t) ?? IsEnum(t);
        }

        private static Action<T, JsonSerializerHandler> FindTypeIsDefaultImplementedBaseType(Type t)
        {
            var baseTypes = t.GetTypeAndBaseTypes();

            //Avoid
            if (SerializerBootTable.Table.DefaultAvoidTypes.Intersect(baseTypes).Any())
                return (a, b) => SpecialConditions.AvoidTypes(a, b);

            //Default
            foreach (var implementedType in SerializerBootTable.Table.DefaultImplementedBaseType)
            {
                foreach (var baseType in baseTypes)
                {
                    if (baseType == implementedType.Key)
                    {
                        return GenerateLambdaCall(implementedType.Value.MethodInfo);
                    }
                }
            }
            return null;
        }

        private static Action<T, JsonSerializerHandler> FindTypeIsDefaultImplementedInterface(Type t)
        {
            var intserfaces = t.GetInterfaces().ToList();
            if (t.IsInterface)
                intserfaces.Add(t);

            foreach (var item in SerializerBootTable.Table.DefaultImplementedInterfaces)
            {
                foreach (var objInterface in intserfaces)
                {
                    if (objInterface == item.Key)
                    {
                        return GenerateLambdaCall(item.Value.MethodInfo);
                    }
                }
            }
            return null;
        }

        private static Action<T, JsonSerializerHandler> DefaultResolve(Type t)
        {
            Func<Action<T, JsonSerializerHandler>> func = TypeIsDictionary(t) ?? TypeIsCollection(t) ?? TypeIsSpecial(t);
            if (func != null)
                return func();

            return BuildFactory.Create<T>(SerializerBuildTypeEnum.KeyValueObject);
        }

        private static Func<Action<T, JsonSerializerHandler>> TypeIsDictionary(Type t)
        {
            return TypeIsDictionaryInterface(t) ?? TypeIsDictionaryType(t);
        }
        private static Func<Action<T, JsonSerializerHandler>> TypeIsDictionaryInterface(Type t)
        {
            if (t == typeof(IDictionary))
                return () => (obj, handler) => SpecialConditions.WriteDictionary((IDictionary)obj, handler);

            if (t.IsGenericType)
            {
                Type[] args = t.GetGenericArguments();
                if (args.Length != 2)
                    return null;
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                var keyType = args[0];
                var valueType = args[1];

                if (genericTypeDefinition == typeof(IDictionary<,>) || genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                    return () => IsWrongKeyType(keyType) ?? BuildFactory.Create<T>(SerializerBuildTypeEnum.IDictionaryGeneric);

            }

            return null;
        }
        private static Func<Action<T, JsonSerializerHandler>> TypeIsDictionaryType(Type t)
        {
            if (t.IsInterface)
                return null;

            var intserfaces = t.GetInterfaces();

            bool hasIDictionaryGeneric = false;
            bool hasIDictionary = false;
            Type iDictionaryGenericType = null;

            Type keyType = null;
            Type valueType = null;
            foreach (var item in intserfaces)
            {
                if (item.IsGenericType)
                {
                    var genericTypeDefinition = item.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(IDictionary<,>))
                    {
                        iDictionaryGenericType = item;
                        hasIDictionaryGeneric = true;
                        keyType = item.GetGenericArguments()[0];
                        valueType = item.GetGenericArguments()[1];
                    }
                }
                if (item == typeof(IDictionary))
                    hasIDictionary = true;
            }

            if (hasIDictionaryGeneric)
            {
                var ctor = t.GetDefaultNoArgCtorOrAppointTypeCtor(iDictionaryGenericType);
                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (TypeIsDictionary(firstCtor.ParameterType) != null)
                            {
                                return () => IsWrongKeyType(keyType) ?? BuildFactory.Create<T>(SerializerBuildTypeEnum.IDictionaryGeneric);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0)
                {
                    return () => IsWrongKeyType(keyType) ?? BuildFactory.Create<T>(SerializerBuildTypeEnum.IDictionaryGeneric);
                }
                else
                    return () => IsWrongKeyType(keyType) ?? BuildFactory.Create<T>(SerializerBuildTypeEnum.IDictionaryGeneric);
            }
            if (hasIDictionary)
            {
                var ctor = t.GetDefaultNoArgCtorOrAppointTypeCtor(typeof(IDictionary));

                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (TypeIsDictionary(firstCtor.ParameterType) != null)
                            {
                                return () => (obj, handler) => SpecialConditions.WriteDictionary((IDictionary)obj, handler);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0)
                {
                    return () => (obj, handler) => SpecialConditions.WriteDictionary((IDictionary)obj, handler);
                }
                else
                    return () => (obj, handler) => SpecialConditions.WriteDictionary((IDictionary)obj, handler);
            }


            return null;
        }

        private static Func<Action<T, JsonSerializerHandler>> TypeIsCollection(Type t)
        {
            return TypeIsCollectionInterface(t) ?? TypeIsCollectionType(t);
        }
        private static Func<Action<T, JsonSerializerHandler>> TypeIsCollectionInterface(Type t)
        {
            if (t == typeof(IEnumerable) || t == typeof(ICollection) || t == typeof(IList))
                return () => (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);

            if (t.IsGenericType)
            {
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                var arrayItemType = t.GetGenericArguments()[0];

                if (genericTypeDefinition == typeof(IEnumerable<>) || genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(ISet<>) || genericTypeDefinition == typeof(IReadOnlyList<>) || genericTypeDefinition == typeof(IReadOnlyCollection<>))
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IEnumerableGeneric);
            }

            return null;
        }
        private static Func<Action<T, JsonSerializerHandler>> TypeIsCollectionType(Type t)
        {
            if (t.IsInterface)
                return null;

            var intserfaces = t.GetInterfaces();

            bool hasIEnumerableGeneric = false;
            bool hasICollectionGeneric = false;
            bool hasIListGeneric = false;
            bool hasICollection = false;
            bool hasIList = false;

            Type arrayItemType = null;
            Type iCollectionGenericType = null;
            Type iEnumerableGenericType = null;
            Type iListGenericType = null;
            foreach (var item in intserfaces)
            {
                if (item.IsGenericType)
                {
                    var genericTypeDefinition = item.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof(IEnumerable<>))
                    {
                        hasIEnumerableGeneric = true;
                        arrayItemType = item.GetGenericArguments()[0];
                        iEnumerableGenericType = item;
                    }
                    else if (genericTypeDefinition == typeof(ICollection<>))
                    {
                        hasICollectionGeneric = true;
                        arrayItemType = item.GetGenericArguments()[0];
                        iCollectionGenericType = item;
                    }
                    else if (genericTypeDefinition == typeof(IList<>))
                    {
                        hasIListGeneric = true;
                        arrayItemType = item.GetGenericArguments()[0];
                        iListGenericType = item;
                    }
                }
                else if (item == typeof(ICollection))
                    hasICollection = true;
                else if (item == typeof(IList))
                    hasIList = true;
            }

            if (hasIListGeneric)
            {
                var ctor = t.GetDefaultNoArgCtorOrAppointTypeCtor(iListGenericType);
                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (firstCtor.ParameterType.IsArray || TypeIsCollection(firstCtor.ParameterType) != null)
                            {
                                return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IListGeneric);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0)
                {
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IListGeneric);
                }
                else
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IListGeneric);
            }


            if (hasICollectionGeneric)
            {
                var ctor = t.GetDefaultNoArgCtorOrAppointTypeCtor(iCollectionGenericType);
                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (firstCtor.ParameterType.IsArray || TypeIsCollection(firstCtor.ParameterType) != null)
                            {
                                return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IEnumerableGeneric);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0) 
                {
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IEnumerableGeneric);
                }
                else 
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IEnumerableGeneric);
            }

            if (hasIEnumerableGeneric && hasICollection)
            {
                var ctor = t.GetAppointTypeCtor(iEnumerableGenericType);
                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (firstCtor.ParameterType.IsArray || TypeIsCollection(firstCtor.ParameterType) != null)
                            {
                                return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IEnumerableGeneric);
                            }
                        }
                    }
                }
                else
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.IEnumerableGeneric);
            }

            if (hasIList)
            {
                var ctor = t.GetDefaultNoArgCtorOrAppointTypeCtor(typeof(IList));
                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (firstCtor.ParameterType.IsArray || TypeIsCollection(firstCtor.ParameterType) != null)
                            {
                                return () => (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0)
                {
                    return () => (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);
                }
                else
                    return () => (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);
            }

            if (hasICollection)
            {
                var ctor = t.GetAppointTypeCtor(typeof(ICollection));
                if (ctor == null)
                {
                    foreach (var item in t.GetConstructors())
                    {
                        if (item.GetParameters().Length == 1)
                        {
                            var firstCtor = item.GetParameters()[0];
                            if (firstCtor.ParameterType.IsArray || TypeIsCollection(firstCtor.ParameterType) != null)
                            {
                                return () => (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);
                            }
                        }
                    }
                }
                else
                    return () => (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);

            }


            return null;
        }

        private static Func<Action<T, JsonSerializerHandler>> TypeIsSpecial(Type t)
        {
            if (t.IsGenericType)
            {
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(KeyValuePair<,>))
                {
                    var keyType = t.GetGenericArguments()[0];
                    return () => IsWrongKeyType(keyType) ?? BuildFactory.Create<T>(SerializerBuildTypeEnum.KeyValuePair);
                }
                if (genericTypeDefinition == typeof(Nullable<>))
                    return () => BuildFactory.Create<T>(SerializerBuildTypeEnum.Nullable);
                if (genericTypeDefinition == typeof(Lazy<>))
                    return() => BuildFactory.Create<T>(SerializerBuildTypeEnum.Lazy);
                if (genericTypeDefinition == typeof(Lookup<,>))
                {
                   
                }
                if (genericTypeDefinition == typeof(ILookup<,>))
                {

                }
            }
            return null;
        }

        private static Action<T, JsonSerializerHandler> IsArray(Type t)
        {
            if (t.IsArray)
            {
                if (t.GetArrayRank() == 1)
                    return BuildFactory.Create<T>(SerializerBuildTypeEnum.IListGeneric);
                else
                    return (obj, handler) => SpecialConditions.WriteCollection((IEnumerable)obj, handler);
            }
            return null;
        }
        private static Action<T, JsonSerializerHandler> IsLoadDynamic(Type t)
        {
            if (t.IsAnonymousType())
                return (a, b) => SpecialConditions.WriteDynamic(a, b);
            return null;
        }
        private static Action<T, JsonSerializerHandler> IsEnum(Type t)
        {
            if (t.IsEnum)
                return GenerateLambdaCallWithTypeConvert(SpecialConditions._WriteEnum, typeof(Enum));
            return null;
        }


        private static Action<T, JsonSerializerHandler> IsWrongKeyType(Type keyType)
        {
            if (keyType.IsWrongKey())
                return BuildFactory.Create<T>(SerializerBuildTypeEnum.WrongGenericKey);
            return null;
        }
        private static Action<T, JsonSerializerHandler> GenerateLambdaCall(MethodInfo method)
        {
            ParameterExpression objArg = Expression.Parameter(typeof(T), "instance");
            ParameterExpression handlerArg = Expression.Parameter(typeof(JsonSerializerHandler), "handler");
            var body = Expression.Call(method, objArg, handlerArg);
            return Expression.Lambda<Action<T, JsonSerializerHandler>>(body, objArg, handlerArg).Compile();
        }
        private static Action<T, JsonSerializerHandler> GenerateLambdaCallWithTypeConvert(MethodInfo method, Type t)
        {
            ParameterExpression objArg = Expression.Parameter(typeof(T), "instance");
            ParameterExpression handlerArg = Expression.Parameter(typeof(JsonSerializerHandler), "handler");
            var body = Expression.Call(method, Expression.Convert(objArg, t), handlerArg);
            return Expression.Lambda<Action<T, JsonSerializerHandler>>(body, objArg, handlerArg).Compile();
        }
    }
}
