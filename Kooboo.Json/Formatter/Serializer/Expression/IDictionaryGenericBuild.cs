using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.IDictionaryGeneric)]
    internal class IDictionaryGenericBuild : ExpressionJsonFormatter 
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];

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
            methodCall.Add(ExpressionMembers.IsIgnoreSelfRefernce(instanceArg, ExpressionMembers.RefernceByEmptyType.Dictionary));

            methodCall.Add(ExpressionMembers.Append("{"));

            ParameterExpression isFirst = Expression.Variable(typeof(bool), "isFirst");

            Expression foreachs = ExpressionHelper.ForEach(instanceArg, Expression.Parameter(typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType), "loopVar"),
                    item =>
                    {
                        List<Expression> calls = new List<Expression>
                        {
                                Expression.IfThenElse(
                                    Expression.IsFalse(isFirst),
                                    Expression.Assign(isFirst, Expression.Constant(true)),
                                    ExpressionMembers.Append(",")
                                )
                        };

                        MemberExpression key = Expression.MakeMemberAccess(item, typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType).GetProperty("Key"));
                        MemberExpression value = Expression.MakeMemberAccess(item, typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType).GetProperty("Value"));

                        calls.Add(ExpressionMembers.WriteKeyValuePairKey(keyType, key));
                        calls.Add(ExpressionMembers.GetMethodCall(valueType, value));

                        return Expression.Block(new[] { isFirst }, calls);
                    }
                    );

            methodCall.Add(foreachs);


            methodCall.Add(ExpressionMembers.Append("}"));
            methodCall.Add(ExpressionMembers.IsReferenceLoopHandlingIsNoneSerializeStacksArgPop);
            methodCall.Add(Expression.Label(ExpressionMembers.ReturnLable));
            return Expression.Block(methodCall);
        }
    }
}
