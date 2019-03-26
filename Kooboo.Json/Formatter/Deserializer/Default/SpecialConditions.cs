using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Deserialize
{
    internal class SpecialConditionsResolve : DefaultJsonResolve
    {
        #region pregenerated metas
        internal static MethodInfo _ReadAvoidNull = typeof(SpecialConditionsResolve).GetMethod(nameof(ReadAvoidNull), BindingFlags.NonPublic | BindingFlags.Static);
        internal static MethodInfo _ReadNullable = typeof(SpecialConditionsResolve).GetMethod(nameof(ReadNullable), BindingFlags.NonPublic | BindingFlags.Static);
        internal static MethodInfo _ReadEnum = typeof(SpecialConditionsResolve).GetMethod(nameof(ReadEnum), BindingFlags.NonPublic | BindingFlags.Static);
        #endregion

        //---------Avoid
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReadAvoidNull(ref JsonReader reader)
        {
            char c = reader.BeforAnnotation();
            if (c == 'n' && reader.StrCompair("ull"))
                return;
            if (c == '{' && reader.GetChar() == '}')
                return;
            throw new JsonWrongCharacterException(reader);
        }

        //---------T?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T? ReadNullable<T>(ref JsonReader reader, JsonDeserializeHandler handler) where T : struct
        {
            var c = reader.BeforAnnotation();
            if (c == 'n')
            {
                if (reader.StrCompair("ull"))
                    return null;
                throw new JsonDeserializationTypeResolutionException(typeof(T?));
            }
            else
            {
                reader.RollbackChar();
                return (T?)ResolveProvider<T>.InvokeGet(ref reader, handler);
            }
        }

        //---------Enum
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object ReadEnum(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            var c = reader.BeforAnnotation();
            var t = handler.Types.Dequeue();
            if (c == '"')
            {
                reader.RollbackChar();
                var str = reader.ReadString();
                return Enum.Parse(t, str);
            }

            if (c >= '0' && c <= '9')// byte、sbyte、short、ushort、int、uint、long 、ulong  default ->int
            {
                reader.RollbackChar();
                var basicType = Enum.GetUnderlyingType(t);
                if (basicType == typeof(long))
                    return PrimitiveResolve.ReadLong(ref reader, handler);
                else if (basicType == typeof(ulong))
                    return PrimitiveResolve.ReadULong(ref reader, handler);
                else if (basicType == typeof(uint))
                    return PrimitiveResolve.ReadUInt(ref reader, handler);
                else
                    return PrimitiveResolve.ReadInt(ref reader, handler);
            }
            throw new JsonDeserializationTypeResolutionException(reader, t);
        }
    }
}
