using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kooboo.Json.Serializer
{
    internal partial class SpecialTypeNormal : DefaultJsonFormatter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(DateTime value, JsonSerializerHandler handler)
        {
            if (handler.Option.DatetimeFormat == DatetimeFormatEnum.ISO8601)
            {
                // "yyyy-mm-ddThh:mm:ss.fffffffZ"
                // 0123456789ABCDEFGHIJKL
                //
                // Yes, DateTime.Max is in fact guaranteed to have a 4 digit year (and no more)
                // f of 7 digits allows for 1 Tick level resolution
                char[] buffer = new char[36];
                buffer[0] = '"';

                // Year
                uint val = (uint)value.Year;
                var digits = DigitPairs[(byte)(val % 100)];
                buffer[4] = digits.Second;
                buffer[3] = digits.First;
                digits = DigitPairs[(byte)(val / 100)];
                buffer[2] = digits.Second;
                buffer[1] = digits.First;

                // delimiter
                buffer[5] = '-';

                // Month
                digits = DigitPairs[value.Month];
                buffer[7] = digits.Second;
                buffer[6] = digits.First;

                // Delimiter
                buffer[8] = '-';

                // Day
                digits = DigitPairs[value.Day];
                buffer[10] = digits.Second;
                buffer[9] = digits.First;

                // Delimiter
                buffer[11] = 'T';

                digits = DigitPairs[value.Hour];
                buffer[13] = digits.Second;
                buffer[12] = digits.First;

                // Delimiter
                buffer[14] = ':';

                digits = DigitPairs[value.Minute];
                buffer[16] = digits.Second;
                buffer[15] = digits.First;

                // Delimiter
                buffer[17] = ':';

                digits = DigitPairs[value.Second];
                buffer[19] = digits.Second;
                buffer[18] = digits.First;

                int fracEnd;
                var remainingTicks = (value - new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second)).Ticks;
                if (remainingTicks > 0)
                {
                    buffer[20] = '.';

                    var fracPart = remainingTicks % 100;
                    remainingTicks /= 100;
                    if (fracPart > 0)
                    {
                        digits = DigitPairs[fracPart];
                        buffer[27] = digits.Second;
                        buffer[26] = digits.First;
                        fracEnd = 28;
                    }
                    else
                    {
                        fracEnd = 26;
                    }

                    fracPart = remainingTicks % 100;
                    remainingTicks /= 100;
                    if (fracPart > 0)
                    {
                        digits = DigitPairs[fracPart];
                        buffer[25] = digits.Second;
                        buffer[24] = digits.First;
                    }
                    else
                    {
                        if (fracEnd == 26)
                        {
                            fracEnd = 24;
                        }
                        else
                        {
                            buffer[25] = '0';
                            buffer[24] = '0';
                        }
                    }

                    fracPart = remainingTicks % 100;
                    remainingTicks /= 100;
                    if (fracPart > 0)
                    {
                        digits = DigitPairs[fracPart];
                        buffer[23] = digits.Second;
                        buffer[22] = digits.First;
                    }
                    else
                    {
                        if (fracEnd == 24)
                        {
                            fracEnd = 22;
                        }
                        else
                        {
                            buffer[23] = '0';
                            buffer[22] = '0';
                        }
                    }

                    fracPart = remainingTicks;
                    buffer[21] = (char)('0' + fracPart);
                }
                else
                {
                    fracEnd = 20;
                }

                buffer[fracEnd] = 'Z';
                buffer[fracEnd + 1] = '"';

                handler.WriteChars(buffer, 0, fracEnd + 2);
            }
            else if (handler.Option.DatetimeFormat == DatetimeFormatEnum.RFC1123)
            {
                // ddd, dd MMM yyyy HH:mm:ss GMT'"
                handler.WriteChar('"');

                // compiles as a switch
                switch (value.DayOfWeek)
                {
                    case DayOfWeek.Sunday: handler.WriteString("Sun, "); break;
                    case DayOfWeek.Monday: handler.WriteString("Mon, "); break;
                    case DayOfWeek.Tuesday: handler.WriteString("Tue, "); break;
                    case DayOfWeek.Wednesday: handler.WriteString("Wed, "); break;
                    case DayOfWeek.Thursday: handler.WriteString("Thu, "); break;
                    case DayOfWeek.Friday: handler.WriteString("Fri, "); break;
                    case DayOfWeek.Saturday: handler.WriteString("Sat, "); break;
                }

                {
                    var day = DigitPairs[value.Day];
                    handler.WriteChar(day.First);
                    handler.WriteChar(day.Second);
                    handler.WriteChar(' ');
                }

                // compiles as a switch
                switch (value.Month)
                {
                    case 1: handler.WriteString("Jan "); break;
                    case 2: handler.WriteString("Feb "); break;
                    case 3: handler.WriteString("Mar "); break;
                    case 4: handler.WriteString("Apr "); break;
                    case 5: handler.WriteString("May "); break;
                    case 6: handler.WriteString("Jun "); break;
                    case 7: handler.WriteString("Jul "); break;
                    case 8: handler.WriteString("Aug "); break;
                    case 9: handler.WriteString("Sep "); break;
                    case 10: handler.WriteString("Oct "); break;
                    case 11: handler.WriteString("Nov "); break;
                    case 12: handler.WriteString("Dec "); break;
                }

                {
                    var year = value.Year;
                    var firstHalfYear = DigitPairs[year / 100];
                    handler.WriteChar(firstHalfYear.First);
                    handler.WriteChar(firstHalfYear.Second);

                    var secondHalfYear = DigitPairs[year % 100];
                    handler.WriteChar(secondHalfYear.First);
                    handler.WriteChar(secondHalfYear.Second);
                    handler.WriteChar(' ');
                }

                {
                    var hour = DigitPairs[value.Hour];
                    handler.WriteChar(hour.First);
                    handler.WriteChar(hour.Second);
                    handler.WriteChar(':');
                }

                {
                    var minute = DigitPairs[value.Minute];
                    handler.WriteChar(minute.First);
                    handler.WriteChar(minute.Second);
                    handler.WriteChar(':');
                }

                {
                    var second = DigitPairs[value.Second];
                    handler.WriteChar(second.First);
                    handler.WriteChar(second.Second);
                }

                handler.WriteString(" GMT\"");
            }
            else if (handler.Option.DatetimeFormat == DatetimeFormatEnum.Microsoft)
            {
                /*
                    "\/Date(628318530718)\/" 
                */
                handler.WriteString("\"\\/Date(");
                handler.WriteLong((value.Ticks - 621355968000000000L) / 10000L);
                handler.WriteString(")\\/\"");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(TimeSpan value, JsonSerializerHandler handler)
        {
            if (handler.Option.TimespanFormat == TimespanFormatEnum.ISO8601)
            {
                // can't negate this, have to handle it manually
                if (value.Ticks == long.MinValue)
                {
                    handler.WriteString("\"-P10675199DT2H48M5.4775808S\"");
                    return;
                }
                char[] buffer = new char[36];
                handler.WriteChar('"');

                if (value.Ticks < 0)
                {
                    handler.WriteChar('-');
                    value = value.Negate();
                }

                handler.WriteChar('P');

                var days = value.Days;
                var hours = value.Hours;
                var minutes = value.Minutes;
                var seconds = value.Seconds;

                // days
                if (days > 0)
                {
                    _CustomWriteIntUnrolledSigned(handler, days, buffer);
                    handler.WriteChar('D');
                }

                // time separator
                handler.WriteChar('T');

                // hours
                if (hours > 0)
                {
                    _CustomWriteIntUnrolledSigned(handler, hours, buffer);
                    handler.WriteChar('H');
                }

                // minutes
                if (minutes > 0)
                {
                    _CustomWriteIntUnrolledSigned(handler, minutes, buffer);
                    handler.WriteChar('M');
                }

                // seconds
                _CustomWriteIntUnrolledSigned(handler, seconds, buffer);

                // fractional part
                {
                    var endCount = 0;
                    var remainingTicks = (value - new TimeSpan(days, hours, minutes, seconds, 0)).Ticks;

                    if (remainingTicks > 0)
                    {
                        int fracEnd;

                        buffer[0] = '.';

                        var fracPart = remainingTicks % 100;
                        remainingTicks /= 100;

                        TwoDigits digits;
                        if (fracPart > 0)
                        {
                            digits = DigitPairs[fracPart];
                            buffer[7] = digits.Second;
                            buffer[6] = digits.First;
                            fracEnd = 8;
                        }
                        else
                        {
                            fracEnd = 6;
                        }

                        fracPart = remainingTicks % 100;
                        remainingTicks /= 100;
                        if (fracPart > 0)
                        {
                            digits = DigitPairs[fracPart];
                            buffer[5] = digits.Second;
                            buffer[4] = digits.First;
                        }
                        else
                        {
                            if (fracEnd == 6)
                            {
                                fracEnd = 4;
                            }
                            else
                            {
                                buffer[5] = '0';
                                buffer[4] = '0';
                            }
                        }

                        fracPart = remainingTicks % 100;
                        remainingTicks /= 100;
                        if (fracPart > 0)
                        {
                            digits = DigitPairs[fracPart];
                            buffer[3] = digits.Second;
                            buffer[2] = digits.First;
                        }
                        else
                        {
                            if (fracEnd == 4)
                            {
                                fracEnd = 2;
                            }
                            else
                            {
                                buffer[3] = '0';
                                buffer[2] = '0';
                            }
                        }

                        fracPart = remainingTicks;
                        buffer[1] = (char)('0' + fracPart);

                        endCount = fracEnd;
                    }

                    handler.WriteChars(buffer, 0, endCount);
                }

                handler.WriteString("S\"");
            }
            else
            {
                if (value.Ticks == long.MinValue)
                {
                    handler.WriteString("\"-10675199.02:48:05.4775808\"");
                    return;
                }
                char[] buffer = new char[36];
                handler.WriteChar('"');

                if (value.Ticks < 0)
                {
                    handler.WriteChar('-');
                    value = value.Negate();
                }

                var days = value.Days;
                var hours = value.Hours;
                var minutes = value.Minutes;
                var secs = value.Seconds;

                TwoDigits digits;

                // days
                {
                    if (days != 0)
                    {
                        PrimitiveNormal.WriteValue(days, handler);
                        handler.WriteChar('.');
                    }
                }

                // hours
                {
                    digits = DigitPairs[hours];
                    buffer[0] = digits.First;
                    buffer[1] = digits.Second;
                }

                buffer[2] = ':';

                // minutes
                {
                    digits = DigitPairs[minutes];
                    buffer[3] = digits.First;
                    buffer[4] = digits.Second;
                }

                buffer[5] = ':';

                // seconds
                {
                    digits = DigitPairs[secs];
                    buffer[6] = digits.First;
                    buffer[7] = digits.Second;
                }

                int endCount = 8;

                // factional part
                {
                    var remainingTicks = (value - new TimeSpan(value.Days, value.Hours, value.Minutes, value.Seconds, 0)).Ticks;
                    if (remainingTicks > 0)
                    {
                        int fracEnd;

                        buffer[8] = '.';

                        var fracPart = remainingTicks % 100;
                        remainingTicks /= 100;
                        if (fracPart > 0)
                        {
                            digits = DigitPairs[fracPart];
                            buffer[15] = digits.Second;
                            buffer[14] = digits.First;
                            fracEnd = 16;
                        }
                        else
                        {
                            fracEnd = 14;
                        }

                        fracPart = remainingTicks % 100;
                        remainingTicks /= 100;
                        if (fracPart > 0)
                        {
                            digits = DigitPairs[fracPart];
                            buffer[13] = digits.Second;
                            buffer[12] = digits.First;
                        }
                        else
                        {
                            if (fracEnd == 14)
                            {
                                fracEnd = 12;
                            }
                            else
                            {
                                buffer[13] = '0';
                                buffer[12] = '0';
                            }
                        }

                        fracPart = remainingTicks % 100;
                        remainingTicks /= 100;
                        if (fracPart > 0)
                        {
                            digits = DigitPairs[fracPart];
                            buffer[11] = digits.Second;
                            buffer[10] = digits.First;
                        }
                        else
                        {
                            if (fracEnd == 12)
                            {
                                fracEnd = 10;
                            }
                            else
                            {
                                buffer[11] = '0';
                                buffer[10] = '0';
                            }
                        }

                        fracPart = remainingTicks;
                        buffer[9] = (char)('0' + fracPart);

                        endCount = fracEnd;
                    }
                }

                handler.WriteChars(buffer, 0, endCount);

                handler.WriteChar('"');
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(Uri value, JsonSerializerHandler handler)
        {
            if (value == null)
                handler.WriteString("null");
            else
                PrimitiveNormal.WriteValue(value.OriginalString, handler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(byte[] value, JsonSerializerHandler handler)
        {
            if (value == null)
                handler.WriteString("null");
            else
            {
                if (handler.Option.IsByteArrayFormatBase64)
                {
                    handler.WriteString("\"");
                    handler.WriteString(Convert.ToBase64String(value));
                    handler.WriteString("\"");
                }
                else
                {
                    handler.WriteString("[");
                    bool isFirst = true;
                    foreach (var obj in value)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            handler.WriteString(",");
                        PrimitiveNormal.WriteValue(obj, handler);
                    }
                    handler.WriteString("]");
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(Guid value, JsonSerializerHandler handler)
        {
            handler.WriteString("\"");
            char[] buffer = new char[36];
            // 1314FAD4-7505-439D-ABD2-DBD89242928C
            // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ
            // 8 4 4 4 12,36 char,64byte
            // guid -> int short short 8byte => 8byte 4byte 4byte 8byte
            // Guid is guaranteed to be a 36 character string

            // get all the dashes in place
            buffer[8] = '-';
            buffer[13] = '-';
            buffer[18] = '-';
            buffer[23] = '-';

            // Bytes are in a different order than you might expect
            // For: 35 91 8b c9 - 19 6d - 40 ea  - 97 79  - 88 9d 79 b7 53 f0 
            // Get: C9 8B 91 35   6D 19   EA 40    97 79    88 9D 79 B7 53 F0 
            // Ix:   0  1  2  3    4  5    6  7     8  9    10 11 12 13 14 15
            //
            // And we have to account for dashes
            //
            // So the map is like so:
            // bytes[0]  -> chars[3]  -> buffer[ 6, 7]
            // bytes[1]  -> chars[2]  -> buffer[ 4, 5]
            // bytes[2]  -> chars[1]  -> buffer[ 2, 3]
            // bytes[3]  -> chars[0]  -> buffer[ 0, 1]
            // bytes[4]  -> chars[5]  -> buffer[11,12]
            // bytes[5]  -> chars[4]  -> buffer[ 9,10]
            // bytes[6]  -> chars[7]  -> buffer[16,17]
            // bytes[7]  -> chars[6]  -> buffer[14,15]
            // bytes[8]  -> chars[8]  -> buffer[19,20]
            // bytes[9]  -> chars[9]  -> buffer[21,22]
            // bytes[10] -> chars[10] -> buffer[24,25]
            // bytes[11] -> chars[11] -> buffer[26,27]
            // bytes[12] -> chars[12] -> buffer[28,29]
            // bytes[13] -> chars[13] -> buffer[30,31]
            // bytes[14] -> chars[14] -> buffer[32,33]
            // bytes[15] -> chars[15] -> buffer[34,35]
            var visibleMembers = new GuidStruct(value);

            // bytes[0]
            var b = visibleMembers.B00 * 2;
            buffer[6] = WriteGuidLookup[b];
            buffer[7] = WriteGuidLookup[b + 1];

            // bytes[1]
            b = visibleMembers.B01 * 2;
            buffer[4] = WriteGuidLookup[b];
            buffer[5] = WriteGuidLookup[b + 1];

            // bytes[2]
            b = visibleMembers.B02 * 2;
            buffer[2] = WriteGuidLookup[b];
            buffer[3] = WriteGuidLookup[b + 1];

            // bytes[3]
            b = visibleMembers.B03 * 2;
            buffer[0] = WriteGuidLookup[b];
            buffer[1] = WriteGuidLookup[b + 1];

            // bytes[4]
            b = visibleMembers.B04 * 2;
            buffer[11] = WriteGuidLookup[b];
            buffer[12] = WriteGuidLookup[b + 1];

            // bytes[5]
            b = visibleMembers.B05 * 2;
            buffer[9] = WriteGuidLookup[b];
            buffer[10] = WriteGuidLookup[b + 1];

            // bytes[6]
            b = visibleMembers.B06 * 2;
            buffer[16] = WriteGuidLookup[b];
            buffer[17] = WriteGuidLookup[b + 1];

            // bytes[7]
            b = visibleMembers.B07 * 2;
            buffer[14] = WriteGuidLookup[b];
            buffer[15] = WriteGuidLookup[b + 1];

            // bytes[8]
            b = visibleMembers.B08 * 2;
            buffer[19] = WriteGuidLookup[b];
            buffer[20] = WriteGuidLookup[b + 1];

            // bytes[9]
            b = visibleMembers.B09 * 2;
            buffer[21] = WriteGuidLookup[b];
            buffer[22] = WriteGuidLookup[b + 1];

            // bytes[10]
            b = visibleMembers.B10 * 2;
            buffer[24] = WriteGuidLookup[b];
            buffer[25] = WriteGuidLookup[b + 1];

            // bytes[11]
            b = visibleMembers.B11 * 2;
            buffer[26] = WriteGuidLookup[b];
            buffer[27] = WriteGuidLookup[b + 1];

            // bytes[12]
            b = visibleMembers.B12 * 2;
            buffer[28] = WriteGuidLookup[b];
            buffer[29] = WriteGuidLookup[b + 1];

            // bytes[13]
            b = visibleMembers.B13 * 2;
            buffer[30] = WriteGuidLookup[b];
            buffer[31] = WriteGuidLookup[b + 1];

            // bytes[14]
            b = visibleMembers.B14 * 2;
            buffer[32] = WriteGuidLookup[b];
            buffer[33] = WriteGuidLookup[b + 1];

            // bytes[15]
            b = visibleMembers.B15 * 2;
            buffer[34] = WriteGuidLookup[b];
            buffer[35] = WriteGuidLookup[b + 1];

            handler.WriteChars(buffer, 0, 36);
            handler.WriteString("\"");

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(ExpandoObject value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.WriteString("null");
                return;
            }

            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
            {
                if (handler.SerializeStacks.Contains(value))
                {
                    if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Null)
                        handler.WriteString("null");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Empty)
                        handler.WriteString("{}");
                    else if (handler.Option.ReferenceLoopHandling == JsonReferenceHandlingEnum.Remove)
                        RemoveWriterHelper.RemoveDictionaryKey(handler);
                    return;
                }
                handler.SerializeStacks.Push(value);
            }

            IDictionary<string, object> keyValuePairs = value;
            handler.WriteString("{");
            bool isFirst = true;
            foreach (var item in value)
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");

                PrimitiveNormal.WriteValue(item.Key, handler);
                handler.WriteString(":");
                var val = item.Value;
                PrimitiveNormal.WriteValue(val, handler);
            }
            handler.WriteString("}");
            if (handler.Option.ReferenceLoopHandling != JsonReferenceHandlingEnum.None)
                handler.SerializeStacks.Pop();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(NameValueCollection value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.WriteString("null");
                return;
            }
            handler.WriteString("{");
            bool isFirst = true;
            foreach (string item in value)
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");
                var name = item;
                PrimitiveNormal.WriteValue(name, handler);
                handler.WriteString(":");
                var val = value[name];
                PrimitiveNormal.WriteValue(val, handler);
            }
            handler.WriteString("}");

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(StringDictionary value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.WriteString("null");
                return;
            }
            handler.WriteString("{");
            bool isFirst = true;
            foreach (DictionaryEntry item in value)
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");
                var name = item.Key;
                PrimitiveNormal.WriteValue(item.Key, handler);
                handler.WriteString(":");
                var val = item.Value;
                PrimitiveNormal.WriteValue(val, handler);
            }
            handler.WriteString("}");

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(DataTable value, JsonSerializerHandler handler)
        {
            if (value == null)
            {
                handler.WriteString("null");
                return;
            }
            handler.WriteString("[");

            bool isFirst = true;
            foreach (DataRow row in value.Rows)
            {
                if (isFirst)
                    isFirst = false;
                else
                    handler.WriteString(",");

                handler.WriteString("{");
                bool isFirst2 = true;
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (isFirst2)
                        isFirst2 = false;
                    else
                        handler.WriteString(",");

                    object columnValue = row[column];
                    handler.WriteString("\"");
                    handler.WriteString(column.ColumnName);//没有检查
                    handler.WriteString("\":");
                    PrimitiveNormal.WriteValue(columnValue, handler);
                }
                handler.WriteString("}");
            }

            handler.WriteString("]");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(DBNull value, JsonSerializerHandler handler)
        {
            handler.WriteString("null");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [FuncLable(FuncType.SameType)]
        internal static void WriteValue(StringBuilder value, JsonSerializerHandler handler)
        {
            if (value == null)
                handler.WriteString("null");
            else
            {
                handler.WriteString("\"");
                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];
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
                    //如果是jsonp格式，多了 u2028 u2029的转换
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
                        default: handler.WriteChar(c); continue;
                    }
                }
                handler.WriteString("\"");
            }
        }

        internal struct TwoDigits
        {
            public readonly char First;
            public readonly char Second;

            public TwoDigits(char first, char second)
            {
                First = first;
                Second = second;
            }
        }
        internal static readonly TwoDigits[] DigitPairs =
        CreateArray(100, i => new TwoDigits((char)('0' + (i / 10)), (char)+('0' + (i % 10))));
        internal static readonly char[] DigitTriplets =
           CreateArray(3 * 1000, i =>
           {
               var ibase = i / 3;
               switch (i % 3)
               {
                   case 0:
                       return (char)('0' + ibase / 100 % 10);
                   case 1:
                       return (char)('0' + ibase / 10 % 10);
                   case 2:
                       return (char)('0' + ibase % 10);
                   default:
                       throw new InvalidOperationException("Unexpectedly reached default case in switch block.");
               }
           });
        internal static T[] CreateArray<T>(int count, Func<int, T> generator)
        {
            var arr = new T[count];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = generator(i);
            }
            return arr;
        }
    }

    internal partial class SpecialTypeNormal
    {
        static readonly char[] WriteGuidLookup = new char[] { '0', '0', '0', '1', '0', '2', '0', '3', '0', '4', '0', '5', '0', '6', '0', '7', '0', '8', '0', '9', '0', 'a', '0', 'b', '0', 'c', '0', 'd', '0', 'e', '0', 'f', '1', '0', '1', '1', '1', '2', '1', '3', '1', '4', '1', '5', '1', '6', '1', '7', '1', '8', '1', '9', '1', 'a', '1', 'b', '1', 'c', '1', 'd', '1', 'e', '1', 'f', '2', '0', '2', '1', '2', '2', '2', '3', '2', '4', '2', '5', '2', '6', '2', '7', '2', '8', '2', '9', '2', 'a', '2', 'b', '2', 'c', '2', 'd', '2', 'e', '2', 'f', '3', '0', '3', '1', '3', '2', '3', '3', '3', '4', '3', '5', '3', '6', '3', '7', '3', '8', '3', '9', '3', 'a', '3', 'b', '3', 'c', '3', 'd', '3', 'e', '3', 'f', '4', '0', '4', '1', '4', '2', '4', '3', '4', '4', '4', '5', '4', '6', '4', '7', '4', '8', '4', '9', '4', 'a', '4', 'b', '4', 'c', '4', 'd', '4', 'e', '4', 'f', '5', '0', '5', '1', '5', '2', '5', '3', '5', '4', '5', '5', '5', '6', '5', '7', '5', '8', '5', '9', '5', 'a', '5', 'b', '5', 'c', '5', 'd', '5', 'e', '5', 'f', '6', '0', '6', '1', '6', '2', '6', '3', '6', '4', '6', '5', '6', '6', '6', '7', '6', '8', '6', '9', '6', 'a', '6', 'b', '6', 'c', '6', 'd', '6', 'e', '6', 'f', '7', '0', '7', '1', '7', '2', '7', '3', '7', '4', '7', '5', '7', '6', '7', '7', '7', '8', '7', '9', '7', 'a', '7', 'b', '7', 'c', '7', 'd', '7', 'e', '7', 'f', '8', '0', '8', '1', '8', '2', '8', '3', '8', '4', '8', '5', '8', '6', '8', '7', '8', '8', '8', '9', '8', 'a', '8', 'b', '8', 'c', '8', 'd', '8', 'e', '8', 'f', '9', '0', '9', '1', '9', '2', '9', '3', '9', '4', '9', '5', '9', '6', '9', '7', '9', '8', '9', '9', '9', 'a', '9', 'b', '9', 'c', '9', 'd', '9', 'e', '9', 'f', 'a', '0', 'a', '1', 'a', '2', 'a', '3', 'a', '4', 'a', '5', 'a', '6', 'a', '7', 'a', '8', 'a', '9', 'a', 'a', 'a', 'b', 'a', 'c', 'a', 'd', 'a', 'e', 'a', 'f', 'b', '0', 'b', '1', 'b', '2', 'b', '3', 'b', '4', 'b', '5', 'b', '6', 'b', '7', 'b', '8', 'b', '9', 'b', 'a', 'b', 'b', 'b', 'c', 'b', 'd', 'b', 'e', 'b', 'f', 'c', '0', 'c', '1', 'c', '2', 'c', '3', 'c', '4', 'c', '5', 'c', '6', 'c', '7', 'c', '8', 'c', '9', 'c', 'a', 'c', 'b', 'c', 'c', 'c', 'd', 'c', 'e', 'c', 'f', 'd', '0', 'd', '1', 'd', '2', 'd', '3', 'd', '4', 'd', '5', 'd', '6', 'd', '7', 'd', '8', 'd', '9', 'd', 'a', 'd', 'b', 'd', 'c', 'd', 'd', 'd', 'e', 'd', 'f', 'e', '0', 'e', '1', 'e', '2', 'e', '3', 'e', '4', 'e', '5', 'e', '6', 'e', '7', 'e', '8', 'e', '9', 'e', 'a', 'e', 'b', 'e', 'c', 'e', 'd', 'e', 'e', 'e', 'f', 'f', '0', 'f', '1', 'f', '2', 'f', '3', 'f', '4', 'f', '5', 'f', '6', 'f', '7', 'f', '8', 'f', '9', 'f', 'a', 'f', 'b', 'f', 'c', 'f', 'd', 'f', 'e', 'f', 'f' };
        static void _CustomWriteIntUnrolledSigned(JsonSerializerHandler handler, int num, char[] buffer)
        {
            if (num == int.MinValue)
            {
                handler.WriteString("-2147483648");
                return;
            }

            int numLen;
            int number;

            if (num < 0)
            {
                handler.WriteChar('-');
                number = -num;
            }
            else
            {
                number = num;
            }

            if (number < 1000)
            {
                if (number >= 100)
                {
                    handler.WriteChars(DigitTriplets, number * 3, 3);
                }
                else
                {
                    if (number >= 10)
                    {
                        handler.WriteChars(DigitTriplets, number * 3 + 1, 2);
                    }
                    else
                    {
                        handler.WriteChars(DigitTriplets, number * 3 + 2, 1);
                    }
                }
                return;
            }
            var d012 = number % 1000 * 3;

            int d543;
            if (number < 1000000)
            {
                d543 = (number / 1000) * 3;
                if (number >= 100000)
                {
                    numLen = 6;
                    goto digit5;
                }
                else
                {
                    if (number >= 10000)
                    {
                        numLen = 5;
                        goto digit4;
                    }
                    else
                    {
                        numLen = 4;
                        goto digit3;
                    }
                }
            }
            d543 = (number / 1000) % 1000 * 3;

            int d876;
            if (number < 1000000000)
            {
                d876 = (number / 1000000) * 3;
                if (number >= 100000000)
                {
                    numLen = 9;
                    goto digit8;
                }
                else
                {
                    if (number >= 10000000)
                    {
                        numLen = 8;
                        goto digit7;
                    }
                    else
                    {
                        numLen = 7;
                        goto digit6;
                    }
                }
            }
            d876 = (number / 1000000) % 1000 * 3;

            numLen = 10;

            // uint is between 0 & 4,294,967,295 (in practice we only get to int.MaxValue, but that's the same # of digits)
            // so 1 to 10 digits

            // [01,]000,000-[99,]000,000
            var d9 = number / 1000000000;
            buffer[0] = (char)('0' + d9);

        digit8:
            buffer[1] = DigitTriplets[d876];
        digit7:
            buffer[2] = DigitTriplets[d876 + 1];
        digit6:
            buffer[3] = DigitTriplets[d876 + 2];

        digit5:
            buffer[4] = DigitTriplets[d543];
        digit4:
            buffer[5] = DigitTriplets[d543 + 1];
        digit3:
            buffer[6] = DigitTriplets[d543 + 2];

            buffer[7] = DigitTriplets[d012];
            buffer[8] = DigitTriplets[d012 + 1];
            buffer[9] = DigitTriplets[d012 + 2];

            handler.WriteChars(buffer, 10 - numLen, numLen);
        }
    }
}
