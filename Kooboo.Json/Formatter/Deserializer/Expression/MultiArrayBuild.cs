using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kooboo.Json.Deserialize
{
    [ExpressionBuildType(DeserializeBuildTypeEnum.MultiArray)]
    internal class MultiArrayBuild : ExpressionJsonResolve
    {
        internal static BlockExpression Build(Type multiType, Type sawtoothType, int rank)
        {
            LabelTarget returnValueLable = Expression.Label(multiType, "returnValue");
            List<ParameterExpression> lambdaVariables = new List<ParameterExpression>();
            Expression[] exs = new Expression[3];

            ParameterExpression sawtoothAry = Expression.Variable(sawtoothType);
            lambdaVariables.Add(sawtoothAry);
            /*
             int[][][] sawtoothAry= InvokeGet<int[][][]>();
             */
            exs[0] = Expression.Assign(sawtoothAry, ExpressionMembers.GetMethodCall(sawtoothType));

            /*
             if(sawtoothAry==null)
                   return null;
             else
                   ...
             */
            exs[1] = Expression.IfThenElse(
                Expression.Equal(sawtoothAry, Expression.Constant(null, sawtoothType)),
                Expression.Return(returnValueLable, Expression.Constant(null, multiType)),
                ReturnFunc<Expression>(() =>
                {
                    List<Expression> expressions = new List<Expression>();

                    /*
                      var index0_Length = sawtoothAry.Length;
                      var index1_Length = sawtoothAry[0].Length;
                      var index2_Length = sawtoothAry[0][0].Length;  

                      var a = 0;
                      var b = 0;
                      var c = 0;
                    */
                    ParameterExpression[] arrayLengthVariables = new ParameterExpression[rank];
                    ParameterExpression[] forVariables = new ParameterExpression[rank];
                    for (int i = 0; i < arrayLengthVariables.Length; i++)
                    {
                        arrayLengthVariables[i] = Expression.Variable(typeof(int));
                        expressions.Add(Expression.Assign(arrayLengthVariables[i], Expression.MakeMemberAccess(GetIndexAccessBySawtoothAry(sawtoothAry, GetExpressionConstantZero(i).ToArray()), typeof(Array).GetProperty("Length"))));

                        forVariables[i] = Expression.Variable(typeof(int));
                    }
                    lambdaVariables.AddRange(arrayLengthVariables);
             
                    /*
                      int[,,] multiAry=new multiAry(index0_Length,index1_Length,index2_Length)
                     */
                    ParameterExpression multiAry = Expression.Variable(multiType);
                    lambdaVariables.Add(multiAry);

                    expressions.Add(Expression.Assign(multiAry, Expression.New(multiType.GetConstructors()[0], arrayLengthVariables)));

                    /*
                        for (int a = 0; a < index0_Length; a++)
                        {
                            for (int b = 0; b < index1_Length; b++)
                            {
                                for (int c = 0; c < index2_Length; c++)
                                {
                                    multiAry[a,b,c] = sawtoothAry[a][b][c];
                                }
                            }
                        }
                     */
                    Expression forMain = null;
                    for (int i = arrayLengthVariables.Length - 1; i >= 0; i--)
                    {
                        Expression content = null;
                        if (forMain == null)
                        {
                            /*
                               sawtoothAry[a][b][c];
                             */
                            Expression sawtoothAryAccess = GetIndexAccessBySawtoothAry(sawtoothAry, forVariables);
                            /*
                               multiAry[a,b,c] = sawtoothAry[a][b][c];
                             */
                            content = Expression.Assign(Expression.ArrayAccess(multiAry, forVariables), sawtoothAryAccess);
                        }
                        else
                            content = forMain;

                        forMain = ExpressionHelper.For(forVariables[i], Expression.LessThan(forVariables[i], arrayLengthVariables[i]), Expression.PostIncrementAssign(forVariables[i]),
                            content
                             );
                    }
                    expressions.Add(forMain);

                    /*
                     return  multiAry
                    */
                    expressions.Add(Expression.Return(returnValueLable, multiAry));

                    return Expression.Block(expressions);
                }));

            exs[2] = Expression.Label(returnValueLable, Expression.Constant(null, multiType));

            return Expression.Block(lambdaVariables.ToArray(), exs);
        }

        internal static IEnumerable<Expression> GetExpressionConstantZero(int num)
        {
            for (int i = 0; i < num; i++)
            {
                yield return Expression.Constant(0);
            }
        }

        internal static Expression GetIndexAccessBySawtoothAry(ParameterExpression sawtoothAry, Expression[] indexs)
        {
            if (!indexs.Any())
                return sawtoothAry;
            Expression lastAccess = null;
            foreach (var t in indexs)
            {
                lastAccess = Expression.ArrayAccess(lastAccess ?? sawtoothAry, t);
            }
            return lastAccess;
        }

        
    }
}
