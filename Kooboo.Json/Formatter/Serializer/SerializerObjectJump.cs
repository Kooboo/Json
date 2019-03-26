using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Kooboo.Json.Serializer
{
    internal static class SerializerObjectJump
    {
        private static  SpinLock _spinLock = new SpinLock();

        private static readonly Dictionary<Type, Action<object, JsonSerializerHandler>> JumpActions = new Dictionary<Type, Action<object, JsonSerializerHandler>>();
       

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Action<object, JsonSerializerHandler> GetThreadSafetyJumpAction(Type t)
        {
            if (JumpActions.TryGetValue(t, out var ac))
                return ac;
            else
            {
                try
                {
                    bool i = false;
                    _spinLock.Enter(ref i);
                    if (!JumpActions.TryGetValue(t,out ac))
                    {
                        ac = GenerateAction(t);
                        JumpActions.Add(t, ac);
                    }
                    return ac;
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

        internal static Action<object, JsonSerializerHandler> GenerateAction( Type t)
        {
            ParameterExpression objArg = Expression.Parameter(typeof(object), "instance");
            Expression originalArg = Expression.Convert(objArg, t);
            ParameterExpression handlerArg = Expression.Parameter(typeof(JsonSerializerHandler), "handler");
            var body = Expression.Call(typeof(FormattingProvider<>).MakeGenericType(t).GetMethod("Convert",BindingFlags.NonPublic|BindingFlags.Static), originalArg, handlerArg);
            return Expression.Lambda<Action<object, JsonSerializerHandler>>(body, objArg, handlerArg).Compile();
        }
    }
}
