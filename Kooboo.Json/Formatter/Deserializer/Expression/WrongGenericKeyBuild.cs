using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.WrongGenericKey)]
    internal class WrongGenericKeyBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type type)
        {
            List<Expression> methodCall = new List<Expression>();
            LabelTarget returnValueLable = Expression.Label(type, "returnValue");
            /*
               if(_ReadNullOrObjLeft)
                  return null;/return default(ValueType)
            */
            Expression ifReadNullOrObjLeftReturnNull1 = Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadNullOrObjLeft),
               Expression.Return(returnValueLable, Expression.Constant(null, type)
               ));
            methodCall.Add(ifReadNullOrObjLeftReturnNull1);

            /*
              reader.ReadObjRight()
              */
            methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjRight));

            /*
             return new dictionary<,>;
             */
            methodCall.Add(Expression.Return(returnValueLable, Expression.New(type)));
            methodCall.Add(Expression.Label(returnValueLable, Expression.New(type)));
            return Expression.Block(methodCall);
        }
    }
}
