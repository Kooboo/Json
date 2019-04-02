using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    internal class ResolveFind<T>
    {
        internal static ResolveDelegate<T> Find()
        {
            Type t = typeof(T);
            return FindTypeIsDefaultImplemented(t) ?? FindTypeIsMeetDefaultCondition(t) ?? FindTypeIsDefaultImplementedBaseType(t) ?? FindTypeIsDefaultImplementedInterface(t) ?? DefaultResolve(t);
        }

        private static ResolveDelegate<T> FindTypeIsDefaultImplemented(Type t)
        {
            if (DeserializeBootTable.Table.DefaultSameTypes.TryGetValue(t, out var member))
                return GenerateLambdaCall(member);
            return null;
        }

        private static ResolveDelegate<T> FindTypeIsMeetDefaultCondition(Type t)
        {
            return IsArray(t) ?? IsEnum(t) ?? IsDynamic(t);
        }

        private static ResolveDelegate<T> FindTypeIsDefaultImplementedBaseType(Type t)
        {
            var baseTypes = t.GetTypeAndBaseTypes();

            //Avoid
            if (DeserializeBootTable.Table.DefaultAvoidTypes.Intersect(baseTypes).Any())
                return GenerateLambdaCall(SpecialConditionsResolve._ReadAvoidNull);

            //BaseType
            foreach (var item in DeserializeBootTable.Table.DefaultImplementedBaseType)
            {
                var implementedType = item.Key;
                foreach (var baseType in baseTypes)
                {
                    if (baseType == implementedType)
                    {
                        return GenerateLambdaCall(item.Value.MethodInfo);
                    }
                }
            }
            return null;
        }

        private static ResolveDelegate<T> FindTypeIsDefaultImplementedInterface(Type t)
        {
            var intserfaces = t.GetInterfaces().ToList();
            if (t.IsInterface)
                intserfaces.Add(t);

            foreach (var item in DeserializeBootTable.Table.DefaultImplementedInterfaces)
            {
                var implementedInterfaces = item.Key;
                foreach (var objInterface in intserfaces)
                {
                    if (objInterface == implementedInterfaces)
                    {
                        return GenerateLambdaCall(item.Value.MethodInfo);
                    }
                }
            }
            return null;
        }

        private static ResolveDelegate<T> DefaultResolve(Type t)
        {
            Func<ResolveDelegate<T>> func = TypeIsDictionary(t) ?? TypeIsCollection(t) ?? TypeIsSpecial(t);
            if (func != null)
                return func();
            return BuildFactory.Build<T>(DeserializeBuildTypeEnum.KeyValueObject, t);
        }

        private static Func<ResolveDelegate<T>> TypeIsCollection(Type t)
        {
            return TypeIsCollectionInterface(t) ?? TypeIsCollectionType(t);
        }
        private static Func<ResolveDelegate<T>> TypeIsDictionary(Type t)
        {
            return TypeIsDictionaryInterface(t) ?? TypeIsDictionaryType(t);
        }
        private static Func<ResolveDelegate<T>> TypeIsSpecial(Type t)
        {
            if (t.IsGenericType)
            {
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                    return () => GenerateLambdaCall(SpecialConditionsResolve._ReadNullable.MakeGenericMethod(t.GetGenericArguments()[0]));
                else if (genericTypeDefinition == typeof(Lazy<>))
                {
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Lazy, t);
                }
                else if (genericTypeDefinition == typeof(Lookup<,>))
                {

                }
                else if (genericTypeDefinition == typeof(KeyValuePair<,>))
                {
                    var keyType = t.GetGenericArguments()[0];
                    var func = IsWrongKeyType(keyType);
                    if (func != null)
                        return () => func;
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.KeyValuePair, t);
                }
                else if (genericTypeDefinition == typeof(ILookup<,>))
                {


                }
            }

            return null;
        }

        private static Func<ResolveDelegate<T>> TypeIsCollectionInterface(Type t)
        {
            if (t == typeof(IEnumerable) || t == typeof(ICollection) || t == typeof(IList))
                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Collection, t, typeof(List<object>), typeof(List<object>), typeof(object));

            if (t.IsGenericType)
            {
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                var arrayItemType = t.GetGenericArguments()[0];

                if (genericTypeDefinition == typeof(IEnumerable<>) || genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>))
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Collection, t, typeof(List<>).MakeGenericType(arrayItemType), typeof(List<>).MakeGenericType(arrayItemType), arrayItemType);

                if (genericTypeDefinition == typeof(IReadOnlyList<>) || genericTypeDefinition == typeof(IReadOnlyCollection<>))
                {
                    var ctor = typeof(ReadOnlyCollection<>).MakeGenericType(arrayItemType).GetCtorByParameterInterfaceType(typeof(IList<>).MakeGenericType(arrayItemType));
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, typeof(ReadOnlyCollection<>).MakeGenericType(arrayItemType), ctor);
                }

                if (genericTypeDefinition == typeof(ISet<>))
                {
                    var ctor = typeof(HashSet<>).MakeGenericType(arrayItemType).GetCtorByParameterInterfaceType(typeof(IEnumerable<>).MakeGenericType(arrayItemType));
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, typeof(HashSet<>).MakeGenericType(arrayItemType), ctor);
                }
            }

            return null;
        }
        private static Func<ResolveDelegate<T>> TypeIsDictionaryInterface(Type t)
        {
            if (t == typeof(IDictionary))
                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Dictionary, t, typeof(Dictionary<string, object>), typeof(Dictionary<string, object>), typeof(string), typeof(object));

            if (t.IsGenericType)
            {
                Type[] args = t.GetGenericArguments();
                if (args.Length != 2)
                    return null;
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                var keyType = args[0];
                var valueType = args[1];

                if (genericTypeDefinition == typeof(IDictionary<,>))
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Dictionary, t, typeof(Dictionary<,>).MakeGenericType(keyType, valueType), t, keyType, valueType);

                if (genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                {
                    var ctor = typeof(ReadOnlyDictionary<,>).MakeGenericType(keyType, valueType).GetCtorByParameterInterfaceType(typeof(IDictionary<,>).MakeGenericType(keyType, valueType));
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, typeof(ReadOnlyDictionary<,>).MakeGenericType(keyType, valueType), ctor);
                }
            }

            return null;
        }
        private static Func<ResolveDelegate<T>> TypeIsCollectionType(Type t)
        {
            if (t.IsInterface)
                return null;

            var intserfaces = t.GetInterfaces();

            bool hasIEnumerableGeneric = false;
            bool hasICollectionGeneric = false;
            bool hasICollection = false;
            bool hasIList = false;

            Type arrayItemType = null;
            Type iCollectionGenericType = null;
            Type iEnumerableGenericType = null;
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
                }
                else if (item == typeof(ICollection))
                    hasICollection = true;
                else if (item == typeof(IList))
                    hasIList = true;
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
                                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, firstCtor.ParameterType, item);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0) //如果空的构造函数，则调用接口Add方法
                {
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Collection, t, t, iCollectionGenericType, arrayItemType);
                }
                else //如果构造函数为iCollectionGenericType，则注入iCollectionGenericType
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, iCollectionGenericType, ctor);
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
                                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, firstCtor.ParameterType, item);
                            }
                        }
                    }
                }
                else
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, iEnumerableGenericType, ctor);
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
                                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, firstCtor.ParameterType, item);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0) //如果空的构造函数，则调用接口Add方法
                {
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Collection, t, t, typeof(IList), typeof(object));
                }
                else //如果构造函数为iCollectionGenericType，则注入iCollectionGenericType
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, typeof(IList), ctor);
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
                                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, firstCtor.ParameterType, item);
                            }
                        }
                    }
                }
                else
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, typeof(ICollection), ctor);

            }


            return null;
        }
        private static Func<ResolveDelegate<T>> TypeIsDictionaryType(Type t)
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
                                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, firstCtor.ParameterType, item);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0)
                {
                    return () => IsWrongKeyType(keyType) ?? BuildFactory.Build<T>(DeserializeBuildTypeEnum.Dictionary, t, t, iDictionaryGenericType, keyType, valueType);
                }
                else
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, iDictionaryGenericType, ctor);
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
                                return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, firstCtor.ParameterType, item);
                            }
                        }
                    }
                }
                else if (ctor.GetParameters().Length == 0)
                {
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.Dictionary, t, t, typeof(IDictionary), typeof(string), typeof(object));
                }
                else
                    return () => BuildFactory.Build<T>(DeserializeBuildTypeEnum.CtorInject, t, typeof(IDictionary), ctor);
            }


            return null;
        }

        private static ResolveDelegate<T> IsArray(Type t)
        {
            if (t.IsArray)
            {
                int rank = t.GetArrayRank();
                if (rank == 1)
                    return BuildFactory.Build<T>(DeserializeBuildTypeEnum.Array, t);
                else
                {
                    Type sawtoothType = t.GetElementType();
                    int i = rank;
                    while (i-- > 0)
                    {
                        var obj = Array.CreateInstance(sawtoothType, 0);
                        sawtoothType = obj.GetType();
                    }
                    return BuildFactory.Build<T>(DeserializeBuildTypeEnum.MultiArray, t, sawtoothType, rank);
                }
            }
            return null;
        }
        private static ResolveDelegate<T> IsEnum(Type t)
        {
            if (t.IsEnum)
                return GenerateLambdaCallWithEnqueueType(SpecialConditionsResolve._ReadEnum, t);
            return null;
        }
        private static ResolveDelegate<T> IsDynamic(Type t)
        {
            if (t.IsAnonymousType())
                return BuildFactory.Build<T>(DeserializeBuildTypeEnum.Dynamic, t);
            return null;
        }


        private static ResolveDelegate<T> IsWrongKeyType(Type keyType)
        {
            if (keyType.IsWrongKey())
                return BuildFactory.Build<T>(DeserializeBuildTypeEnum.WrongGenericKey);
            return null;
        }
        private static ResolveDelegate<T> GenerateLambdaCall(MethodInfo methodInfo)
        {
            ParameterExpression readerArg = Expression.Parameter(typeof(JsonReader), "reader");
            ParameterExpression handlerArg = Expression.Parameter(typeof(JsonDeserializeHandler), "handler");
            var body = Expression.Call(methodInfo, readerArg, handlerArg);
            return Expression.Lambda<ResolveDelegate<T>>(body, readerArg, handlerArg).Compile();
        }
        private static ResolveDelegate<T> GenerateLambdaCallWithEnqueueType(MethodInfo methodInfo, Type t)
        {
            ParameterExpression readerArg = Expression.Parameter(typeof(JsonReader), "reader");
            ParameterExpression handlerArg = Expression.Parameter(typeof(JsonDeserializeHandler), "handler");
            MemberExpression option = Expression.MakeMemberAccess(handlerArg, JsonDeserializeHandler._Types);
            LabelTarget label = Expression.Label(t);
            var body = Expression.Block(Expression.Call(option, typeof(Queue<Type>).GetMethod("Enqueue"), Expression.Constant(t, typeof(Type)))
               , Expression.Return(label, Expression.Convert(Expression.Call(methodInfo, readerArg, handlerArg), t)),
                 Expression.Label(label, Expression.Convert(Expression.Call(methodInfo, readerArg, handlerArg), t))
               );
            return Expression.Lambda<ResolveDelegate<T>>(body, readerArg, handlerArg).Compile();
        }
    }
}
