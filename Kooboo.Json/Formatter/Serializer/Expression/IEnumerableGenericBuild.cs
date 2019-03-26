using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.IEnumerableGeneric)]
    internal class IEnumerableGenericBuild : ExpressionJsonFormatter
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            Type arrayItemType = type.GetElementType() ?? type.GetGenericArguments()[0];

            List<Expression> methodCall = new List<Expression>();

            if (!type.IsValueType)
            {
                ConditionalExpression ifNullAppendNull = Expression.IfThen(
                          Expression.Equal(instanceArg, Expression.Constant(null,type)),
                          Expression.Block(ExpressionMembers.Append("null"),
                                              Expression.Return(ExpressionMembers.ReturnLable)
                                               ));

                methodCall.Add(ifNullAppendNull);
            }
            methodCall.Add(ExpressionMembers.IsIgnoreSelfRefernce(instanceArg, ExpressionMembers.RefernceByEmptyType.Array));

            methodCall.Add(ExpressionMembers.Append("["));

            ParameterExpression isFirst = Expression.Variable(typeof(bool), "isFirst");

            Expression foreachs = ExpressionHelper.ForEach(instanceArg, Expression.Parameter(arrayItemType, "loopVar"),
           item => Expression.Block(new[] { isFirst },
               Expression.IfThenElse(
                   Expression.IsFalse(isFirst),
                   Expression.Assign(isFirst, Expression.Constant(true)),
                   ExpressionMembers.Append(",")
               ),
               ExpressionMembers.GetMethodCall(arrayItemType, item)
           ));

            methodCall.Add(foreachs);
            methodCall.Add(ExpressionMembers.Append("]"));
            methodCall.Add(ExpressionMembers.IsReferenceLoopHandlingIsNoneSerializeStacksArgPop);
            methodCall.Add(Expression.Label(ExpressionMembers.ReturnLable));
            return Expression.Block(methodCall);

        }
    }
}
