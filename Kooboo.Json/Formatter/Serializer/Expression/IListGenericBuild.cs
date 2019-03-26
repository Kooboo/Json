using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.IListGeneric)]
    internal class IListGenericBuild : ExpressionJsonFormatter
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            Type arrayItemType = type.GetElementType() ?? type.GetGenericArguments()[0];

            List<Expression> methodCall = new List<Expression>();

            if (!type.IsValueType)
            {
                ConditionalExpression ifNullAppendNull = Expression.IfThen(
                          Expression.Equal(instanceArg, Expression.Constant(null, type)),
                          Expression.Block(ExpressionMembers.Append("null"),
                                              Expression.Return(ExpressionMembers.ReturnLable)
                                               ));

                methodCall.Add(ifNullAppendNull);
            }
            methodCall.Add(ExpressionMembers.IsIgnoreSelfRefernce(instanceArg, ExpressionMembers.RefernceByEmptyType.Array));

            methodCall.Add(ExpressionMembers.Append("["));

            ParameterExpression isFirst = Expression.Variable(typeof(bool), "isFirst");

            Type iListType = typeof(IList<>).MakeGenericType(arrayItemType);
            ParameterExpression iList = Expression.Variable(iListType);

            methodCall.Add(Expression.Assign(iList, Expression.Convert(instanceArg, iListType)));

            ParameterExpression forVariable = Expression.Variable(typeof(int));
            //Expression forExpression = ExpressionHelper.For(forVariable, Expression.LessThan(forVariable, Expression.MakeMemberAccess(instanceArg, type.IsArray ? type.GetProperty("Length") : type.GetProperty("Count"))), Expression.PostIncrementAssign(forVariable), Expression.Block(new[] { isFirst },
            //      Expression.IfThenElse(
            //          Expression.IsFalse(isFirst),
            //          Expression.Assign(isFirst, Expression.Constant(true)),
            //          ExpressionMembers.Append(",")
            //      ),
            //      ExpressionMembers.GetMethodCall(arrayItemType, type.IsArray ? Expression.ArrayAccess(instanceArg, forVariable) : Expression.MakeIndex(instanceArg, type.GetProperty("Item"), new[] { forVariable }))
            //  ));

            Expression forExpression = ExpressionHelper.For(forVariable, Expression.LessThan(forVariable, Expression.MakeMemberAccess(instanceArg, type.IsArray ? type.GetProperty("Length") : type.GetProperty("Count"))), Expression.PostIncrementAssign(forVariable), Expression.Block(new[] { isFirst },
                  Expression.IfThenElse(
                      Expression.IsFalse(isFirst),
                      Expression.Assign(isFirst, Expression.Constant(true)),
                      ExpressionMembers.Append(",")
                  ),
                  ExpressionMembers.GetMethodCall(arrayItemType, Expression.MakeIndex(iList, iListType.GetProperty("Item"), new[] { forVariable }))
              ));


            methodCall.Add(forExpression);
            methodCall.Add(ExpressionMembers.Append("]"));
            methodCall.Add(ExpressionMembers.IsReferenceLoopHandlingIsNoneSerializeStacksArgPop);
            methodCall.Add(Expression.Label(ExpressionMembers.ReturnLable));
            return Expression.Block(new[] { iList },methodCall);

        }
    }
}
