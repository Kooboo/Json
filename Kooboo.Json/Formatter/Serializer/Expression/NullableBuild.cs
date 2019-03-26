using System;
using System.Linq.Expressions;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.Nullable)]
    internal class NullableBuild : ExpressionJsonFormatter 
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            Expression orginal = ExpressionMembers.GetMethodCall(type.GetGenericArguments()[0], Expression.MakeMemberAccess(instanceArg, type.GetProperty("Value")));

            return Expression.IfThenElse(
                         Expression.Equal(instanceArg, Expression.Constant(null,type)),
                         ExpressionMembers.Append("null")
                         ,
                         //para.value
                         orginal
                         );
        }
    }
}
