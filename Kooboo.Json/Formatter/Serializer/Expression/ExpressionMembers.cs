using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kooboo.Json.Serializer
{
    internal class ExpressionMembers
    {
        //- handler;
        internal static ParameterExpression HandlerArg = Expression.Parameter(typeof(JsonSerializerHandler), "handler");
        //- return(label);
        internal static readonly LabelTarget ReturnLable = Expression.Label(typeof(void));

        //- handler.sbArg;
        internal static readonly MemberExpression SbArg = Expression.MakeMemberAccess(HandlerArg, typeof(JsonSerializerHandler).GetField("Writer", BindingFlags.NonPublic | BindingFlags.Instance));
        //- handler.option;
        internal static readonly MemberExpression JOptionArg = Expression.MakeMemberAccess(HandlerArg, JsonSerializerHandler._Option);
        //- handler.serializeStacks
        internal static readonly MemberExpression SerializeStacksArg = Expression.MakeMemberAccess(HandlerArg, JsonSerializerHandler._SerializeStacks);

        //- handler.option.GlobalKeyFormat(delegate)
        internal static readonly MemberExpression GlobalKeyFormat = Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._GlobalKeyFormat);
        //- handler.option.GlobalValueFormat(delegate)
        internal static readonly MemberExpression GlobalValueFormat = Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._GlobalValueFormat);

        //- afterValueFormat(string)
        internal static readonly ParameterExpression AfterValueFormat = Expression.Variable(typeof(string), "afterValueFormat");
        //- isValueFormat(bool)
        internal static readonly ParameterExpression IsValueFormat = Expression.Variable(typeof(bool), "isValueFormat");

        //- handler.option.ignoreKeys(List<string>)
        internal static readonly MemberExpression IgnoreKeys = Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._IgnoreKeys);
        //- handler.option.ignoreKeys == null
        internal static readonly BinaryExpression IgnoreKeysIsNull = Expression.Equal(IgnoreKeys, Expression.Constant(null));
        //- handler.option.ignoreKeys != null
        internal static readonly BinaryExpression IgnoreKeysIsNotNull = Expression.NotEqual(IgnoreKeys, Expression.Constant(null));
        //- handler.option.ignoreKeys.count() == 0
        internal static readonly BinaryExpression IgnoreKeysCountIsZero = Expression.Equal(Expression.MakeMemberAccess(IgnoreKeys, typeof(List<string>).GetProperty("Count")), Expression.Constant(0));

        //- handler.option.jsonCharacterRead(Enum)
        internal static readonly MemberExpression JsonCharacterRead = Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._JsonCharacterRead);

        //- handler.option.isEnumNum(bool)
        internal static readonly MemberExpression IsEnumNum = Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._IsEnumNum);

        //- handler.option.isIgnoreValueNull(bool)
        internal static readonly UnaryExpression IgnoreValueNullIsTrue = Expression.IsTrue(Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._IsIgnoreValueNull));

        //- handler.option.referenceLoopHandling(Enum)
        internal static readonly MemberExpression ReferenceLoopHandling = Expression.MakeMemberAccess(JOptionArg, JsonSerializerOption._ReferenceLoopHandling);
        //- handler.option.referenceLoopHandling != Enum.None
        internal static readonly BinaryExpression IsReferenceLoopHandlingNotEqualsNone = Expression.NotEqual(ReferenceLoopHandling, Expression.Constant(JsonReferenceHandlingEnum.None));
        //- handler.option.referenceLoopHandling == Enum.Remove
        internal static readonly BinaryExpression IsReferenceLoopHandlingEqualsRemove = Expression.Equal(ReferenceLoopHandling, Expression.Constant(JsonReferenceHandlingEnum.Remove));
        //- handler.option.referenceLoopHandling == Enum.Empty
        internal static readonly BinaryExpression IsReferenceLoopHandlingEqualsEmpty = Expression.Equal(ReferenceLoopHandling, Expression.Constant(JsonReferenceHandlingEnum.Empty));
        //- handler.option.referenceLoopHandling == Enum.Null
        internal static readonly BinaryExpression IsReferenceLoopHandlingEqualsNull = Expression.Equal(ReferenceLoopHandling, Expression.Constant(JsonReferenceHandlingEnum.Null));

        //- handler.commaIndexLists
        internal static readonly MemberExpression ListCommaIndexs = Expression.MakeMemberAccess(HandlerArg, JsonSerializerHandler._CommaIndexLists);

        //if ( ignoreValueNull == true || ignoreKeys!=null ) ==> RemoveLastComma()
        internal static readonly ConditionalExpression IgnoreValueNullIsTrueOrIgnoreKeysNotEqureNullRemoveLastComma = Expression.IfThen(
               Expression.OrElse(IgnoreValueNullIsTrue, IgnoreKeysIsNotNull),
                RemoveLastComma()
            );

        //if ( referenceLoopHandling == Enum.Remove ) ==> DeleteComma()
        internal static readonly ConditionalExpression IsReferenceLoopHandlingIsRemoveDeleteComma = Expression.IfThen(
            IsReferenceLoopHandlingEqualsRemove,
            DeleteComma()
            );

        //if ( referenceLoopHandling == Enum.None ) ==> serializeStacks.Pop()
        internal static readonly ConditionalExpression IsReferenceLoopHandlingIsNoneSerializeStacksArgPop = Expression.IfThen(
            IsReferenceLoopHandlingNotEqualsNone,
            Expression.Call(SerializeStacksArg, typeof(Stack<object>).GetMethod("Pop"))
          );

        //call
        internal static readonly MethodCallExpression WriteLeft = Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Constant("{"));
        internal static readonly MethodCallExpression WriteRight = Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Constant("}"));
        internal static readonly MethodCallExpression WriteComma = Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Constant(","));

        internal enum RefernceByEmptyType
        {
            Array,
            Dictionary,
            CustomType
        }
        internal static Expression WriteKeyValuePairKey(Type keyType, Expression key)
        {
            List<Expression> calls = new List<Expression>();
            if (keyType == typeof(string) || keyType == typeof(Guid) || keyType == typeof(DateTime))
            {
                calls.Add(GetMethodCall(keyType, key));
                calls.Add(Append(":"));
            }
            else if (keyType.IsPrimitive)
            {
                calls.Add(Append("\""));
                calls.Add(Expression.Call(SerializerBootTable.Table.DefaultSameTypes[keyType], key, HandlerArg));
                calls.Add(Append("\":"));
            }
            else if (keyType.IsEnum)//The default is tostring
            {
                calls.Add(Append("\""));
                calls.Add(Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Call(key, typeof(Enum).GetMethod("ToString", new Type[0]))));
                calls.Add(Append("\":"));
            }

            return Expression.Block(calls);
        }
        internal static Expression IsIgnoreSelfRefernce(ParameterExpression instanceArg, RefernceByEmptyType emptyType)
        {
            MethodCallExpression contains = Expression.Call(SerializeStacksArg, typeof(Stack<object>).GetMethod("Contains", new[] { typeof(object) }), Expression.Convert(instanceArg, typeof(object)));

            MethodCallExpression call;
            if (emptyType == RefernceByEmptyType.Array)
                call = RemoveArrayItem();
            else if (emptyType == RefernceByEmptyType.Dictionary)
                call = RemoveDictionaryKey();
            else
                call = RemoveKeyAndAddIndex();

            BlockExpression containsBody = Expression.Block(
            Expression.IfThenElse(
                IsReferenceLoopHandlingEqualsNull, Append("null"),
                Expression.IfThenElse(
                    IsReferenceLoopHandlingEqualsEmpty, Append(emptyType == RefernceByEmptyType.Array ? "[]" : "{}"),
                      call)
                    ),
                    Expression.Return(ReturnLable)
            );

            ConditionalExpression ifContainsBlock = Expression.IfThen(contains, containsBody);

            // handler.serializeStacks.Push(value);
            MethodCallExpression push = Expression.Call(SerializeStacksArg, typeof(Stack<object>).GetMethod("Push", new[] { typeof(object) }), Expression.Convert(instanceArg, typeof(object)));

            ConditionalExpression isIgnoreSelfRefernce = Expression.IfThen(IsReferenceLoopHandlingNotEqualsNone,
                Expression.Block(
                 ifContainsBlock, push
                    ));

            return isIgnoreSelfRefernce;
        }
        internal static MethodCallExpression GetMethodCall(Type type, Expression member)
        {
            return Expression.Call(typeof(FormattingProvider<>).MakeGenericType(type).GetMethod("Convert", BindingFlags.NonPublic | BindingFlags.Static), member, HandlerArg);
        }

        internal static MethodCallExpression AppendKey(string str)
        {
            return Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Constant("\"" + str + "\":"));
        }
        internal static MethodCallExpression Append(string str)
        {
            return Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), Expression.Constant(str));
        }
        internal static MethodCallExpression Append(Expression strExpression)
        {
            return Expression.Call(SbArg, typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) }), strExpression);
        }
        internal static MethodCallExpression RemoveDictionaryKey()
        {
            return Expression.Call(RemoveWriterHelper._RemoveDictionaryKey, SbArg);
        }
        internal static MethodCallExpression RemoveArrayItem()
        {
            return Expression.Call(RemoveWriterHelper._RemoveArrayItem, SbArg);
        }
        internal static MethodCallExpression RemoveKeyAndAddIndex()
        {
            return Expression.Call(RemoveWriterHelper._RemoveKeyAndAddIndex, SbArg, ListCommaIndexs);
        }
        internal static MethodCallExpression RemoveLastComma()
        {
            return Expression.Call(RemoveWriterHelper._RemoveLastComma, SbArg);
        }
        internal static MethodCallExpression DeleteComma()
        {
            return Expression.Call(RemoveWriterHelper._DeleteComma, SbArg, ListCommaIndexs);
        }
    }
}
