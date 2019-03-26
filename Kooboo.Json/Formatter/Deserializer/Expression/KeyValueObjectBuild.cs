using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.KeyValueObject)]
    internal class KeyValueObjectBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type type)
        {
            List<Expression> methodCall = new List<Expression>();
            LabelTarget returnValueLable = Expression.Label(type, "returnValue");

            if (type.IsValueType)
            {
                /*
                  ReadObjLeft()
                 */
                methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjLeft));
            }
            else
            {
                /*
                 if(_ReadNullOrObjLeft)
                    return null;/return default(ValueType)
                 */
                Expression ifReadNullOrObjLeftReturnNull1 = Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadNullOrObjLeft),
                    Expression.Return(returnValueLable, Expression.Constant(null, type)
                    ));
                methodCall.Add(ifReadNullOrObjLeftReturnNull1);
            }

            /*
             Model m=new Model();
             */
            NewExpression newCtor = null;
            if (type.IsValueType)
            {
                List<Expression> ctorArgs = null;
                var ctor = type.GetValueTypeCtor(ref ctorArgs);
                if (ctor == null)
                    newCtor = Expression.New(type);
                else
                    newCtor = Expression.New(ctor, ctorArgs);
            }
            else
            {
                if (type.IsInterface)
                {
                    newCtor = Expression.New((Type)typeof(InterfaceImplementation<>).MakeGenericType(type).GetField("Proxy").GetValue(null));
                }
                else
                {
                    List<Expression> ctorArgs = null;
                    var ctor = type.GetClassCtor(ref ctorArgs);
                    newCtor = Expression.New(ctor, ctorArgs);
                }
            }
            ParameterExpression newModel = Expression.Variable(type, "newModel");
            methodCall.Add(Expression.Assign(newModel, newCtor));

            /*
              if(reader.ReadBoolObjRight)
                    return m;
             */
            methodCall.Add(Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadBoolObjRight), Expression.Return(returnValueLable, newModel)));

            /*
              int moveNext=2;
             */
            methodCall.Add(ExpressionMembers.MoveNextAssignOne); //Add moveNext localVirbale

            /*
              if(jsonDeserializeOption.IsFirstUpper)
                       while(moveNext-->0)
                       { }
              else if(jsonDeserializeOption.IsFirstLower)
                       while(moveNext-->0)
                       { }
              else
                       while(moveNext-->0)
                       { }
             */
            var charTries = type.GetCharTries();
            if (charTries.Childrens.Count > 0)
                methodCall.Add(GenerateWhile(newModel, type, charTries));

            /*
              reader.ReadObjRight()
             */
            methodCall.Add(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadObjRight));

            /*
             return newModel;
             */
            methodCall.Add(Expression.Return(returnValueLable, newModel));
            methodCall.Add(Expression.Label(returnValueLable, newModel));

            return Expression.Block(new[] { ExpressionMembers.AfterFormatKey, ExpressionMembers.CurrentIdx, ExpressionMembers.IsValueFormat, ExpressionMembers.ValueLength, ExpressionMembers.FormatResult, ExpressionMembers.CharVariable, ExpressionMembers.MoveNext, ExpressionMembers.IsArrive, newModel }, methodCall);
        }

        private static BlockExpression GenerateWhile(ParameterExpression newModel, Type type, CharTries charTries)
        {
            //(state):0==The initial state does not change，1==Capital letters，2==Initial lowercase
            /*
             char c;
             while(moveNext-->0)
             { 
             }
             */
            ParameterExpression firstChar = Expression.Variable(typeof(char), "firstChar");
            LabelTarget whileBreak = Expression.Label();
            LoopExpression loopExpression = Expression.Loop(Expression.IfThenElse(ExpressionMembers.MoveNextDecrement,
                     ReturnFunc<Expression>(() =>
                     {
                         Expression[] expressions = new Expression[4];
                         /*
                            isArrive=false;
                          */
                         expressions[0] = ExpressionMembers.IsArriveAssignFalse;

                         /*
                          * if(handler.option.GlobalKeyFormat!=null)
                                GenerateGlobalKeyFormat()
                            else
                                ReadKey()
                           */
                         expressions[1] = Expression.IfThenElse(
                            Expression.NotEqual(Expression.MakeMemberAccess(ExpressionMembers.JsonDeserializeOption, JsonDeserializeOption._GlobalKeyFormat), Expression.Constant(null, JsonDeserializeOption._GlobalKeyFormat.FieldType)),
                            GenerateGlobalKeyFormat(type, newModel),
                            GenerateKeyValueObjectReadKey(firstChar, charTries, newModel)
                            );

                         /*
                             if(!isArrive)
                                  throw 
                          */
                         expressions[2] = Expression.IfThen(Expression.Equal(ExpressionMembers.IsArrive, Expression.Constant(false, typeof(bool))), Expression.Throw(Expression.New(JsonDeserializationTypeResolutionException._JsonDeserializationTypeResolutionExceptionCtor, ExpressionMembers.Reader, type.IsInterface ? (Expression)Expression.Constant(type, typeof(Type)) : Expression.Call(newModel, type.GetMethod("GetType")))));

                         /*
                             if(reader.ReadComma()==true)
                                 moveNext++;
                         */
                         expressions[3] = ExpressionMembers.IfReadBoolCommaIsTrueSoMoveNextIncrement;

                         return Expression.Block(expressions);
                     })
                     , Expression.Break(whileBreak)));
            return Expression.Block(new[] { firstChar }, loopExpression, Expression.Label(whileBreak));
        }

        private static Expression GenerateGlobalKeyFormat(Type type, ParameterExpression newModel)
        {
            return Expression.Block(

                /*
                 var afterFormatKey = handler.option.GlobalKeyFormat.Invoke(reader.ReadString(),type)
                 */
                Expression.Assign(ExpressionMembers. AfterFormatKey,
                 Expression.Call(ExpressionMembers.GlobalKeyFormat, JsonDeserializeOption._GlobalKeyFormatInvoke,
                  Expression.Call(ExpressionMembers.Reader, JsonReader._ReadString),Expression.Constant(type,typeof(Type)))),
                /*
                 reader.ReadColon()
                 */
                Expression.Call(ExpressionMembers.Reader, JsonReader._ReadColon),
                /*
                 Switch(afterFormatKey)
                     case 'Name':
                         ReadValue()....
                 */
                Expression.Switch(
                    typeof(void),
                    ExpressionMembers.AfterFormatKey,
                    null, null,
                    ReturnFunc(() =>
                    {
                        var members = type.GetModelMembers();
                        SwitchCase[] switchCases = new SwitchCase[members.Count];
                        for (int i = 0; i < members.Count; i++)
                        {
                            var item = members[i];
                            switchCases[i] = Expression.SwitchCase(GenerateKeyValueObjectReadValue(item.Value, newModel), Expression.Constant(item.Key));

                        }
                        return switchCases;
                    })
                    ),
                /*
                    isArrive=true;
                 */
                ExpressionMembers.IsArriveAssignTrue
                 );
        }

        private static Expression GenerateKeyValueObjectReadValue(MemberExtension member, ParameterExpression newModel)
        {
            if (!member.IsProperty || (member.IsProperty && member.PropertyInfo.CanWrite))
            {
                var valueFormatAttribute = member.MemberInfo.GetCustomAttribute<ValueFormatAttribute>() ?? member.MemberInfo.DeclaringType.GetCustomAttribute<ValueFormatAttribute>();
                if (valueFormatAttribute != null)//part
                {
                    return (GenerateValueFormatCode(Expression.Constant(valueFormatAttribute, typeof(ValueFormatAttribute)), ValueFormatAttribute._ReadValueFormat, new[] { ExpressionMembers.JsonSubstring,Expression.Constant(member.Type,typeof(Type)), ExpressionMembers.JsonDeserializeHandler, ExpressionMembers.IsValueFormat }, newModel, member));
                }
                else //global
                {
                    /*
                     if( jsonDeserializeHandler.jsonDeserializeOption .globalValueFormat!=null)
                     {
                     }
                     else
                         model.N=ReadJson();
                     */
                    return (Expression.IfThenElse(ExpressionMembers.GlobalValueFormatNotEqualNull, GenerateValueFormatCode(ExpressionMembers.GlobalValueFormat, JsonDeserializeOption._GlobalValueFormatInvoke, new[] { ExpressionMembers.JsonSubstring, Expression.Constant(member.Type, typeof(Type)), ExpressionMembers.JsonDeserializeHandler, ExpressionMembers.IsValueFormat }, newModel, member), Expression.Assign(Expression.MakeMemberAccess(newModel, member.MemberInfo), ExpressionMembers.GetMethodCall(member.Type))));
                }
            }
            else
                /*   SkipObj(); */
                return (Expression.Call(ExpressionMembers.Reader, JsonReader._SkipObj, ExpressionMembers.JsonDeserializeHandler));
        }

        private static BlockExpression GenerateValueFormatCode(Expression formatDeclareInstance, MethodInfo callFormat, Expression[] paras, ParameterExpression newModel, MemberExtension member)
        {
            Expression[] expressions = new Expression[6];
            /*
             reader.BeforAnnotation();
             reader.RollBackChar()
             */
            expressions[0] = Expression.Call(ExpressionMembers.Reader, JsonReader._BeforAnnotation);
            expressions[1] = Expression.Call(ExpressionMembers.Reader, JsonReader._RollbackChar);
            /*
             currentIdx= Length - reader.Remaining
             */
            expressions[2] = ExpressionMembers.CurrentIdxAssignReming;
            /*
             valueLength=reader.Skipobj()
             */
            expressions[3] = (ExpressionMembers.ValueLengthAssignSkipObj);
            /*
              object formatResult = ValueFormat.ReadValueFormat(  reader.Substring(currentIdx,valueLength) ,out isValueFormat);
            */
            expressions[4] = (Expression.Assign(ExpressionMembers.FormatResult, Expression.Call(formatDeclareInstance, callFormat, paras)));
            /*
              if(isValueFormat==true)
                 model.N=(Convert)obj;
              else
                reader.Rollback(valueLength)
                m.N=ReadJson();//string
            */
            expressions[5] = (Expression.IfThenElse(ExpressionMembers.IsValueFormatEqualTrue, Expression.Assign(Expression.MakeMemberAccess(newModel, member.MemberInfo), Expression.Convert(ExpressionMembers.FormatResult, member.Type)), Expression.Block(Expression.Call(ExpressionMembers.Reader, JsonReader._Rollback, ExpressionMembers.ValueLength), Expression.Assign(Expression.MakeMemberAccess(newModel, member.MemberInfo), ExpressionMembers.GetMethodCall(member.Type)))));

            return Expression.Block(expressions);
        }

        private static Expression GenerateKeyValueObjectReadKey(ParameterExpression firstChar, CharTries charTries, ParameterExpression newModel)
        {

            return
                        Expression.IfThenElse(
                            Expression.NotEqual(ExpressionMembers.JsonCharacterReadState, Expression.Constant(JsonCharacterReadStateEnum.IgnoreCase, typeof(JsonCharacterReadStateEnum))),
                            GenerateKeyValueObjectReadKeyWithInitial(firstChar, charTries, newModel),
                            GenerateKeyValueObjectReadKeyWithIgnoreCase(charTries, newModel));
        }

        private static Expression GenerateKeyValueObjectReadKeyWithInitial(ParameterExpression firstChar, CharTries charTries, ParameterExpression newModel)
        {
            return Expression.Block(
              /*
              _ReadQuotes();
               */
              Expression.Call(ExpressionMembers.Reader, JsonReader._ReadQuotes),
                /*
                 switch (handler.Option.InitialReadState)
                    {
                        case InitialReadStateEnum.None:
                            firstChar = reader.GetChar();
                            break;
                        case InitialReadStateEnum.Upper:
                            firstChar = char.ToUpper(reader.GetChar());
                            break;
                        case InitialReadStateEnum.Lower:
                            firstChar = char.ToLower(reader.GetChar());
                            break;
                    }
                 */

                Expression.Switch(typeof(void), ExpressionMembers.JsonCharacterReadState,
                null, null,
                Expression.SwitchCase(Expression.Assign(firstChar, ExpressionMembers.GetChar), Expression.Constant(JsonCharacterReadStateEnum.None, typeof(JsonCharacterReadStateEnum))),
                 Expression.SwitchCase(Expression.Assign(firstChar, Expression.Call(typeof(char).GetMethod("ToUpper", new[] { typeof(char) }), ExpressionMembers.GetChar)), Expression.Constant(JsonCharacterReadStateEnum.InitialUpper, typeof(JsonCharacterReadStateEnum))),
                  Expression.SwitchCase(Expression.Assign(firstChar, Expression.Call(typeof(char).GetMethod("ToLower", new[] { typeof(char) }), ExpressionMembers.GetChar)), Expression.Constant(JsonCharacterReadStateEnum.InitialLower, typeof(JsonCharacterReadStateEnum)))
                  ),
               /*
                Switch(firstChar)
                    case 'N'
                          Switch(getChar())
                                case 'a'
                                      Switch(getChar())
                                            case 'm'
                                                  Switch(getChar())
                                                        case 'e'
                                                              Switch(getChar())
                                                                    case '"':
                                                                        ReadValue()...
                */
               GenerateSwitchCodeByChar(firstChar, charTries, newModel)
              );

        }

        private static Expression GenerateKeyValueObjectReadKeyWithIgnoreCase(CharTries charTries, ParameterExpression newModel)
        {
            return Expression.Block(
              /*
              _ReadQuotes();
               */
              Expression.Call(ExpressionMembers.Reader, JsonReader._ReadQuotes),

               /*
                Switch(getChar())
                    case 'N'
                          Switch(getChar())
                                case 'a'
                                      Switch(getChar())
                                            case 'm'
                                                  Switch(getChar())
                                                        case 'e'
                                                              Switch(getChar())
                                                                    case '"':
                                                                        ReadValue()...
                */
               GenerateSwitchCodeByChar(null, charTries, newModel, true)
              );

        }

        private static SwitchExpression GenerateSwitchCodeByChar(ParameterExpression firstChar, CharTries charTries, ParameterExpression newModel, bool isToLower = false)
        {
            List<SwitchCase> switchCases = new List<SwitchCase>();

            if (charTries.IsValue)
            {
                //case '"'
                var caseQuotes = Expression.SwitchCase(

                 Expression.Block(
                       typeof(void),
                       /*
                          ReadColon();
                        */
                       Expression.Call(ExpressionMembers.Reader, JsonReader._ReadColon),
                       GenerateKeyValueObjectReadValue(charTries.Member, newModel),
                     /*
                       isArrive=true;
                      */
                     ExpressionMembers.IsArriveAssignTrue
                       )
                 , Expression.Constant('"', typeof(char)));

                switchCases.Add(caseQuotes);
            }

            if (charTries.Childrens.Count > 0)
            {
                foreach (var item in charTries.Childrens)
                {
                    char c = item.Val;
                    SwitchCase caseOrdinary = Expression.SwitchCase( //When there are two duplicate case items, the expression takes the first one automatically
                                                  GenerateSwitchCodeByChar(null, item, newModel,isToLower),
                                                 Expression.Constant(isToLower ? char.ToLower(c) : c, typeof(char)));

                    switchCases.Add(caseOrdinary);
                }
            }

            return Expression.Switch(isToLower ? Expression.Call(typeof(char).GetMethod("ToLower", new[] { typeof(char) }), ExpressionMembers.GetChar) : firstChar ?? ExpressionMembers.GetChar, switchCases.ToArray());
        }

    }

}
