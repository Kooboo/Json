using System;

namespace Kooboo.Json.Deserialize
{
    internal class ExpressionJsonResolve : JsonResolveBase
    {
        internal static T ReturnFunc<T>(Func<T> f) => f();
    }
}
