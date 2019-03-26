using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    internal static class ExpressionMembers
    {
        // -charVariable
        internal static readonly ParameterExpression CharVariable = Expression.Variable(typeof(char), "charVariable");

        //- reader (FuncArgs)
        internal static readonly ParameterExpression Reader =
            Expression.Parameter(typeof(JsonReader).MakeByRefType(), "reader");

        //- reader.Remaining (FuncArgs.Member)
        internal static readonly MemberExpression Remaining =
            Expression.MakeMemberAccess(Reader, JsonReader._Remaining);

        //- reader.GetChar() (FuncArgs.Func)
        internal static readonly Expression GetChar = Expression.Call(Reader,JsonReader._GetChar);

        //- jsonDeserializeHandler (FuncArgs)
        internal static readonly ParameterExpression JsonDeserializeHandler =
            Expression.Parameter(typeof(JsonDeserializeHandler), "jsonDeserializeHandler");

        //- jsonDeserializeHandler.jsonDeserializeOption (FuncArgs.Member)
        internal static readonly MemberExpression JsonDeserializeOption =
            Expression.MakeMemberAccess(JsonDeserializeHandler, Json.JsonDeserializeHandler. _Option
              );

        //- jsonDeserializeHandler.jsonCharacterReadState
        internal static readonly MemberExpression JsonCharacterReadState =
             Expression.MakeMemberAccess(JsonDeserializeOption, Json.JsonDeserializeOption._JsonCharacterReadState);

        //- jsonDeserializeOption.GlobalValueFormat
        internal static readonly MemberExpression GlobalValueFormat =
            Expression.MakeMemberAccess(JsonDeserializeOption, Json.JsonDeserializeOption._GlobalValueFormat);

        //- jsonDeserializeOption.GlobalValueFormat!=null
        internal static readonly BinaryExpression GlobalValueFormatNotEqualNull = Expression.NotEqual(GlobalValueFormat,
            Expression.Constant(null, Json.JsonDeserializeOption._GlobalValueFormat.FieldType));

        //- jsonDeserializeOption.GlobalKeyFormat
        internal static readonly MemberExpression GlobalKeyFormat =
            Expression.MakeMemberAccess(JsonDeserializeOption, Json.JsonDeserializeOption._GlobalKeyFormat);

        //- jsonDeserializeOption.GlobalKeyFormat!=null
        internal static readonly BinaryExpression GlobalKeyFormatNotEqualNull = Expression.NotEqual(GlobalKeyFormat,
            Expression.Constant(null, Json.JsonDeserializeOption._GlobalKeyFormat.FieldType));

        //- afterFormatKey
        internal static readonly ParameterExpression AfterFormatKey =
            Expression.Variable(typeof(string), "afterFormatKey");

        //- moveNext (Func.LocalVariable) 
        internal static readonly ParameterExpression MoveNext = Expression.Variable(typeof(int), "moveNext");

        //- moveNext=1 (Func.LocalVariable.)
        internal static readonly BinaryExpression MoveNextAssignOne =
            Expression.Assign(MoveNext, Expression.Constant(1));

        //- moveNext-->0
        internal static readonly BinaryExpression MoveNextDecrement =
            Expression.GreaterThan(Expression.PostDecrementAssign(MoveNext), Expression.Constant(0));

        //- if(ReadBoolComma()==true) moveNext++;  
        internal static readonly ConditionalExpression IfReadBoolCommaIsTrueSoMoveNextIncrement =
            Expression.IfThen(
                Expression.Equal(Expression.Call(Reader, JsonReader._ReadBoolComma),
                    Expression.Constant(true, typeof(bool))),
                Expression.Assign(MoveNext, Expression.Increment(MoveNext)));

        //- isArrive (Func.LocalVariable) 
        internal static readonly ParameterExpression IsArrive = Expression.Variable(typeof(bool), "isArrive");

        //- isArrive=true 
        internal static readonly BinaryExpression IsArriveAssignTrue =
            Expression.Assign(IsArrive, Expression.Constant(true));

        //- isArrive=false
        internal static readonly BinaryExpression IsArriveAssignFalse =
            Expression.Assign(IsArrive, Expression.Constant(false));

        //- currentIdx
        internal static readonly ParameterExpression CurrentIdx = Expression.Variable(typeof(int), "currentIdx");

        //- currentIdx= Length - reader.Remaining
        internal static readonly BinaryExpression CurrentIdxAssignReming = Expression.Assign(CurrentIdx,
            Expression.Subtract(Expression.MakeMemberAccess(Reader, JsonReader._Length), Remaining));

        //- isValueFormat
        internal static readonly ParameterExpression IsValueFormat = Expression.Variable(typeof(bool), "isValueFormat");

        //- isValueFormat==true
        internal static readonly BinaryExpression IsValueFormatEqualTrue =
            Expression.Equal(IsValueFormat, Expression.Constant(true, typeof(bool)));

        //- valueLength
        internal static readonly ParameterExpression ValueLength = Expression.Variable(typeof(int), "valueLength");

        //- valueLength=reader.Skipobj()
        internal static readonly BinaryExpression ValueLengthAssignSkipObj = Expression.Assign(ValueLength,
            Expression.Call(Reader, JsonReader._SkipObj, JsonDeserializeHandler));

        //- reader.Substring(currentIdx,valueLength)
        internal static readonly Expression JsonSubstring =
            Expression.Call(Reader, JsonReader._SubString, CurrentIdx, ValueLength);

        //- object formatResult = ValueFormat.ReadValueFormat(  reader.Json.Substring(currentIdx,leng) ,out isValueFormat);
        internal static readonly ParameterExpression FormatResult = Expression.Variable(typeof(object), "formatResult");

        internal static Expression GetMethodCall(Type type)
        {
            var realType = typeof(ResolveProvider<>).MakeGenericType(type);
            return Expression.Call(realType.GetMethod("InvokeGet", BindingFlags.NonPublic | BindingFlags.Static),
                Reader, JsonDeserializeHandler);
        }

        internal static Expression GenerateKeyValuePairByReadKey(Type keyType, Expression keyPara)
        {
            List<Expression> lists = new List<Expression>();
            /*
             reader._ReadQuotes()
             */
            if (keyType.IsPrimitive)
                lists.Add(Expression.Call(Reader, JsonReader._ReadQuotes));

            /*
             key = ResolveProvider<KeyType>.InvokeGet(reader, handler);
            */
            lists.Add(Expression.Assign(keyPara, GetMethodCall(keyType)));

            /*
              if(getChar()!='"')
                    throw new Ex
             */
            if (keyType.IsPrimitive)
                lists.Add(Expression.IfThen(Expression.NotEqual(GetChar, Expression.Constant('"', typeof(char))),
                    Expression.Throw(Expression.New(JsonWrongCharacterException._JsonWrongCharacterExceptionCtor, Reader)
                    )));

            return Expression.Block(lists);
        }
    }
}
