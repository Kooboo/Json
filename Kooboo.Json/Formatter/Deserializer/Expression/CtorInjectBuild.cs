using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.CtorInject)]
    internal class CtorInjectBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type type, Type injectType, ConstructorInfo ctr)
        {
            LabelTarget returnTarget = Expression.Label(type, "returnLable");

            ParameterExpression inject = Expression.Variable(injectType, "inject");

            if (!type.IsValueType)
                return Expression.Block(new[] { inject },

                    Expression.Assign(inject, ExpressionMembers.GetMethodCall(injectType)),//inject = Read<T>();  T-->
                    Expression.IfThenElse(
                       Expression.Equal(inject, Expression.Constant(null, injectType)),
                       Expression.Return(returnTarget, Expression.Constant(null, type)),
                        Expression.Return(returnTarget, Expression.Convert(Expression.New(ctr, inject), type))),
                        Expression.Label(returnTarget, Expression.Constant(null, type))
                        );

            return Expression.Block(new[] { inject },
                      Expression.Assign(inject, ExpressionMembers.GetMethodCall(injectType)),
                       Expression.Return(returnTarget, Expression.Convert(Expression.New(ctr, inject), type)),
                       Expression.Label(returnTarget, Expression.Convert(Expression.New(ctr, inject), type)));
        }
     
       
    }
}
