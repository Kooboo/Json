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
                handler.WriteString("-2147483648");
                return;
            }
            if (value == int.MaxValue)
            {
                handler.WriteString("2147483647");
                return;
            }
            var ptr = 35;
            char[] buffer = new char[36];
            uint copy;
            if (value < 0)
            {
                handler.WriteChar('-');
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

            handler.WriteChars(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(uint value, JsonSerializerHandler handler)
        {
            if (value == uint.MaxValue)
            {
                handler.WriteString("4294967295");
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

            handler.WriteChars(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(long value, JsonSerializerHandler handler)
        {
            if (value == long.MinValue)
            {
                handler.WriteString("-9223372036854775808");
                return;
            }
            char[] buffer = new char[36];
            var ptr = 35;

            ulong copy;
            if (value < 0)
            {
                handler.WriteChar('-');
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

            handler.WriteChars(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(ulong value, JsonSerializerHandler handler)
        {
            if (value == ulong.MaxValue)
            {
                handler.WriteString("18446744073709551615");
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

            handler.WriteChars(buffer, ptr + 1, 35 - ptr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(float value, JsonSerializerHandler handler)
        {
            switch (value)
            {
                case float.NaN:
                    handler.WriteString("\"NaN\"");
                    return;
                case float.NegativeInfinity:
                    handler.WriteString("\"-Infinity\"");
                    return;
                case float.PositiveInfinity:
                    handler.WriteString("\"Infinity\"");
                    return;
            }
            handler.WriteString(value.ToString("R",CultureInfo.InvariantCulture));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(decimal value, JsonSerializerHandler handler)
        {
            handler.WriteString(value.ToString(CultureInfo.InvariantCulture));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(double value, JsonSerializerHandler handler)
        {
            switch (value)
            {
                case double.NaN:
                    handler.WriteString("\"NaN\"");
                    return;
                case double.NegativeInfinity:
                    handler.WriteString("\"-Infinity\"");
                    return;
                case double.PositiveInfinity:
                    handler.WriteString("\"Infinity\"");
                    return;
            }
            handler.WriteString(value.ToString("R", CultureInfo.InvariantCulture));
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
            handler.WriteString(value ? "true" : "false");
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
                handler.WriteString("null");
                return;
            }
            //handler.Writer.EnsureCapacity(handler.Writer.Capacity + value.Length);
            handler.WriteString("\"");
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
                        handler.WriteString(@"\\");
                        continue;
                    }

                    if (c == '"')//\u0022   //%x22
                    {
                        handler.WriteString("\\\"");
                        continue;
                    }
                    switch (c)
                    {
                        //%x00-x19
                        case '\u0000': handler.WriteString(@"\u0000"); continue;
                        case '\u0001': handler.WriteString(@"\u0001"); continue;
                        case '\u0002': handler.WriteString(@"\u0002"); continue;
                        case '\u0003': handler.WriteString(@"\u0003"); continue;
                        case '\u0004': handler.WriteString(@"\u0004"); continue;
                        case '\u0005': handler.WriteString(@"\u0005"); continue;
                        case '\u0006': handler.WriteString(@"\u0006"); continue;
                        case '\u0007': handler.WriteString(@"\u0007"); continue;
                        case '\u0008': handler.WriteString(@"\b"); continue;
                        case '\u0009': handler.WriteString(@"\t"); continue;
                        case '\u000A': handler.WriteString(@"\n"); continue;
                        case '\u000B': handler.WriteString(@"\u000b"); continue;
                        case '\u000C': handler.WriteString(@"\f"); continue;
                        case '\u000D': handler.WriteString(@"\r"); continue;
                        case '\u000E': handler.WriteString(@"\u000e"); continue;
                        case '\u000F': handler.WriteString(@"\u000f"); continue;
                        case '\u0010': handler.WriteString(@"\u0010"); continue;
                        case '\u0011': handler.WriteString(@"\u0011"); continue;
                        case '\u0012': handler.WriteString(@"\u0012"); continue;
                        case '\u0013': handler.WriteString(@"\u0013"); continue;
                        case '\u0014': handler.WriteString(@"\u0014"); continue;
                        case '\u0015': handler.WriteString(@"\u0015"); continue;
                        case '\u0016': handler.WriteString(@"\u0016"); continue;
                        case '\u0017': handler.WriteString(@"\u0017"); continue;
                        case '\u0018': handler.WriteString(@"\u0018"); continue;
                        case '\u0019': handler.WriteString(@"\u0019"); continue;
                        case '\u001A': handler.WriteString(@"\u001a"); continue;
                        case '\u001B': handler.WriteString(@"\u001b"); continue;
                        case '\u001C': handler.WriteString(@"\u001c"); continue;
                        case '\u001D': handler.WriteString(@"\u001d"); continue;
                        case '\u001E': handler.WriteString(@"\u001e"); continue;
                        case '\u001F': handler.WriteString(@"\u001f"); continue;
                        /*JavaScript   */
                        case '\u0085': // Next Line
                            handler.WriteString(@"\u0085"); continue;
                        case '\u2028': // Line Separator
                            handler.WriteString(@"\u2028"); continue;
                        case '\u2029': // Paragraph Separator
                            handler.WriteString(@"\u2029"); continue;
                        default: handler.WriteChar(c); continue;
                    }
                }
            }
            handler.WriteString("\"");
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
                handler.WriteString("null");
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
