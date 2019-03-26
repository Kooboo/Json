using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.KeyValuePair)]
    internal class KeyValuePairBuild : ExpressionJsonFormatter 
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];

            Expression[] methodCall = new Expression[7];
            methodCall[0] = ExpressionMembers.IsIgnoreSelfRefernce(instanceArg, ExpressionMembers.RefernceByEmptyType.Dictionary);
            methodCall[1] = ExpressionMembers.Append("{");

            MemberExpression key = Expression.MakeMemberAccess(instanceArg, typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType).GetProperty("Key"));
            MemberExpression value = Expression.MakeMemberAccess(instanceArg, typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType).GetProperty("Value"));

            methodCall[2] = ExpressionMembers.WriteKeyValuePairKey(keyType, key);
            methodCall[3] = ExpressionMembers.GetMethodCall(valueType, value);
            methodCall[4] = ExpressionMembers.Append("}");
            methodCall[5] = ExpressionMembers.IsReferenceLoopHandlingIsNoneSerializeStacksArgPop;
            methodCall[6] = Expression.Label(ExpressionMembers.ReturnLable);

            return Expression.Block(methodCall);
        }
    }
}
