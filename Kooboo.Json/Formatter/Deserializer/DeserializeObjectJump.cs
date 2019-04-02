using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading;

namespace Kooboo.Json.Deserialize
{
    internal class DeserializeObjectJump
    {
        private static SpinLock _spinLock = new SpinLock();

        private static readonly Dictionary<Type, Func<string, JsonDeserializeHandler, object>> JumpStringConvertDics =
            new Dictionary<Type, Func<string, JsonDeserializeHandler, object>>();

        private static readonly Dictionary<Type, Func<StreamReader, JsonDeserializeHandler, object>> JumpStreamConvertDics =
           new Dictionary<Type, Func<StreamReader, JsonDeserializeHandler, object>>();

        internal static object GetThreadSafetyJumpFunc(string json, Type t, JsonDeserializeHandler handler)
        {
            if (JumpStringConvertDics.TryGetValue(t, out var fc))
                return fc(json, handler);
            else
            {
                try
                {
                    bool i = false;
                    _spinLock.Enter(ref i);
                    if (!JumpStringConvertDics.TryGetValue(t, out fc))
                    {
                        fc = GenerateJumpStringConvertFunc(t);
                        JumpStringConvertDics.Add(t, fc);
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
        internal static object GetThreadSafetyJumpFunc(StreamReader stream, Type t, JsonDeserializeHandler handler)
        {
            if (JumpStreamConvertDics.TryGetValue(t, out var fc))
                return fc(stream, handler);
            else
            {
                try
                {
                    bool i = false;
                    _spinLock.Enter(ref i);
                    if (!JumpStreamConvertDics.TryGetValue(t, out fc))
                    {
                        fc = GenerateJumpStreamConvertFunc(t);
                        JumpStreamConvertDics.Add(t, fc);
                    }
                    return fc(stream, handler);
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
        internal static Func<string, JsonDeserializeHandler, object> GenerateJumpStringConvertFunc(Type t)
        {
            var genericType = typeof(ResolveProvider<>).MakeGenericType(t);
            var jsonEx = Expression.Parameter(typeof(string), "json");
            var handlerEx = Expression.Parameter(typeof(JsonDeserializeHandler), "handler");
            var body = Expression.Convert(Expression.Call(genericType.GetMethod("Convert",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static,null, new Type[] { typeof(string), typeof(JsonDeserializeHandler) },null), jsonEx, handlerEx), typeof(object));
            return Expression.Lambda<Func<string, JsonDeserializeHandler, object>>(body, jsonEx, handlerEx).Compile();
        }
        internal static Func<StreamReader, JsonDeserializeHandler, object> GenerateJumpStreamConvertFunc(Type t)
        {
            var genericType = typeof(ResolveProvider<>).MakeGenericType(t);
            var jsonEx = Expression.Parameter(typeof(StreamReader), "reader");
            var handlerEx = Expression.Parameter(typeof(JsonDeserializeHandler), "handler");
            var body = Expression.Convert(Expression.Call(genericType.GetMethod("Convert", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static ,null,new Type[] { typeof(StreamReader), typeof(JsonDeserializeHandler) },null), jsonEx, handlerEx), typeof(object));
            return Expression.Lambda<Func<StreamReader, JsonDeserializeHandler, object>>(body, jsonEx, handlerEx).Compile();
        }
    }
}
