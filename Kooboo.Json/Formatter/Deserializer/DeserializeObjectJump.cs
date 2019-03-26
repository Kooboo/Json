using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace Kooboo.Json.Deserialize
{
    internal class DeserializeObjectJump
    {
        private static  SpinLock _spinLock = new SpinLock();

        private static readonly Dictionary<Type, Func<string, JsonDeserializeHandler, object>> JumpFuncs =
            new Dictionary<Type, Func<string, JsonDeserializeHandler, object>>();

        internal static object GetThreadSafetyJumpFunc(string json, Type t, JsonDeserializeHandler handler)
        {
            if (JumpFuncs.TryGetValue(t, out var fc))
                return fc(json, handler);
            else
            {
                try
                {
                    bool i = false;
                    _spinLock.Enter(ref i);
                    if (!JumpFuncs.TryGetValue(t, out fc))
                    {
                        fc = GenerateFunc(t);
                        JumpFuncs.Add(t, fc);
                    }
                    return fc(json, handler);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    _spinLock.Exit();
                }

            }
        }

        internal static Func<string, JsonDeserializeHandler, object> GenerateFunc(Type t)
        {
            var genericType = typeof(ResolveProvider<>).MakeGenericType(t);
            var jsonEx = Expression.Parameter(typeof(string), "json");
            var handlerEx = Expression.Parameter(typeof(JsonDeserializeHandler), "handler");
            var body = Expression.Convert(Expression.Call(genericType.GetMethod("InvokeGet"), jsonEx, handlerEx), typeof(object));
            return Expression.Lambda<Func<string, JsonDeserializeHandler, object>>(body, jsonEx, handlerEx).Compile();
        }

    }
}
