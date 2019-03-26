using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.Lazy)]
    internal class LazyBuild : ExpressionJsonResolve
    {
        #region pregenerated metas
        static MethodInfo _TransitionFunc = typeof(LazyBuild).GetMethod(nameof(TransitionFunc), BindingFlags.Static | BindingFlags.NonPublic);
        #endregion

        internal static BlockExpression Build(Type type)
        {
            LabelTarget returnTarget = Expression.Label(type, "returnLable");
            Type elementType = type.GetGenericArguments()[0];
            ParameterExpression element = Expression.Variable(elementType, "element");
            var ctr = type.GetAppointTypeCtor(typeof(Func<>).MakeGenericType(elementType));
            return Expression.Block(new[] { element },
                Expression.IfThenElse(
                    Expression.Call(ExpressionMembers.Reader, JsonReader._ReadBoolNull),
                    Expression.Return(returnTarget, Expression.Constant(null, type)),
                    Expression.Block(
                          Expression.Assign(element, ExpressionMembers.GetMethodCall(elementType)),   //inject = Read<T>();  T-->)
                      Expression.Return(returnTarget, Expression.New(ctr, Expression.Call(_TransitionFunc.MakeGenericMethod(elementType), element)))
                )),
                  Expression.Label(returnTarget, Expression.Constant(null, type))
                );
        }

        private static Func<T> TransitionFunc<T>(T t)
        {
            return () => t;
        }
    }
}
