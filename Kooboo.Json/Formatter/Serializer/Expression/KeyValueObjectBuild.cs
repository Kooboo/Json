using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kooboo.Json.Serializer
{
    [ExpressionBuildType(SerializerBuildTypeEnum.KeyValueObject)]
    internal class KeyValueObjectBuild : ExpressionJsonFormatter 
    {
        internal static Expression Build(Type type, ParameterExpression instanceArg)
        {
            List<Expression> methodCall = new List<Expression>();

            if (!type.IsValueType)
            {
                ConditionalExpression ifParaIsNullAppendNull = Expression.IfThen(Expression.Equal(instanceArg, Expression.Constant(null,type)),
                    Expression.Block(
                        ExpressionMembers.Append("null"),
                       Expression.Return(ExpressionMembers.ReturnLable)
                    ));
                methodCall.Add(ifParaIsNullAppendNull);//add

                Expression isIgnoreSelfRefernceModule = ExpressionMembers.IsIgnoreSelfRefernce(instanceArg, ExpressionMembers.RefernceByEmptyType.CustomType);

                methodCall.Add(isIgnoreSelfRefernceModule);//add
            }

            methodCall.Add(ExpressionMembers.WriteLeft);

            var elements = type.GetModelMembers(ReadMemberStateEnum.CanRead);
            for (int i = 0; i < elements.Count; i++)
            {
                var item = elements[i];

                //valueformat
                ValueFormatAttribute valueformat = item.Value.MemberInfo.GetCustomAttribute<ValueFormatAttribute>() ??
                   item.Value.MemberInfo.DeclaringType.GetCustomAttribute<ValueFormatAttribute>();

                //if (!(m.Name == null && handler.option.IsIgnoreNull))//0
                MemberExpression mName = Expression.MakeMemberAccess(instanceArg, item.Value.MemberInfo);

                //  if (option1.IsFirstLower)
                string lowerName = item.Key.Substring(0, 1).ToLower() + item.Key.Substring(1);
                string upperName = item.Key.Substring(0, 1).ToUpper() + item.Key.Substring(1);

                /*
                 ignoreKeys == null || (ignoreKeys.count() == 0 || ignoreKeys.Contains(name)==false
                 */
                BinaryExpression ignoreKeysJudge = Expression.OrElse(
                    ExpressionMembers.IgnoreKeysIsNull,
                    Expression.OrElse(
                        ExpressionMembers.IgnoreKeysCountIsZero,
                        Expression.Equal(
                            Expression.Call(ExpressionMembers.IgnoreKeys, typeof(List<string>).GetMethod("Contains"), Expression.Constant(item.Key))
                            , Expression.Constant(false))
                            ));

                /*
                  if(option.keyformat!=null)
                         $handler.Writer.Append("\"");
						 $handler.Writer.Append(option.keyformat.invoke(string));
						 append("\":")
				   else
					  {
                        .If (($handler.Option).IsFirstLower == True) {
                            .Call ($handler.Writer).Append(""ns":")
                        } .Else {
                            .If (($handler.Option).IsFirstUpper == True) {
                                .Call ($handler.Writer).Append(""Ns":")
                            } .Else {
                                .Call ($handler.Writer).Append(""ns":")
                            }
                        }
					  }
                    };
                 
                 */
                Expression writeKey = Expression.IfThenElse(Expression.NotEqual(ExpressionMembers.GlobalKeyFormat, Expression.Constant(null, JsonSerializerOption._GlobalKeyFormat.FieldType)),
                    Expression.Block(
                        ExpressionMembers.Append("\""),
                        Expression.Call(ExpressionMembers.SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Call(ExpressionMembers.GlobalKeyFormat, JsonSerializerOption._GlobalKeyFormatInvoke, Expression.Constant(item.Key), Expression.Constant(type, typeof(Type)), ExpressionMembers.HandlerArg)
                        ),
                       ExpressionMembers.Append("\":")
                        ),
                    Expression.Switch(typeof(void), ExpressionMembers.InitialReadState, null, null,
                        Expression.SwitchCase(ExpressionMembers.AppendKey(item.Key), Expression.Constant(JsonCharacterReadStateEnum.None, typeof(JsonCharacterReadStateEnum))),
                        Expression.SwitchCase(ExpressionMembers.AppendKey(upperName), Expression.Constant(JsonCharacterReadStateEnum.InitialUpper, typeof(JsonCharacterReadStateEnum))),
                        Expression.SwitchCase(ExpressionMembers.AppendKey(lowerName), Expression.Constant(JsonCharacterReadStateEnum.InitialLower, typeof(JsonCharacterReadStateEnum)))
                    )
                  );


                /*
                   string afterValueFormat= valueformat.WriteValueFormat( model.Name,hander out isValueFormat)
                   if(isValueFormat)
                         sb.append(afterValueFormat)
                   else 
                         WriteValue(model.Name,handler)
                 */
                Expression writeValue;
                if (valueformat != null)
                {
                    writeValue = Expression.Block(
                           Expression.NotEqual(ExpressionMembers.GlobalValueFormat, Expression.Constant(null, JsonSerializerOption._GlobalValueFormat.FieldType)),
                            Expression.Assign(ExpressionMembers.AfterValueFormat, Expression.Call(Expression.Constant(valueformat, typeof(ValueFormatAttribute)), ValueFormatAttribute._WriteValueFormat, Expression.Convert(mName, typeof(object)),Expression.Constant(item.Value.Type,typeof(Type)), ExpressionMembers.HandlerArg, ExpressionMembers.IsValueFormat)),
                            Expression.IfThenElse(Expression.IsTrue(ExpressionMembers.IsValueFormat), ExpressionMembers.Append(ExpressionMembers.AfterValueFormat), ExpressionMembers.GetMethodCall(item.Value.Type, mName))
                            );
                }
                else
                {
                    /*
                     if( option.GlobalValueFormat!=null){
                            string afterValueFormat= option.GlobalValueFormat.Invoke( model.Name,hander out isValueFormat)
                            if(isValueFormat)
                                  sb.append(afterValueFormat)
                            else 
                                  WriteValue(model.Name,handler)
                      }
                      else
                      {
                             WriteValue(model.Name,handler)
                      }
                    */
                    writeValue = Expression.IfThenElse(
                        Expression.NotEqual(ExpressionMembers.GlobalValueFormat, Expression.Constant(null, JsonSerializerOption._GlobalValueFormat.FieldType)),
                        Expression.Block(
                            Expression.Assign(ExpressionMembers.AfterValueFormat, Expression.Call(ExpressionMembers.GlobalValueFormat, JsonSerializerOption._GlobalValueFormatInvoke, Expression.Convert(mName, typeof(object)),Expression.Constant(item.Value.Type,typeof(Type)), ExpressionMembers.HandlerArg, ExpressionMembers.IsValueFormat)),
                            Expression.IfThenElse(Expression.IsTrue(ExpressionMembers.IsValueFormat), ExpressionMembers.Append(ExpressionMembers.AfterValueFormat), ExpressionMembers.GetMethodCall(item.Value.Type, mName))
                            ),
                        ExpressionMembers.GetMethodCall(item.Value.Type, mName)
                        );
                }

                Expression trunk = Expression.Block(writeKey, writeValue);

                // json.Append(",");
                if (i != elements.Count - 1)
                    trunk = Expression.Block(trunk, ExpressionMembers.WriteComma);

                if (!item.Value.Type.IsValueType || (item.Value.Type.IsGenericType && item.Value.Type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    BinaryExpression mNameIsNull = Expression.Equal(mName, Expression.Constant(null));
                    //if(!null&&isgorenull)
                    trunk = Expression.IfThen(Expression.IsFalse(Expression.And(mNameIsNull, ExpressionMembers.IgnoreValueNullIsTrue)), trunk);
                }
                // 
                trunk = Expression.IfThen(ignoreKeysJudge, trunk);//忽略key
           
                methodCall.Add(trunk);

            }

            methodCall.Add(ExpressionMembers.IgnoreValueNullIsTrueOrIgnoreKeysNotEqureNullRemoveLastComma);
            methodCall.Add(ExpressionMembers.IsReferenceLoopHandlingIsRemoveDeleteComma);
            methodCall.Add(ExpressionMembers.WriteRight);
            methodCall.Add(ExpressionMembers.IsReferenceLoopHandlingIsNoneSerializeStacksArgPop);
            methodCall.Add(Expression.Label(ExpressionMembers.ReturnLable));

            return Expression.Block(new[] { ExpressionMembers.AfterValueFormat, ExpressionMembers.IsValueFormat }, methodCall);
        }
    }
}
