using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.Dictionary)]
    internal class DictionaryBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type defindType, Type instanceType, Type convertType,Type keyType,Type valueType)
        {
            /*
             * defind:ExpandoObject    new:ExpandoObject   convert:IDictionary<string, object>
               ExpandoObject ss = new ExpandoObject();
               IDictionary<string, object> dic = (IDictionary<string, object>)ss;

               hashtable
             */


            Expression[] methodDictionaryCall = new Expression[9];

            LabelTarget returnTarget = Expression.Label(defindType, "returnLable");
            Expression ifReadNullOrObjLeftReturnNull;

            if (!defindType.IsValueType)
                /*
                if (reader.ReadNullOrObjLeft())
                     return null;    
                */
                ifReadNullOrObjLeftReturnNull = Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadNullOrObjLeft), Expression.Return(returnTarget, Expression.Constant(null, defindType)));
            else
            {
                ifReadNullOrObjLeftReturnNull= Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjLeft);
            }
            methodDictionaryCall[0] = ifReadNullOrObjLeftReturnNull;

            /*
             Dictionary<> dictionary =new Dictionary<>();
             */
            NewExpression dictionaryCtor = Expression.New(instanceType);
            ParameterExpression dictionary = Expression.Variable(defindType, "dictionary");
            methodDictionaryCall[1] = Expression.Assign(dictionary, dictionaryCtor);

            /*
             if(reader.ReadBoolObjRight)
                   return dictionary;
            */
            methodDictionaryCall[2] = (Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadBoolObjRight), Expression.Return(returnTarget, dictionary)));

            /*
             IDictionary iDic = dictionary;  
             IDictionary<,> iDic = dictionary;
            */
            MethodInfo iDicAdd = convertType.GetMethod("Add");
            ParameterExpression iDic = Expression.Variable(convertType, "iDic");
            methodDictionaryCall[3] = Expression.Assign(iDic, Expression.Convert(dictionary, convertType));

            /*
              int moveNext=2;
             */
            methodDictionaryCall[4] = ExpressionMembers.MoveNextAssignOne;

            ParameterExpression key = Expression.Variable(keyType, "key");
            ParameterExpression value = Expression.Variable(valueType, "value");

            /*
             while(moveNext-->0)
                {}
             */
            LabelTarget loopBreak = Expression.Label("loopBreak");
            LoopExpression loopExpression = Expression.Loop(Expression.IfThenElse(ExpressionMembers.MoveNextDecrement,
                ReturnFunc<Expression>(() =>
                {
                    Expression[] methodCall = new Expression[5];

                    /*
                        ReadKey()
                     */
                    methodCall[0] = ExpressionMembers.GenerateKeyValuePairByReadKey(keyType, key);
                    /*
                       reader.ReadColon()
                     */
                    methodCall[1] = (Expression.Call(ExpressionMembers.Reader, JsonReader._ReadColon));
                    /*
                       value = ResolveProvider<ValueType>.InvokeGet(reader, handler);
                    */
                    methodCall[2] = (Expression.Assign(value, ExpressionMembers.GetMethodCall(valueType)));
                    /*
                       iDic.Add(key,value);
                     */
                    methodCall[3] = (Expression.Call(iDic, iDicAdd, key, value));
                    /*
                       if(reader.ReadComma()==true)
                             moveNext++;
                     */
                    methodCall[4] = (ExpressionMembers.IfReadBoolCommaIsTrueSoMoveNextIncrement);

                    return Expression.Block(methodCall); ;
                })
                , Expression.Break(loopBreak)));
            methodDictionaryCall[5] = Expression.Block(loopExpression, Expression.Label(loopBreak));

            /*
             reader.ReadObjRight();
             */
            methodDictionaryCall[6] = Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjRight);

            /*
             return dictionary
             */
            GotoExpression returnExpression = Expression.Return(returnTarget, dictionary);
            LabelExpression returnLabel = Expression.Label(returnTarget, dictionary);
            methodDictionaryCall[7] = returnExpression;
            methodDictionaryCall[8] = returnLabel;

            return Expression.Block(new[] { iDic, dictionary, key, value, ExpressionMembers.MoveNext }, methodDictionaryCall);
        }
    }
}
