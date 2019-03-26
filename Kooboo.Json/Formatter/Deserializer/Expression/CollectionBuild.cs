using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.Collection)]
    internal class CollectionBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type defindType, Type instanceType, Type convertType,Type arrayItemType)
        {
            /*
                //defind:IEnumerable<string>    new: List<string>  convert: List<string>
                IEnumerable<string> li = new List<string>();
                List<string> list = (List<string>)li;

                //defind:ArrayList   new:ArrayList   convert: IList/ArrayList
                 ArrayList v = new ArrayList();
                 var df = (IList)v;
             */

            Expression[] methodListCall = new Expression[9];

            LabelTarget returnTarget = Expression.Label(defindType, "returnLable");
            Expression ifReadNullOrArrayLeftReturnNull;
            if (!defindType.IsValueType)
            {
                /*
                if (reader.ReadNullOrArrayLeft())
                     return null;    
                */
                ifReadNullOrArrayLeftReturnNull = Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadNullOrArrayLeft), Expression.Return(returnTarget, Expression.Constant(null, defindType)));
            }
            else
            {
                ifReadNullOrArrayLeftReturnNull = Expression.Call(ExpressionMembers.Reader,JsonReader._ReadArrayLeft);
            }
            methodListCall[0] = ifReadNullOrArrayLeftReturnNull;

            /*
            List<> list =new List<>();
            */
            NewExpression listCtor = Expression.New(instanceType);
            ParameterExpression list = Expression.Variable(defindType, "list");
            methodListCall[1] = Expression.Assign(list, listCtor);

            /*
               if(reader.ReadBoolArrayRight)
                     return list;
              */
            methodListCall[2] = (Expression.IfThen(Expression.Call(ExpressionMembers.Reader, JsonReader._ReadBoolArrayRight), Expression.Return(returnTarget, list)));

            /*
               ICollection<int> iCollec = (ICollection<int>)list;
               iCollec.Add(ReadInt())
           */
            MethodInfo iCollecAdd = convertType.GetMethod("Add",BindingFlags.Public|BindingFlags.Instance);

            ParameterExpression iCollec = Expression.Variable(convertType, "iDic");
            methodListCall[3] = Expression.Assign(iCollec, Expression.Convert(list, convertType));

            /*
            int moveNext=1;
           */
            methodListCall[4] = ExpressionMembers.MoveNextAssignOne;

            /*
            while(moveNext-->0)
               {}
            */
            LabelTarget loopBreak = Expression.Label("loopBreak");
            LoopExpression loopExpression = Expression.Loop(Expression.IfThenElse(ExpressionMembers.MoveNextDecrement,
                ReturnFunc<Expression>(() =>
                {
                    Expression[] methodCall = new Expression[2];
                    /*
                       list.Add(item);
                     */
                    methodCall[0] = Expression.Call(iCollec, iCollecAdd, ExpressionMembers.GetMethodCall(arrayItemType));
                    /*
                       if(reader.ReadBoolComma()==true)
                             moveNext++;
                     */
                    methodCall[1] = ExpressionMembers.IfReadBoolCommaIsTrueSoMoveNextIncrement;

                    return Expression.Block(methodCall); ;
                })
                , Expression.Break(loopBreak)));
            methodListCall[5] = Expression.Block(loopExpression, Expression.Label(loopBreak));

            /*
             reader.ReadArrayRight()
             */
            methodListCall[6] = Expression.Call(ExpressionMembers.Reader, JsonReader._ReadArrayRight);

            /*
             return list;
            */
            GotoExpression returnExpression = Expression.Return(returnTarget, list);
            LabelExpression returnLabel = Expression.Label(returnTarget, list);
            methodListCall[7] = returnExpression;
            methodListCall[8] = returnLabel;

            return Expression.Block(new[] { iCollec, list, ExpressionMembers.MoveNext }, methodListCall);
        }
    }
}
