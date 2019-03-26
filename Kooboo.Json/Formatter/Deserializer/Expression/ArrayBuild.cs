using System;
using System.Linq.Expressions;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.Array)]
    internal class ArrayBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type type)
        {
            Type arrayItemType = type.GetElementType();
            Expression[] methodListCall = new Expression[11];
            LabelTarget returnTarget = Expression.Label(type, "returnLable");
            /*
            if (reader.ReadNullOrArrayLeft())
                 return null;    
            */
            Expression ifReadNullOrArrayLeftReturnNull = Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadNullOrArrayLeft), Expression.Return(returnTarget, Expression.Constant(null, type)));
            methodListCall[0] = ifReadNullOrArrayLeftReturnNull;

            /*
            int arrayLength=reader._GetArrayLength;
            T[] array=new [arrayLength];
            */
            ParameterExpression arrayLength = Expression.Variable(typeof(int), "arrayLength");
            methodListCall[1] = Expression.Assign(arrayLength, Expression.Call(ExpressionMembers.Reader, JsonReader._GetArrayLength, ExpressionMembers.JsonDeserializeHandler));
            NewExpression aryCtor = Expression.New(type.GetConstructors()[0], arrayLength);
            ParameterExpression array = Expression.Variable(type, "array");
            methodListCall[2] = Expression.Assign(array, aryCtor);

            /*
             if(arrayLength==0)
                goto ReadArrayRightLabel;
             */
            LabelTarget readArrayRightLabel = Expression.Label(typeof(void), "ReadArrayRightLabel");
            methodListCall[3] = Expression.IfThen(Expression.Equal(arrayLength, Expression.Constant(0)), Expression.Goto(readArrayRightLabel));

            /*
            int moveNext=1;
           */
            methodListCall[4] = ExpressionMembers.MoveNextAssignOne;

            /*
             int idx=0;
             */
            ParameterExpression idx = Expression.Variable(typeof(int), "idx");
            methodListCall[5] = Expression.Assign(idx, Expression.Constant(0));

            /*
            while(moveNext-->0)
               {}
            */
            LabelTarget loopBreak = Expression.Label("loopBreak");
            LoopExpression loopExpression = Expression.Loop(Expression.IfThenElse(ExpressionMembers.MoveNextDecrement,
                ReturnFunc<Expression>(() =>
                {
                    Expression[] methodCall = new Expression[3];
                    /*
                       ary[idx]=item;
                     */
                    methodCall[0] = Expression.Assign(Expression.ArrayAccess(array, idx), ExpressionMembers.GetMethodCall(arrayItemType));

                    /*
                       if(reader.ReadBoolComma()==true)
                             moveNext++;
                     */
                    methodCall[1] = ExpressionMembers.IfReadBoolCommaIsTrueSoMoveNextIncrement;

                    /*
                     idx++;
                     */
                    methodCall[2] = Expression.PostIncrementAssign(idx);
                    return Expression.Block(methodCall); ;
                })
                , Expression.Break(loopBreak)));
            methodListCall[6] = Expression.Block(loopExpression, Expression.Label(loopBreak));

            /*
        ReadArrayRightLabel:
             reader.ReadArrayRight()
             */
            methodListCall[7] = Expression.Label(readArrayRightLabel);
            methodListCall[8] = Expression.Call(ExpressionMembers.Reader, JsonReader._ReadArrayRight);

            /*
             return list;
            */
            GotoExpression returnExpression = Expression.Return(returnTarget, array);
            LabelExpression returnLabel = Expression.Label(returnTarget, array);
            methodListCall[9] = returnExpression;
            methodListCall[10] = returnLabel;

            return Expression.Block(new[] { arrayLength, array, idx, ExpressionMembers.MoveNext }, methodListCall);
        }
    }
}
