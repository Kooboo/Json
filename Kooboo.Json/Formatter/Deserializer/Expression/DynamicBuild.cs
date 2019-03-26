using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.Dynamic)]
    internal class DynamicBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type type)
        {
            List<Expression> methodCall = new List<Expression>();
            LabelTarget returnValueLable = Expression.Label(type, "returnValue");

            var c = type.GetProperties();
            ParameterExpression[] ctorArgs = null;
            if (c.Length > 0)
            {
                ctorArgs = new ParameterExpression[c.Length];
                for (int i = 0; i < c.Length; i++)
                {
                    ctorArgs[i] = Expression.Variable(c[i].PropertyType);
                    if (c[i].PropertyType.IsValueType)
                        methodCall.Add(Expression.Assign(ctorArgs[i], Expression.New(c[i].PropertyType)));
                    else
                        methodCall.Add(Expression.Assign(ctorArgs[i], Expression.Constant(null, c[i].PropertyType)));
                }
            }
            methodCall.Add(Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadNullOrObjLeft), Expression.Return(returnValueLable, Expression.Constant(null, type))));

            methodCall.Add(Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadBoolObjRight), Expression.Return(returnValueLable, c.Length == 0 ? Expression.New(type) : Expression.New(type.GetConstructors()[0], ctorArgs))));
            /*
            read.ReadLeftOrNull() 
                return null
            if(_ReadBoolObjRight)
                reture new (default)
            int a;
            string b;
            i=2
            while(i-->0)
            {
               var s= readString;
               ReadColon()
               switch(s)
                 case(s=='Name')
                     b=readValue

                 case(s=='Age')
                     a=readValue
                     arrig=true
               if(!arrig)
                  throw
               if(comma())
                 i++;
            }
            readRight();
            return new dynamicModel(a,b)
            */
            List<ParameterExpression> args = new List<ParameterExpression>() { };
            if (c.Length > 0)
            {
                ParameterExpression str = Expression.Variable(typeof(string), "str");
                LabelTarget whileBreak = Expression.Label();
                methodCall.Add(ExpressionMembers.MoveNextAssignOne);
                LoopExpression loopExpression = Expression.Loop(Expression.IfThenElse(ExpressionMembers.MoveNextDecrement,
                      ReturnFunc<Expression>(() =>
                      {
                          Expression[] expressions = new Expression[5];
                          expressions[0] = Expression.Assign(str, Expression.Call(ExpressionMembers.Reader, JsonReader._ReadString));
                          expressions[1] = Expression.Call(ExpressionMembers.Reader, JsonReader._ReadColon);
                          expressions[2] = Expression.Switch(str,
                               ReturnFunc(() =>
                               {
                                   SwitchCase[] switchCases = new SwitchCase[c.Length];
                                   for (int i = 0; i < c.Length; i++)
                                   {
                                       switchCases[i] = Expression.SwitchCase(
                                           Expression.Block(typeof(void),
                                            Expression.Assign(ctorArgs[i], ExpressionMembers.GetMethodCall(c[i].PropertyType)),
                                            ExpressionMembers.IsArriveAssignTrue
                                            )
                                           , Expression.Constant(c[i].Name, typeof(string)));
                                   }
                                   return switchCases;
                               })
                              );
                          expressions[3] = Expression.IfThen(Expression.Equal(ExpressionMembers.IsArrive, Expression.Constant(false, typeof(bool))), Expression.Throw(Expression.New(JsonDeserializationTypeResolutionException._JsonDeserializationTypeResolutionExceptionMsgCtor, Expression.Constant("An error occurred parsing the dynamically typed key. The key does not match", typeof(string)))));
                          expressions[4] = ExpressionMembers.IfReadBoolCommaIsTrueSoMoveNextIncrement;
                          return Expression.Block(expressions);
                      }),
                       Expression.Break(whileBreak)
                   ));
                methodCall.Add(loopExpression);
                methodCall.Add(Expression.Label(whileBreak));
                args.Add(str);
                args.AddRange(ctorArgs);
            }
            methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjRight));
            methodCall.Add(Expression.Return(returnValueLable, c.Length == 0 ? Expression.New(type) : Expression.New(type.GetConstructors()[0], ctorArgs)));
            methodCall.Add(Expression.Label(returnValueLable, Expression.Constant(null, type)));


            args.Add(ExpressionMembers.IsArrive);
            args.Add(ExpressionMembers.MoveNext);
            return Expression.Block(args, methodCall);

        }
    }
}
