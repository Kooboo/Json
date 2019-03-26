using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.KeyValuePair)]
    internal class KeyValuePairBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type type)
        {

            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];
            List<Expression> methodCall = new List<Expression>();
            Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            LabelTarget returnValueLable = Expression.Label(keyValuePairType, "returnValue");

            ConstructorInfo ctor = keyValuePairType.GetConstructor(new[] { keyType, valueType });
            ParameterExpression key = Expression.Variable(keyType, "key");
            ParameterExpression value = Expression.Variable(valueType, "value");

            /*
                ReadObjLeft()
               */
            methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjLeft));
            /*
                if(reader.ReadBoolObjRight)
                        return new KeyValuePair<,>(default,default);
             */
            methodCall.Add(
                Expression.IfThen(
                     Expression.Call(ExpressionMembers.Reader, JsonReader._ReadBoolObjRight),
                        Expression.Return(returnValueLable, Expression.New(ctor, keyType.IsValueType ? (Expression)Expression.New(keyType) : Expression.Constant(null, keyType), valueType.IsValueType ? (Expression)Expression.New(valueType) : Expression.Constant(null, valueType)))));

            /*
              ReadKey()
            */
            methodCall.Add(ExpressionMembers.GenerateKeyValuePairByReadKey(keyType, key));

            /*
               reader.ReadColon()
             */
            methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadColon));

            /*
               value = ResolveProvider<ValueType>.InvokeGet(reader, handler);
            */
            methodCall.Add(Expression.Assign(value, ExpressionMembers.GetMethodCall(valueType)));

            /*
             reader.ReadObjRight();
             */
            methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjRight));

            /*
             return new KeyValuePair<,>(key,value);
             */
            methodCall.Add(Expression.Return(returnValueLable, Expression.New(ctor, key, value)));
            methodCall.Add(Expression.Label(returnValueLable, Expression.New(ctor, key, value)));

            return Expression.Block(new[] { key, value }, methodCall);
        }
    }
}
