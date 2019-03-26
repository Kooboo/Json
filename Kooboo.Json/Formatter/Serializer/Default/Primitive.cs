using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Serializer
{
    internal class PrimitiveNormal : DefaultJsonFormatter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(int value, JsonSerializerHandler handler)
        {
            // Gotta special case this, we can't negate it
            if (value == int.MinValue)
            {
                handler.Writer.Append("-2147483648");
                return;
            }
            if (value == int.MaxValue)
            {
                handler.Writer.Append("2147483647");
                return;
            }
            var ptr = 35;
            char[] buffer = new char[36];
            uint copy;
            if (value < 0)
            {
                handler.Writer.Append('-');
                copy = (uint)(-value);
            }
            else
                copy = (uint)value;

            do
            {
                byte ix = (byte)(copy % 100);
                copy /= 100;

                var chars = SpecialTypeNormal. DigitPairs[ix];
                buffer[ptr--] = chars.Second;
                buffer[ptr--] = chars.First;
            } while (copy != 0);

            if (buffer[ptr + 1] == '0')
            {
                ptr++;
            }

            handler.Writer.Append(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(uint value, JsonSerializerHandler handler)
        {
            if (value == uint.MaxValue)
            {
                handler.Writer.Append("4294967295");
                return;
            }
            var ptr = 35;
            char[] buffer = new char[36];
            var copy = value;

            do
            {
                byte ix = (byte)(copy % 100);
                copy /= 100;

                var chars = SpecialTypeNormal.DigitPairs[ix];
                buffer[ptr--] = chars.Second;
                buffer[ptr--] = chars.First;
            } while (copy != 0);

            if (buffer[ptr + 1] == '0')
            {
                ptr++;
            }

            handler.Writer.Append(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(long value, JsonSerializerHandler handler)
        {
            if (value == long.MinValue)
            {
                handler.Writer.Append("-9223372036854775808");
                return;
            }
            char[] buffer = new char[36];
            var ptr = 35;

            ulong copy;
            if (value < 0)
            {
                handler.Writer.Append('-');
                copy = (ulong)(-value);
            }
            else
            {
                copy = (ulong)value;
            }

            do
            {
                byte ix = (byte)(copy % 100);
                copy /= 100;

                var chars = SpecialTypeNormal.DigitPairs[ix];
                buffer[ptr--] = chars.Second;
                buffer[ptr--] = chars.First;
            } while (copy != 0);

            if (buffer[ptr + 1] == '0')
            {
                ptr++;
            }

            handler.Writer.Append(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(ulong value, JsonSerializerHandler handler)
        {
            if (value == ulong.MaxValue)
            {
                handler.Writer.Append("18446744073709551615");
                return;
            }

            var ptr = 35;
            char[] buffer = new char[36];
            var copy = value;

            do
            {
                byte ix = (byte)(copy % 100);
                copy /= 100;

                var chars = SpecialTypeNormal.DigitPairs[ix];
                buffer[ptr--] = chars.Second;
                buffer[ptr--] = chars.First;
            } while (copy != 0);

            if (buffer[ptr + 1] == '0')
            {
                ptr++;
            }

            handler.Writer.Append(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(float value, JsonSerializerHandler handler)
        {
            switch (value)
            {
                case float.NaN:
                    handler.Writer.Append("\"NaN\"");
                    return;
                case float.NegativeInfinity:
                    handler.Writer.Append("\"-Infinity\"");
                    return;
                case float.PositiveInfinity:
                    handler.Writer.Append("\"Infinity\"");
                    return;
            }
            handler.Writer.Append(value.ToString("R",CultureInfo.InvariantCulture));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(decimal value, JsonSerializerHandler handler)
        {
            handler.Writer.Append(value.ToString(CultureInfo.InvariantCulture));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(double value, JsonSerializerHandler handler)
        {
            switch (value)
            {
                case double.NaN:
                    handler.Writer.Append("\"NaN\"");
                    return;
                case double.NegativeInfinity:
                    handler.Writer.Append("\"-Infinity\"");
                    return;
                case double.PositiveInfinity:
                    handler.Writer.Append("\"Infinity\"");
                    return;
            }
            handler.Writer.Append(value.ToString("R", CultureInfo.InvariantCulture));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(byte value, JsonSerializerHandler handler)
        {
            WriteValue((int)value, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(sbyte value, JsonSerializerHandler handler)
        {
            WriteValue((int)value, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(short value, JsonSerializerHandler handler)
        {
            WriteValue((int)value, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(ushort value, JsonSerializerHandler handler)
        {
            WriteValue((int)value, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(bool value, JsonSerializerHandler handler)
        {
            handler.Writer.Append(value ? "true" : "false");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(char value, JsonSerializerHandler handler)
        {
            WriteValue(char.ToString(value), handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static unsafe void WriteValue(string value, JsonSerializerHandler handler)
        {
            /*
               string = quotation-mark *char quotation-mark
               
               char = unescaped /
                   escape (
                       %x22 /          ; "    quotation mark  U+0022
                       %x5C /          ; \    reverse solidus U+005C
                       %x2F /          ; /    solidus         U+002F
                       %x62 /          ; b    backspace       U+0008
                       %x66 /          ; f    form feed       U+000C
                       %x6E /          ; n    line feed       U+000A
                       %x72 /          ; r    carriage return U+000D
                       %x74 /          ; t    tab             U+0009
                       %x75 4HEXDIG )  ; uXXXX                U+XXXX
                
               escape = %x5C              ; \

               quotation-mark = %x22      ; "

               unescaped = %x20-21 / %x23-5B / %x5D-10FFFF
             */
            if (value == null)
            {
                handler.Writer.Append("null");
                return;
            }
            //handler.Writer.EnsureCapacity(handler.Writer.Capacity + value.Length);
            handler.Writer.Append("\"");
            fixed (char* strFixed = value)
            {
                char* str = strFixed;
                char c;
                var len = value.Length;
                while (len > 0)
                {
                    c = *str;
                    str++;
                    len--;

                    if (c == '\\')//\u005c   //%x5c        a\\b  =>  a\u005cb
                    {
                        handler.Writer.Append(@"\\");
                        continue;
                    }

                    if (c == '"')//\u0022   //%x22
                    {
                        handler.Writer.Append("\\\"");
                        continue;
                    }
                    switch (c)
                    {
                        //%x00-x19
                        case '\u0000': handler.Writer.Append(@"\u0000"); continue;
                        case '\u0001': handler.Writer.Append(@"\u0001"); continue;
                        case '\u0002': handler.Writer.Append(@"\u0002"); continue;
                        case '\u0003': handler.Writer.Append(@"\u0003"); continue;
                        case '\u0004': handler.Writer.Append(@"\u0004"); continue;
                        case '\u0005': handler.Writer.Append(@"\u0005"); continue;
                        case '\u0006': handler.Writer.Append(@"\u0006"); continue;
                        case '\u0007': handler.Writer.Append(@"\u0007"); continue;
                        case '\u0008': handler.Writer.Append(@"\b"); continue;
                        case '\u0009': handler.Writer.Append(@"\t"); continue;
                        case '\u000A': handler.Writer.Append(@"\n"); continue;
                        case '\u000B': handler.Writer.Append(@"\u000b"); continue;
                        case '\u000C': handler.Writer.Append(@"\f"); continue;
                        case '\u000D': handler.Writer.Append(@"\r"); continue;
                        case '\u000E': handler.Writer.Append(@"\u000e"); continue;
                        case '\u000F': handler.Writer.Append(@"\u000f"); continue;
                        case '\u0010': handler.Writer.Append(@"\u0010"); continue;
                        case '\u0011': handler.Writer.Append(@"\u0011"); continue;
                        case '\u0012': handler.Writer.Append(@"\u0012"); continue;
                        case '\u0013': handler.Writer.Append(@"\u0013"); continue;
                        case '\u0014': handler.Writer.Append(@"\u0014"); continue;
                        case '\u0015': handler.Writer.Append(@"\u0015"); continue;
                        case '\u0016': handler.Writer.Append(@"\u0016"); continue;
                        case '\u0017': handler.Writer.Append(@"\u0017"); continue;
                        case '\u0018': handler.Writer.Append(@"\u0018"); continue;
                        case '\u0019': handler.Writer.Append(@"\u0019"); continue;
                        case '\u001A': handler.Writer.Append(@"\u001a"); continue;
                        case '\u001B': handler.Writer.Append(@"\u001b"); continue;
                        case '\u001C': handler.Writer.Append(@"\u001c"); continue;
                        case '\u001D': handler.Writer.Append(@"\u001d"); continue;
                        case '\u001E': handler.Writer.Append(@"\u001e"); continue;
                        case '\u001F': handler.Writer.Append(@"\u001f"); continue;
                        /*JavaScript   */
                        case '\u0085': // Next Line
                            handler.Writer.Append(@"\u0085"); continue;
                        case '\u2028': // Line Separator
                            handler.Writer.Append(@"\u2028"); continue;
                        case '\u2029': // Paragraph Separator
                            handler.Writer.Append(@"\u2029"); continue;
                        default: handler.Writer.Append(c); continue;
                    }
                }
            }
            handler.Writer.Append("\"");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(IntPtr value, JsonSerializerHandler handler)
        {
            WriteValue((long)value, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(UIntPtr value, JsonSerializerHandler handler)
        {
            WriteValue((ulong)value, handler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(object obj, JsonSerializerHandler handler)
        {
            if (obj == null)
            {
                handler.Writer.Append("null");
                return;
            }
            var type = obj.GetType();
            if (type.IsAnonymousType() || type == typeof(object))
                SpecialConditions.WriteDynamic(obj, handler);
            else
            {
                var jumpAction = SerializerObjectJump.GetThreadSafetyJumpAction(type);
                jumpAction(obj, handler);
            }
        }

    }
}
