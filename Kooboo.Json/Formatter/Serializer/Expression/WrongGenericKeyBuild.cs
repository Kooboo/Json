using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.WrongGenericKey)]
    internal class WrongGenericKeyBuild : ExpressionJsonFormatter
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            List<Expression> expressions = new List<Expression>();
            if (!type.IsValueType)
                expressions.Add(Expression.IfThen(
                          Expression.Equal(instanceArg, Expression.Constant(null,type)),
                          Expression.Block(ExpressionMembers.Append("null"),
                                              Expression.Return(ExpressionMembers.ReturnLable)
                                               )));
            expressions.Add(ExpressionMembers.Append("{}"));
            expressions.Add(Expression.Label(ExpressionMembers.ReturnLable));
            return Expression.Block(expressions);
        }
    }
}
