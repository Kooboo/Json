using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Kooboo.Json
{
   internal class ExpressionHelper
    {
        internal static Expression For(ParameterExpression loopVar, Expression condition, Expression increment, Expression loopContent)
        {
            var initAssign = Expression.Assign(loopVar, Expression.Constant(0));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { loopVar },
                initAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        condition,
                        Expression.Block(
                            loopContent,
                            increment
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        internal static Expression ForEach(Expression enumerable, ParameterExpression loopVar, Func<ParameterExpression, Expression> loopContentAc)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(enumerable, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);
            var enumeratorDispose = Expression.Call(enumeratorVar, typeof(IDisposable).GetMethod("Dispose"));

            // The MoveNext method's actually on IEnumerator, not IEnumerator<T>
            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var trueConstant = Expression.Constant(true);

            var loop =
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, trueConstant),
                        Expression.Block(
                            new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContentAc(loopVar)),
                        Expression.Break(breakLabel)),
                    breakLabel);

            var tryFinally =
                Expression.TryFinally(
                    loop,
                    enumeratorDispose);

            var body =
                Expression.Block(
                    new[] { enumeratorVar },
                    enumeratorAssign,
                    tryFinally);

            return body;
        }
    }
}
