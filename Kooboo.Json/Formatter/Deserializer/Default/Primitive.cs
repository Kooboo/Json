using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kooboo.Json.Deserialize
{
    internal class PrimitiveResolve : DefaultJsonResolve
    {
        #region pregenerated metas
        internal static MethodInfo _ReadEscapeString = GetMethodInfo(nameof(ReadEscapeString));
        #endregion

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ReadInt(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            int c = reader.BeforAnnotation();

            // max:  2147483647
            // min: -2147483648
            // digits:       10
            long ret = 0;
            bool negative = false;

            if (c == '-')
            {
                negative = true;
                c = reader.GetChar();
            }

            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(reader, typeof(int));

            bool isFirstDigitZero = c == '0';

            c = c - '0';
            ret += c;

            if (reader.Remaining == 0)
                return (int)(ret * (negative ? -1 : 1));

            // digit #2
            c = reader.GetChar();
            if (c < '0' || c > '9')
            {
                reader.RollbackChar();
                return (int)(ret * (negative ? -1 : 1));
            }
            if (isFirstDigitZero)
                throw new JsonDeserializationTypeResolutionException(reader, typeof(int));

            c = c - '0';
            ret *= 10;
            ret += c;

            while (reader.Remaining > 0)
            {
                c = reader.GetChar();

                if (c < '0' || c > '9')
                {
                    reader.RollbackChar();
                    goto Return;
                }

                c = c - '0';
                ret *= 10;
                ret += c;
            }
        // 9999999999 > int.Max
        Return: checked
            {
                return (int)(ret * (negative ? -1 : 1));
            }
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ReadUInt(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            int c = reader.BeforAnnotation();

            //MaxValue = 4294967295; 10
            //MinValue = 0; 
            if (c < '0' || c > '9')
                if (c == '-' && reader.GetChar() == '0')
                    return 0;
                else
                    throw new JsonDeserializationTypeResolutionException(typeof(uint));

            long ret = 0;

            var firstDigitZero = c == '0';

            c = c - '0';
            ret += (uint)c;

            if (reader.Remaining == 0)
                return (uint)ret;

            c = reader.GetChar();
            if (c < '0' || c > '9')
            {
                reader.RollbackChar();
                return (uint)ret;
            }
            if (firstDigitZero)
                throw new JsonDeserializationTypeResolutionException(reader, typeof(uint));

            c = c - '0';
            ret *= 10;
            ret += c;

            while (reader.Remaining > 0)
            {
                c = reader.GetChar();

                if (c < '0' || c > '9')
                {
                    reader.RollbackChar();
                    goto Return;
                }

                c = c - '0';
                ret *= 10;
                ret += (uint)c;
            }

        Return: checked
            {
                return (uint)ret;
            }
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long ReadLong(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            int c = reader.BeforAnnotation();

            //MaxValue =  9223372036854775807; 19
            //MinValue = -9223372036854775808; 20
            //ulong      18446744073709551615; 20
            ulong ret = 0;
            var negative = false;

            if (c == '-')
            {
                negative = true;
                c = reader.GetChar();
            }

            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(reader, typeof(long));

            var firstDigitZero = c == '0';

            c = c - '0';
            ret += (uint)c;

            if (reader.Remaining == 0)
                return ((long)ret * (negative ? -1 : 1));

            c = reader.GetChar();
            if (c < '0' || c > '9')
            {
                reader.RollbackChar();
                return ((long)ret * (negative ? -1 : 1));
            }
            if (firstDigitZero)
                throw new JsonDeserializationTypeResolutionException(reader, typeof(long));

            c = c - '0';
            ret *= 10;
            ret += (uint)c;

            while (reader.Remaining > 0)
            {
                c = reader.GetChar();

                if (c < '0' || c > '9')
                {
                    reader.RollbackChar();
                    goto Return;
                }

                c = c - '0';
                ret *= 10;
                ret += (uint)c;
            }
            if (ret == 9223372036854775808 && negative)
                return long.MinValue;
            Return: checked
            {
                return ((long)ret * (negative ? -1 : 1));
            }
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ReadULong(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            uint c = reader.BeforAnnotation();

            if (c < '0' || c > '9')
                if (c == '-' && reader.GetChar() == '0')
                    return 0;
                else
                    throw new JsonDeserializationTypeResolutionException(typeof(ulong));

            //MaxValue = 18446744073709551615; 20
            //MinValue = 0; 
            ulong ret = 0;
            var firstDigitZero = c == '0';

            c = c - '0';
            ret += c;

            if (reader.Remaining == 0)
                return ret;

            c = reader.GetChar();
            if (c < '0' || c > '9')
            {
                reader.RollbackChar();
                return ret;
            }
            if (firstDigitZero)
                throw new JsonDeserializationTypeResolutionException(reader, typeof(uint));

            c = c - '0';
            ret *= 10;
            ret += c;

            int num = 2;
            while (reader.Remaining > 0)
            {
                ++num;
                c = reader.GetChar();
                if (c < '0' || c > '9')
                {
                    reader.RollbackChar();
                    goto Return;
                }

                if (num == 20)
                {
                    checked
                    {
                        c = c - '0';
                        ret *= 10;
                        ret += c;
                    }
                }
                else
                {
                    c = c - '0';
                    ret *= 10;
                    ret += c;
                }
            }
        Return: checked
            {
                return ret;
            }
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ReadEscapeString(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            var c = reader.BeforAnnotation();
            if (c == '"')
            {
                int start = reader.Length - reader.Remaining;
                int length = 0;
                StringBuilder charBufferSb = reader.CharBufferSb ?? new StringBuilder();
                int sbStart = charBufferSb.Length;
                while (reader.Remaining > 0)
                {
                    c = reader.GetChar();
                    if (c == '"')//end
                    {
                        if (length > 0)
                            charBufferSb.Append(reader.Json, start, length);
                        return charBufferSb.ToString(sbStart, charBufferSb.Length - sbStart);
                    }

                    if (c != '\\')
                    {
                        ++length;
                        continue;
                    }
                    if (length > 0)
                    {
                        charBufferSb.Append(reader.Json, start, length);
                        start += length;
                        length = 0;
                    }

                    /*
                     string s = "a\\\\b"; 
                     string s = "a\\u005cb";
                     */
                    start += 2;
                    if (reader.Remaining < 1) //   \后面必须接任意一个符号，否则字符串有误
                        throw new JsonDeserializationTypeResolutionException(reader, typeof(string));

                    c = reader.GetChar();
                    switch (c)
                    {
                        case '"': charBufferSb.Append('"'); continue;
                        case '\\': charBufferSb.Append('\\'); continue;
                        case '/': charBufferSb.Append('/'); continue;
                        case 'b': charBufferSb.Append('\b'); continue;
                        case 'f': charBufferSb.Append('\f'); continue;
                        case 'n': charBufferSb.Append('\n'); continue;
                        case 'r': charBufferSb.Append('\r'); continue;
                        case 't': charBufferSb.Append('\t'); continue;
                        case 'u':
                            {   // \\uXXXX
                                if (reader.Remaining > 4)//4+'"'
                                {
                                    int idx = charBufferSb.Length;
                                    charBufferSb.Append(reader.GetChar());
                                    charBufferSb.Append(reader.GetChar());
                                    charBufferSb.Append(reader.GetChar());
                                    charBufferSb.Append(reader.GetChar());
                                    int unicode = (((((charBufferSb[idx].CharToNumber() * 16) + charBufferSb[++idx].CharToNumber()) * 16) + charBufferSb[++idx].CharToNumber()) * 16) + charBufferSb[++idx].CharToNumber();
                                    charBufferSb.Remove(idx - 3, 4);
                                    charBufferSb.Append((char)unicode);

                                    start += 4;
                                    continue;
                                }
                                throw new JsonWrongCharacterException(reader, "It should be four hexadecimal digits , such as \\uFFFF");
                            }
                        default:
                            throw new JsonWrongCharacterException(reader, c);
                    }
                }
            }
            else if (c == 'n' && reader.StrCompair("ull"))
                return null;

            throw new JsonDeserializationTypeResolutionException(reader, typeof(string));
        }

        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ReadEscapeChar(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            var c = reader.BeforAnnotation();
            if (c == '"')
            {
                c = reader.GetChar();
                if (c != '\\')//char 只有1位
                {
                    if (reader.GetChar() == '"')
                        return c;
                    throw new JsonWrongCharacterException(reader);
                }

                c = reader.GetChar();
                switch (c)
                {
                    case '"': { c = reader.GetChar(); if (c == '"') return '"'; throw new JsonWrongCharacterException(reader); }
                    case '\\': { c = reader.GetChar(); if (c == '"') return '\\'; throw new JsonWrongCharacterException(reader); }
                    case '/': { c = reader.GetChar(); if (c == '"') return '/'; throw new JsonWrongCharacterException(reader); }
                    case 'b': { c = reader.GetChar(); if (c == '"') return '\b'; throw new JsonWrongCharacterException(reader); }
                    case 'f': { c = reader.GetChar(); if (c == '"') return '\f'; throw new JsonWrongCharacterException(reader); }
                    case 'n': { c = reader.GetChar(); if (c == '"') return '\n'; throw new JsonWrongCharacterException(reader); }
                    case 'r': { c = reader.GetChar(); if (c == '"') return '\r'; throw new JsonWrongCharacterException(reader); }
                    case 't': { c = reader.GetChar(); if (c == '"') return '\t'; throw new JsonWrongCharacterException(reader); }
                    case 'u':
                        {
                            if (reader.Remaining < 4)
                                throw new JsonWrongCharacterException(reader);

                            int unicode = (((((reader.GetChar().CharToNumber() * 16) + reader.GetChar().CharToNumber()) * 16) + reader.GetChar().CharToNumber()) * 16) + reader.GetChar().CharToNumber();

                            if (reader.GetChar() == '"')
                                return (char)unicode;
                            break;
                        }
                }
            }
            throw new JsonDeserializationTypeResolutionException(reader, typeof(char));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static IntPtr ReadIntPtr(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            return new IntPtr(ReadLong(ref reader, handler));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static UIntPtr ReadUIntPtr(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            return new UIntPtr(ReadULong(ref reader, handler));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static byte ReadByte(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            checked
            {
                return (byte)ReadInt(ref reader, handler);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static sbyte ReadSByte(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            checked
            {
                return (sbyte)ReadInt(ref reader, handler);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static short ReadShort(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            checked
            {
                return (short)ReadInt(ref reader, handler);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static ushort ReadUShort(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            checked
            {
                return (ushort)ReadInt(ref reader, handler);
            }
        }

        /*
        [-]0[.Num* [e/E +/- Num*]]
        [-]Num[*][.Num* [e/E +/- Num*]]
        */
        private static readonly double[] DoubleDividers = new[] {
            1.0,
            10.0,
            100.0,
            1000.0,
            10000.0,
            100000.0,
            1000000.0,
            10000000.0,
            100000000.0,
            1000000000.0,
            10000000000.0,
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static unsafe double ReadDouble(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            if (c == '\"')
            {
                switch (reader.GetChar())
                {
                    case 'N':
                        if (reader.StrCompair("aN\""))
                            return double.NaN;
                        break;
                    case '-':
                        if (reader.StrCompair("Infinity\""))
                            return double.NegativeInfinity;
                        break;
                    case 'I':
                        if (reader.StrCompair("nfinity\""))
                            return double.PositiveInfinity;
                        break;
                }
                throw new JsonDeserializationTypeResolutionException(typeof(double));
            }
            reader.RollbackChar();

            var idx = 0;
            var prev = -1;

            var firstDigitIdx = -1;
            var firstValidCharIdx = -1;
            var decimalPointIdx = -1;
            var eIdx = -1;

            var pointer = reader.Pointer;
            while (reader.Remaining > 0)
            {
                c = reader.GetChar();

                if (c >= '0' && c <= '9')
                {
                    if (firstDigitIdx < 0)
                    {
                        firstDigitIdx = idx;
                        if (firstValidCharIdx < 0)
                        {
                            firstValidCharIdx = idx;
                        }
                    }
                }
                else
                {
                    if (c == '+')
                    {
                        if (!(prev == 'e' || prev == 'E'))
                        {
                            throw new JsonWrongCharacterException();
                        }
                        firstValidCharIdx = idx;
                    }
                    else
                    {
                        if (c == '-')
                        {
                            if (prev != -1 && !(prev == 'e' || prev == 'E'))
                            {
                                throw new JsonWrongCharacterException();
                            }
                            firstValidCharIdx = idx;
                        }
                        else
                        {
                            if (c == 'e' || c == 'E')
                            {
                                if (eIdx >= 0 || firstDigitIdx < 0)
                                {
                                    throw new JsonWrongCharacterException();
                                }
                                eIdx = idx;
                            }
                            else
                            {
                                if (c == '.')
                                {
                                    if (eIdx >= 0 || decimalPointIdx >= 0)
                                    {
                                        throw new JsonWrongCharacterException();
                                    }
                                    decimalPointIdx = idx;
                                }
                                else
                                {
                                    reader.RollbackChar();
                                    break;
                                }
                            }
                        }
                    }
                }


                idx++;
                prev = c;
            }

            if (firstDigitIdx == -1) throw new JsonWrongCharacterException();

            if (firstDigitIdx == -1) throw new JsonWrongCharacterException();

            if (pointer[idx - 1] == '.') throw new JsonWrongCharacterException();
            if (idx >= 2 && pointer[0] == '0')
            {
                var secondChar = pointer[1];
                if (secondChar != '.' && secondChar != 'e' && secondChar != 'E')
                {
                    throw new JsonWrongCharacterException();
                }
            }
            if (idx >= 3 && pointer[0] == '-' && pointer[1] == '0')
            {
                var secondChar = pointer[2];
                if (secondChar != '.' && secondChar != 'e' && secondChar != 'E')
                {
                    throw new JsonWrongCharacterException();
                }
            }

            if (eIdx < 0)
            {
                var endIdx = idx;
                while (decimalPointIdx >= 0 && endIdx > 1 && pointer[endIdx - 1] == '0')
                {
                    endIdx--;
                }

                var startIdx =
                    decimalPointIdx < 0 ?
                        firstDigitIdx :
                        Math.Min(decimalPointIdx, firstDigitIdx);

                while (startIdx < endIdx && pointer[startIdx] == '0')
                {
                    startIdx++;
                }

                var hasIntegerComponent = pointer[startIdx] != '.';
                var includesDecimalPoint = decimalPointIdx >= 0;
                var lastCharIs5 = endIdx > 1 && pointer[endIdx - 1] == '5';
                var maxChars = 5 +
                    (hasIntegerComponent ? 1 : 0) +
                    (includesDecimalPoint ? 1 : 0) +
                    (lastCharIs5 ? 1 : 0);

                if (endIdx - startIdx <= maxChars)
                {
                    if (decimalPointIdx == endIdx - 1)
                    {
                        decimalPointIdx = -1;
                        endIdx--;
                    }

                    var n = 0;
                    for (idx = startIdx; idx < endIdx; ++idx)
                    {
                        if (idx != decimalPointIdx)
                            n = n * 10 + pointer[idx] - '0';
                    }

                    if (pointer[firstValidCharIdx] == '-')
                    {
                        n = -n;
                    }

                    var result = (double)n;
                    if (decimalPointIdx >= 0)
                    {
                        result /= DoubleDividers[endIdx - decimalPointIdx - 1];
                    }

                    return result;
                }
            }

            return double.Parse(new string(pointer, 0, idx), CultureInfo.InvariantCulture);
        }

        private static readonly decimal[] DecimalMultipliers = new[] {
                1m,
                0.1m,
                0.01m,
                0.001m,
                0.0001m,
                0.00001m,
                0.000001m,
                0.0000001m,
                0.00000001m,
                0.000000001m,
                0.0000000001m,
                0.00000000001m,
                0.000000000001m,
                0.0000000000001m,
                0.00000000000001m,
                0.000000000000001m,
                0.0000000000000001m,
                0.00000000000000001m
            };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static unsafe decimal ReadDecimal(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            reader.BeforAnnotation();
            reader.RollbackChar();

            var idx = 0;
            char c;

            var prev = -1;

            var firstDigitIdx = -1;
            var firstValidCharIdx = -1;
            var decimalPointIdx = -1;
            var eIdx = -1;

            var pointer = reader.Pointer;
            while (reader.Remaining > 0)
            {
                c = reader.GetChar();
                if (c >= '0' && c <= '9')
                {
                    if (firstDigitIdx < 0)
                    {
                        firstDigitIdx = idx;
                        if (firstValidCharIdx < 0)
                        {
                            firstValidCharIdx = idx;
                        }
                    }
                }
                else
                {
                    if (c == '+')
                    {
                        if (!(prev == 'e' || prev == 'E'))
                        {
                            throw new JsonWrongCharacterException();
                        }
                        firstValidCharIdx = idx;
                    }
                    else
                    {
                        if (c == '-')
                        {
                            if (prev != -1 && !(prev == 'e' || prev == 'E'))
                            {
                                throw new JsonWrongCharacterException();
                            }
                            firstValidCharIdx = idx;
                        }
                        else
                        {
                            if (c == 'e' || c == 'E')
                            {
                                if (eIdx >= 0 || firstDigitIdx < 0)
                                {
                                    throw new JsonWrongCharacterException();
                                }
                                eIdx = idx;
                            }
                            else
                            {
                                if (c == '.')
                                {
                                    if (eIdx >= 0 || decimalPointIdx >= 0)
                                    {
                                        throw new JsonWrongCharacterException();
                                    }
                                    decimalPointIdx = idx;
                                }
                                else
                                {
                                    reader.RollbackChar();
                                    break;
                                }
                            }
                        }
                    }
                }


                idx++;

                prev = c;
            }

            if (firstDigitIdx == -1) throw new JsonWrongCharacterException();

            if (firstDigitIdx == -1) throw new JsonWrongCharacterException();

            if (pointer[idx - 1] == '.') throw new JsonWrongCharacterException();
            if (idx >= 2 && pointer[0] == '0')
            {
                var secondChar = pointer[1];
                if (secondChar != '.' && secondChar != 'e' && secondChar != 'E')
                {
                    throw new JsonWrongCharacterException();
                }
            }
            if (idx >= 3 && pointer[0] == '-' && pointer[1] == '0')
            {
                var secondChar = pointer[2];
                if (secondChar != '.' && secondChar != 'e' && secondChar != 'E')
                {
                    throw new JsonWrongCharacterException();
                }
            }

            if (eIdx < 0)
            {
                var endIdx = idx;

                var maxChars = decimalPointIdx < 0 ? 18 : 19;
                if (endIdx - firstDigitIdx <= maxChars)
                {
                    if (decimalPointIdx == endIdx - 1)
                    {
                        decimalPointIdx = -1;
                        endIdx--;
                    }

                    var negative = pointer[firstValidCharIdx] == '-';

                    decimal result;
                    var n1 = 0; // we use int rather than long so as to work well on 32-bit runtime

                    for (idx = firstDigitIdx; idx < endIdx && n1 < 100000000; ++idx)
                    {
                        if (idx != decimalPointIdx)
                        {
                            n1 = n1 * 10 + pointer[idx] - '0';
                        }
                    }

                    if (negative)
                    {
                        n1 = -n1;
                    }

                    if (idx == endIdx)
                    {
                        result = n1;
                    }
                    else
                    {
                        var n2 = 0;
                        var multiplier = 1;
                        while (idx < endIdx)
                        {
                            if (idx != decimalPointIdx)
                            {
                                multiplier *= 10;
                                n2 = n2 * 10 + pointer[idx] - '0';
                            }

                            idx++;
                        }

                        if (negative)
                        {
                            n2 = -n2;
                        }

                        result = (long)n1 * multiplier + n2;
                    }

                    if (decimalPointIdx > 0)
                    {
                        result *= DecimalMultipliers[endIdx - decimalPointIdx - 1];
                    }

                    return result;
                }
            }

            return decimal.Parse(new string(pointer, 0, idx), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        private static readonly float[] SingleDividers = {
            1.0f,
            10.0f,
            100.0f,
            1000.0f,
            10000.0f,
            100000.0f,
            1000000.0f,
            10000000.0f,
            100000000.0f,
            1000000000.0f,
            10000000000.0f,
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static unsafe float ReadFloat(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            if (c == '\"')
            {
                switch (reader.GetChar())
                {
                    case 'N':
                        if (reader.StrCompair("aN\""))
                            return float.NaN;
                        break;
                    case '-':
                        if (reader.StrCompair("Infinity\""))
                            return float.NegativeInfinity;
                        break;
                    case 'I':
                        if (reader.StrCompair("nfinity\""))
                            return float.PositiveInfinity;
                        break;
                }
                throw new JsonDeserializationTypeResolutionException(typeof(float));
            }
            reader.RollbackChar();

            var idx = 0;
            c = '0';

            var prev = -1;

            var firstDigitIdx = -1;
            var firstValidCharIdx = -1;
            var decimalPointIdx = -1;
            var eIdx = -1;
            var pointer = reader.Pointer;
            while (reader.Remaining > 0)
            {
                c = reader.GetChar();
                if (c >= '0' && c <= '9')
                {
                    if (firstDigitIdx < 0)
                    {
                        firstDigitIdx = idx;
                        if (firstValidCharIdx < 0)
                        {
                            firstValidCharIdx = idx;
                        }
                    }
                }
                else
                {
                    if (c == '+')
                    {
                        if (!(prev == 'e' || prev == 'E'))
                        {
                            throw new JsonWrongCharacterException();
                        }
                        firstValidCharIdx = idx;
                    }
                    else
                    {
                        if (c == '-')
                        {
                            if (prev != -1 && !(prev == 'e' || prev == 'E'))
                            {
                                throw new JsonWrongCharacterException();
                            }
                            firstValidCharIdx = idx;
                        }
                        else
                        {
                            if (c == 'e' || c == 'E')
                            {
                                if (eIdx >= 0 || firstDigitIdx < 0)
                                {
                                    throw new JsonWrongCharacterException();
                                }
                                eIdx = idx;
                            }
                            else
                            {
                                if (c == '.')
                                {
                                    if (eIdx >= 0 || decimalPointIdx >= 0)
                                    {
                                        throw new JsonWrongCharacterException();
                                    }
                                    decimalPointIdx = idx;
                                }
                                else
                                {
                                    reader.RollbackChar();
                                    break;
                                }
                            }
                        }
                    }
                }

                idx++;

                prev = c;
            }

            if (firstDigitIdx == -1) throw new JsonWrongCharacterException();

            if (firstDigitIdx == -1) throw new JsonWrongCharacterException();

            if (pointer[idx - 1] == '.') throw new JsonWrongCharacterException();
            if (idx >= 2 && pointer[0] == '0')
            {
                var secondChar = pointer[1];
                if (secondChar != '.' && secondChar != 'e' && secondChar != 'E')
                {
                    throw new JsonWrongCharacterException();
                }
            }
            if (idx >= 3 && pointer[0] == '-' && pointer[1] == '0')
            {
                var secondChar = pointer[2];
                if (secondChar != '.' && secondChar != 'e' && secondChar != 'E')
                {
                    throw new JsonWrongCharacterException();
                }
            }

            if (eIdx < 0)
            {
                var endIdx = idx;
                while (decimalPointIdx >= 0 && endIdx > 1 && pointer[endIdx - 1] == '0')
                {
                    endIdx--;
                }

                var startIdx =
                    decimalPointIdx < 0 ?
                        firstDigitIdx :
                        Math.Min(decimalPointIdx, firstDigitIdx);

                while (startIdx < endIdx && pointer[startIdx] == '0')
                {
                    startIdx++;
                }

                var hasIntegerComponent = pointer[startIdx] != '.';
                var includesDecimalPoint = decimalPointIdx >= 0;
                var maxChars =
                    6 +
                    (hasIntegerComponent ? 1 : 0) +
                    (includesDecimalPoint ? 1 : 0);

                if (endIdx - startIdx <= maxChars)
                {
                    if (decimalPointIdx == endIdx - 1)
                    {
                        decimalPointIdx = -1;
                        endIdx--;
                    }

                    var n = 0;
                    for (idx = startIdx; idx < endIdx; idx++)
                    {
                        if (idx != decimalPointIdx)
                        {
                            n = n * 10 + pointer[idx] - '0';
                        }
                    }

                    if (pointer[firstValidCharIdx] == '-')
                    {
                        n = -n;
                    }

                    var result = (float)n;
                    if (decimalPointIdx >= 0)
                    {
                        result /= SingleDividers[endIdx - decimalPointIdx - 1];
                    }

                    return result;
                }
            }

            return float.Parse(new string(pointer, 0, idx), CultureInfo.InvariantCulture);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static bool ReadBool(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            if (c == 'f' && reader.StrCompair("alse"))
            {
                return false;
            }

            if (c == 't' && reader.StrCompair("rue"))
            {
                return true;
            }
            throw new JsonDeserializationTypeResolutionException(reader, typeof(bool));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static unsafe object ReadObject(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            char c = reader.BeforAnnotation();
            switch (c)
            {
                case 'f':
                    if (reader.StrCompair("alse"))
                        return false;
                    break;
                case 't':
                    if (reader.StrCompair("rue"))
                        return true;
                    break;
                case 'n':
                    if (reader.StrCompair("ull"))
                        return null;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    {
                        int idx = 1;
                        bool negative = c == '-';

                        while (reader.Remaining > 0)
                        {
                            ++idx;
                            c = reader.GetChar();
                            if (c == 'e' || c == 'E' || c == '.')
                            {
                                reader.Remaining += idx;
                                reader.Pointer -= idx;
                                return ReadDouble(ref reader, handler);
                                //double
                            }
                            else if (c < '0' || c > '9')
                            {
                                goto Return;
                            }

                        }
                    Return:
                        reader.Remaining += idx;
                        reader.Pointer -= idx;
                        if (idx <= 19)
                        {
                            //long
                            return ReadLong(ref reader, handler);
                        }
                        else if (idx == 20)
                        {
                            double d = ReadDouble(ref reader, handler);
                            if (negative)
                            {
                                if (d >= long.MinValue)
                                    return (long)d;
                                return d;
                            }//double
                            else
                            {
                                if (d >= ulong.MaxValue)
                                    return d;
                                else
                                    return (ulong)d;
                            }//ulong
                        }
                        else
                        {
                            return ReadDouble(ref reader, handler);
                            //double
                        }
                    }
                case '[':
                    {
                        if (reader.ReadBoolArrayRight())
                            return new JArray();
                        JArray list = new JArray();
                        int moveNext = 1;
                        while (moveNext-- > 0)
                        {
                            list.Add(ReadObject(ref reader, handler));
                            if (reader.ReadBoolComma())
                                moveNext++;
                        }
                        reader.ReadArrayRight();
                        return list;
                    }
                case '{':
                    {
                        if (reader.ReadBoolObjRight())
                            return new JObject();
                        JObject dictionary = new JObject();
                        int moveNext = 1;
                        while (moveNext-- > 0)
                        {
                            var key = ReadEscapeString(ref reader, handler);
                            reader.ReadColon();
                            var value = ReadObject(ref reader, handler);
                            dictionary.Add(key, value);
                            if (reader.ReadBoolComma())
                                moveNext++;
                        }
                        reader.ReadObjRight();
                        return dictionary;
                    }
                case '"':
                    {
                        reader.RollbackChar();
                        return ReadEscapeString(ref reader, handler);
                    }
            }
            throw new JsonDeserializationTypeResolutionException(reader, typeof(object));
        }

        private static MethodInfo GetMethodInfo(string name)
        {
            return typeof(PrimitiveResolve).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
        }
    }
}
