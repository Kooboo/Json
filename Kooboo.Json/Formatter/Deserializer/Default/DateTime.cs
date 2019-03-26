using System;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Deserialize
{
    internal class DateTimeResolve : DefaultJsonResolve
    {
        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static DateTime ReadDateTime(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            reader.ReadQuotes();
            char* ip = reader.Pointer;
            char c = ip[0];
            if (c >= '0' && c <= '9')
            {
                //https://tools.ietf.org/html/rfc3339
                //http://en.wikipedia.org/wiki/ISO_8601
                //\"2019-02-11T18:42:04.0068385Z\"  
                //\"2019-02-11T18:42:04.0068385+08:00\" 
                return ReadISO8601Date(ref reader);
            }
            else if (c == '\\')
            {
                // "\/Date(628318530718)\/" 
                return ReadMicrosoftDate(ref reader, handler);
            }
            else
            {
                //Mon, 11 Feb 2019 18:39:32 GMT
                return ReadRFC1123DateTime(ref reader);
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe DateTime ReadMicrosoftDate(ref JsonReader reader, JsonDeserializeHandler handler)
        {
            if (!reader.StrCompair("\\/Date("))
                goto Throw;

            long l = PrimitiveResolve.ReadLong(ref reader, handler);
            DateTime dt = new DateTime(l * 10000L + 621355968000000000L);

            if (reader.StrCompair(")\\/\""))
                return dt;
        Throw:
            throw new JsonDeserializationTypeResolutionException( reader, typeof(DateTime), "Unresolvable Date");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe DateTime ReadISO8601Date(ref JsonReader reader)
        {
            // ISO8601 / RFC3339 (the internet "profile"* of ISO8601) is a plague
            //   See: http://en.wikipedia.org/wiki/ISO_8601 &
            //        http://tools.ietf.org/html/rfc3339
            //        *is bullshit

            // Here are the possible formats for dates
            // YYYY-MM-DD
            // YYYY-MM
            // YYYY-DDD (ordinal date)
            // YYYY-Www (week date, the W is a literal)
            // YYYY-Www-D
            // YYYYMMDD
            // YYYYWww
            // YYYYWwwD
            // YYYYDDD

            // Here are the possible formats for times
            // hh
            // hh:mm
            // hhmm
            // hh:mm:ss
            // hhmmss
            // hh,fff*
            // hh:mm,fff*
            // hhmm,fff*
            // hh:mm:ss,fff*
            // hhmmss,fff*
            // hh.fff*
            // hh:mm.fff*
            // hhmm.fff*
            // hh:mm:ss.fff*
            // hhmmss.fff*
            // * arbitrarily many (technically an "agreed upon" number, I'm agreeing on 7 because that's out to a Tick)

            // Here are the possible formats for timezones
            // Z
            // +hh
            // +hh:mm
            // +hhmm
            // -hh
            // -hh:mm
            // -hhmm

            // they are concatenated to form a full instant, with T as a separator between date & time
            // i.e. <date>T<time><timezone>
            // the longest possible string:
            // 9999-12-31T01:23:45.6789012+34:56
            //
            // Maximum date size is 33 characters

            int leng = -1;
            int? tPos = null;
            int? zPlusOrMinus = null;
            char c;

            char* ip = reader.Pointer;

            for (int i = 0; i < reader.Remaining; i++)
            {
                c = ip[i];
                if (c == '"')
                {
                    leng = i - 1;
                    break;
                }

                // RFC3339 allows lowercase t and spaces as alternatives to ISO8601's T
                if (c == 'T' || c == 't' || c == ' ')
                {
                    if (tPos.HasValue)
                        throw new JsonDeserializationTypeResolutionException( reader, typeof(DateTime), "Unexpected second T in ISO8601 date");
                    tPos = i - 1;
                }

                if (tPos.HasValue)
                {
                    // RFC3339 allows lowercase z as alternatives to ISO8601's Z
                    if (c == 'Z' || c == 'z' || c == '+' || c == '-')
                    {
                        if (zPlusOrMinus.HasValue)
                            throw new JsonDeserializationTypeResolutionException( reader, typeof(DateTime), "Unexpected second Z, +, or - in ISO8601 date");
                        zPlusOrMinus = i - 1;
                    }
                }
            }
            if (leng < 1)
                throw new JsonWrongCharacterException( reader);
            if (leng > 32)
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 date is too long");

            reader.Pointer += leng + 2;//1 => idx ,1 => " ==2
            reader.Remaining -= leng + 2;

            var date = ParseISO8601Date(ip, 0, tPos ?? leng); // this is in *LOCAL TIME* because that's what the spec says
            if (!tPos.HasValue)
            {

                return date;

            }

            var time = ParseISO8601Time(ip, tPos.Value + 2, zPlusOrMinus ?? leng);
            if (!zPlusOrMinus.HasValue)
            {
                try
                {
                    return date + time;

                }
                catch
                {
                    throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 date with time could not be represented as a DateTime");
                }
            }

            // only +1 here because the separator is significant (oy vey)
            var timezoneOffset = ParseISO8601TimeZoneOffset(ip, zPlusOrMinus.Value + 1, leng, out bool unknownLocalOffset);

            try
            {

                if (unknownLocalOffset)
                {
                    return DateTime.SpecifyKind(date, DateTimeKind.Unspecified) + time;
                }

                return DateTime.SpecifyKind(date, DateTimeKind.Utc) + time - timezoneOffset;
            }
            catch
            {
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 date with time and timezone offset could not be represented as a DateTime");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe DateTime ParseISO8601Date(char* buffer, int start, int stop)
        {
            // Here are the possible formats for dates
            // YYYY-MM-DD
            // YYYY-MM
            // YYYY-DDD (ordinal date)
            // YYYY-Www (week date, the W is a literal)
            // YYYY-Www-D
            // YYYYMMDD
            // YYYYWww
            // YYYYWwwD
            // YYYYDDD

            bool? hasSeparators = null;

            var len = stop - start + 1;
            if (len < 4)
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 date must begin with a 4 character year");

            var year = 0;
            var month = 0;
            var day = 0;
            int c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            year += c - '0';
            year *= 10;
            start++;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            year += c - '0';
            year *= 10;
            start++;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            year += c - '0';
            year *= 10;
            start++;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c); ;
            year += c - '0';

            if (year == 0) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 year 0000 cannot be converted to a DateTime");

            // we've reached the end
            if (start == stop)
            {
                hasSeparators = null;
                // year is [1,9999] for sure, no need to handle errors
                return new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local);
            }

            start++;
            hasSeparators = buffer[start] == '-';
            var isWeekDate = buffer[start] == 'W';
            if (hasSeparators.Value && start != stop)
            {
                isWeekDate = buffer[start + 1] == 'W';
                if (isWeekDate)
                {
                    start++;
                }
            }

            if (isWeekDate)
            {
                start++;    // skip the W

                var week = 0;

                if (hasSeparators.Value)
                {
                    // Could still be
                    // YYYY-Www         length:  8
                    // YYYY-Www-D       length: 10

                    switch (len)
                    {

                        case 8:
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            week *= 10;
                            start++;
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            if (week == 0 || week > 53) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected week to be between 01 and 53");

                            return ConvertWeekDateToDateTime(year, week, 1);

                        case 10:
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            week *= 10;
                            start++;
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            if (week == 0 || week > 53) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected week to be between 01 and 53");
                            start++;

                            c = buffer[start];
                            if (c != '-') throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
                            start++;

                            c = buffer[start];
                            if (c < '1' || c > '7') throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected day to be a digit between 1 and 7");
                            day = c - '0';

                            return ConvertWeekDateToDateTime(year, week, day);

                        default:
                            throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Unexpected date string length");
                    }
                }
                else
                {
                    // Could still be
                    // YYYYWww          length: 7
                    // YYYYWwwD         length: 8
                    switch (len)
                    {

                        case 7:
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            week *= 10;
                            start++;
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            if (week == 0 || week > 53) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), " Expected week to be between 01 and 53");

                            return ConvertWeekDateToDateTime(year, week, 1);

                        case 8:
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            week *= 10;
                            start++;
                            c = buffer[start];
                            CheckCharFromZeroToNineInDateTime(c);
                            week += c - '0';
                            if (week == 0 || week > 53) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected week to be between 01 and 53");
                            start++;

                            c = buffer[start];
                            if (c < '1' || c > '7') throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected day to be a digit between 1 and 7");
                            day = c - '0';

                            return ConvertWeekDateToDateTime(year, week, day);

                        default:
                            throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Unexpected date string length");
                    }
                }
            }

            if (hasSeparators.Value)
            {
                start++;

                // Could still be:
                // YYYY-MM              length:  7
                // YYYY-DDD             length:  8
                // YYYY-MM-DD           length: 10

                switch (len)
                {
                    case 7:
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        month += c - '0';
                        month *= 10;
                        start++;
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        month += c - '0';
                        if (month == 0 || month > 12) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected month to be between 01 and 12");

                        // year is [1,9999] and month is [1,12] for sure, no need to handle errors
                        return new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Local);

                    case 8:
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        day += c - '0';
                        day *= 10;
                        start++;
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        day += c - '0';
                        day *= 10;
                        start++;
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        day += c - '0';
                        if (day == 0 || day > 366) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected ordinal day to be between 001 and 366");

                        if (day == 366)
                        {
                            var isLeapYear = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

                            if (!isLeapYear) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Ordinal day can only be 366 in a leap year");
                        }

                        // year is [1,9999] and day is [1,366], no need to handle errors
                        return new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(day - 1);

                    case 10:
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        month += c - '0';
                        month *= 10;
                        start++;
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        month += c - '0';
                        if (month == 0 || month > 12) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected month to be between 01 and 12");
                        start++;

                        if (buffer[start] != '-') throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
                        start++;

                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        day += c - '0';
                        day *= 10;
                        start++;
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);
                        day += c - '0';
                        if (day == 0 || day > 31) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected day to be between 01 and 31");
                        start++;

                        try
                        {
                            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local);
                        }
                        catch
                        {
                            throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 date could not be mapped to DateTime");
                        }

                    default:
                        throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Unexpected date string length");
                }
            }

            // Could still be
            // YYYYDDD          length: 7
            // YYYYMMDD         length: 8

            switch (len)
            {
                case 7:
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    day += c - '0';
                    day *= 10;
                    start++;
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    day += c - '0';
                    day *= 10;
                    start++;
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    day += c - '0';
                    if (day == 0 || day > 366) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected ordinal day to be between 001 and 366");
                    start++;

                    if (day == 366)
                    {
                        var isLeapYear = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

                        if (!isLeapYear) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Ordinal day can only be 366 in a leap year");
                    }

                    // year is [1,9999] and day is [1,366], no need to handle errors
                    return new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(day - 1);

                case 8:
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    month += c - '0';
                    month *= 10;
                    start++;
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    month += c - '0';
                    if (month == 0 || month > 12) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected month to be between 01 and 12");
                    start++;

                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    day += c - '0';
                    day *= 10;
                    start++;
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);
                    day += c - '0';
                    if (day == 0 || day > 31) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected day to be between 01 and 31");
                    start++;

                    try
                    {
                        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local);
                    }
                    catch
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 date could not be mapped to DateTime");
                    }

                default:
                    throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Unexpected date string length");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe TimeSpan ParseISO8601Time(char* buffer, int start, int stop)
        {
            const long hoursToTicks = 36000000000;
            const long minutesToTicks = 600000000;
            const long secondsToTicks = 10000000;

            // Here are the possible formats for times
            // hh
            // hh,fff
            // hh.fff
            //
            // hhmmss
            // hhmm
            // hhmm,fff
            // hhmm.fff
            // hhmmss.fff
            // hhmmss,fff
            // hh:mm
            // hh:mm:ss
            // hh:mm,fff
            // hh:mm:ss,fff
            // hh:mm.fff
            // hh:mm:ss.fff

            bool? hasSeparators = null;

            var len = stop - start + 1;
            if (len < 2) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "ISO8601 time must begin with a 2 character hour");

            var hour = 0;
            int c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            hour += c - '0';
            hour *= 10;
            start++;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            hour += c - '0';
            if (hour > 24) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected hour to be between 00 and 24");

            // just an hour part
            if (start == stop)
            {
                return TimeSpan.FromHours(hour);
            }

            start++;
            c = buffer[start];

            // hour with a fractional part
            if (c == ',' || c == '.')
            {
                start++;
                var frac = 0;
                var fracLength = 0;
                while (start <= stop)
                {
                    c = buffer[start];
                    CheckCharFromZeroToNineInDateTime(c);

                    // Max precision of TimeSpan.FromTicks
                    if (fracLength < 9)
                    {
                        frac *= 10;
                        frac += c - '0';
                        fracLength++;
                    }

                    start++;
                }

                if (fracLength == 0) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected fractional part of ISO8601 time");

                long hoursAsTicks = hour * hoursToTicks;
                hoursAsTicks += frac * 36 * TimeSpanResolve.Pow10(9 - fracLength);

                return TimeSpan.FromTicks(hoursAsTicks);
            }

            if (c == ':')
            {
                if (hasSeparators.HasValue && !hasSeparators.Value) throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

                hasSeparators = true;
                start++;
            }
            else
            {
                if (hasSeparators.HasValue && hasSeparators.Value) throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

                hasSeparators = false;
            }

            if (hasSeparators.Value)
            {
                // Could still be
                // hh:mm
                // hh:mm:ss
                // hh:mm,fff
                // hh:mm:ss,fff
                // hh:mm.fff
                // hh:mm:ss.fff

                if (len < 4) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected minute part of ISO8601 time");

                var min = 0;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                min += c - '0';
                min *= 10;
                start++;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                min += c - '0';
                if (min > 59) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected minute to be between 00 and 59");

                // just HOUR and MINUTE part
                if (start == stop)
                {
                    return new TimeSpan(hour, min, 0);
                }

                start++;
                c = buffer[start];

                // HOUR, MINUTE, and FRACTION
                if (c == ',' || c == '.')
                {
                    start++;
                    var frac = 0;
                    var fracLength = 0;
                    while (start <= stop)
                    {
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);

                        // Max precision of TimeSpan.FromTicks
                        if (fracLength < 8)
                        {
                            frac *= 10;
                            frac += c - '0';
                            fracLength++;
                        }

                        start++;
                    }

                    if (fracLength == 0) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected fractional part of ISO8601 time");

                    long hoursAsTicks = hour * hoursToTicks;
                    long minsAsTicks = min * minutesToTicks;
                    minsAsTicks += frac * 6 * TimeSpanResolve.Pow10(8 - fracLength);

                    return TimeSpan.FromTicks(hoursAsTicks + minsAsTicks);
                }

                if (c != ':') throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
                start++;

                var secs = 0;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                secs += c - '0';
                secs *= 10;
                start++;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                secs += c - '0';

                // HOUR, MINUTE, and SECONDS
                if (start == stop)
                {
                    return new TimeSpan(hour, min, secs);
                }

                start++;
                c = buffer[start];
                if (c == ',' || c == '.')
                {
                    start++;
                    var frac = 0;
                    var fracLength = 0;
                    while (start <= stop)
                    {
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);

                        // Max precision of TimeSpan.FromTicks
                        if (fracLength < 7)
                        {
                            frac *= 10;
                            frac += c - '0';
                            fracLength++;
                        }

                        start++;
                    }

                    if (fracLength == 0) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected fractional part of ISO8601 time");

                    long hoursAsTicks = hour * hoursToTicks;
                    long minsAsTicks = min * minutesToTicks;
                    long secsAsTicks = secs * secondsToTicks;
                    secsAsTicks += frac * TimeSpanResolve.Pow10(7 - fracLength);

                    return TimeSpan.FromTicks(hoursAsTicks + minsAsTicks + secsAsTicks);
                }

                throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected ,, or .");
            }
            else
            {
                // Could still be
                // hhmmss
                // hhmm
                // hhmm,fff
                // hhmm.fff
                // hhmmss.fff
                // hhmmss,fff

                if (len < 4) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected minute part of ISO8601 time");

                var min = 0;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                min += c - '0';
                min *= 10;
                start++;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                min += c - '0';
                if (min > 59) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected minute to be between 00 and 59");

                // just HOUR and MINUTE part
                if (start == stop)
                {
                    return new TimeSpan(hour, min, 0);
                }

                start++;
                c = buffer[start];

                // HOUR, MINUTE, and FRACTION
                if (c == ',' || c == '.')
                {
                    start++;
                    var frac = 0;
                    var fracLength = 0;
                    while (start <= stop)
                    {
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);

                        // Max precision of TimeSpan.FromTicks
                        if (fracLength < 8)
                        {
                            frac *= 10;
                            frac += c - '0';
                            fracLength++;
                        }

                        start++;
                    }

                    if (fracLength == 0) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected fractional part of ISO8601 time");

                    long hoursAsTicks = hour * hoursToTicks;
                    long minsAsTicks = min * minutesToTicks;
                    minsAsTicks += frac * 6 * TimeSpanResolve.Pow10(8 - fracLength);

                    return TimeSpan.FromTicks(hoursAsTicks + minsAsTicks);
                }

                if (c == ':') throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Unexpected separator in ISO8601 time");

                var secs = 0;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                secs += c - '0';
                secs *= 10;
                start++;
                c = buffer[start];
                CheckCharFromZeroToNineInDateTime(c);
                secs += c - '0';

                // HOUR, MINUTE, and SECONDS
                if (start == stop)
                {
                    return new TimeSpan(hour, min, secs);
                }

                start++;
                c = buffer[start];
                if (c == ',' || c == '.')
                {
                    start++;
                    var frac = 0;
                    var fracLength = 0;
                    while (start <= stop)
                    {
                        c = buffer[start];
                        CheckCharFromZeroToNineInDateTime(c);

                        // Max precision of TimeSpan.FromTicks
                        if (fracLength < 7)
                        {
                            frac *= 10;
                            frac += c - '0';
                            fracLength++;
                        }

                        start++;
                    }

                    if (fracLength == 0) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected fractional part of ISO8601 time");

                    long hoursAsTicks = hour * hoursToTicks;
                    long minsAsTicks = min * minutesToTicks;
                    long secsAsTicks = secs * secondsToTicks;
                    secsAsTicks += frac * TimeSpanResolve.Pow10(7 - fracLength);

                    return TimeSpan.FromTicks(hoursAsTicks + minsAsTicks + secsAsTicks);
                }

                throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected ,, or .");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe TimeSpan ParseISO8601TimeZoneOffset(char* buffer, int start, int stop, out bool unknownLocalOffset)
        {
            // Here are the possible formats for timezones
            // Z
            // +hh
            // +hh:mm
            // +hhmm
            // -hh
            // -hh:mm
            // -hhmm

            bool? hasSeparators = null;

            int c = buffer[start];
            // no need to validate, the caller has done that
            if (c == 'Z' || c == 'z')
            {
                unknownLocalOffset = false;
                return TimeSpan.Zero;
            }
            var isNegative = c == '-';
            start++;

            var len = stop - start + 1;

            if (len < 2) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected hour part of ISO8601 timezone offset");
            var hour = 0;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            hour += c - '0';
            hour *= 10;
            start++;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            hour += c - '0';
            if (hour > 24) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected hour offset to be between 00 and 24");

            // just an HOUR offset
            if (start == stop)
            {
                unknownLocalOffset = false;

                if (isNegative)
                {
                    return new TimeSpan(-hour, 0, 0);
                }

                return new TimeSpan(hour, 0, 0);
            }

            start++;
            c = buffer[start];
            if (c == ':')
            {
                if (hasSeparators.HasValue && !hasSeparators.Value) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Unexpected separator in ISO8601 timezone offset");

                hasSeparators = true;
                start++;
            }
            else
            {
                if (hasSeparators.HasValue && hasSeparators.Value) throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

                hasSeparators = false;
            }

            if (stop - start + 1 < 2) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Not enough character for ISO8601 timezone offset");

            var mins = 0;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            mins += c - '0';
            mins *= 10;
            start++;
            c = buffer[start];
            CheckCharFromZeroToNineInDateTime(c);
            mins += c - '0';
            if (mins > 59) throw new JsonDeserializationTypeResolutionException(typeof(DateTime), "Expected minute offset to be between 00 and 59");

            if (isNegative)
            {
                // per Section 4.3 of of RFC3339 (http://tools.ietf.org/html/rfc3339)
                // a timezone of "-00:00" is used to indicate an "Unknown Local Offset"
                unknownLocalOffset = hour == 0 && mins == 0;

                return new TimeSpan(-hour, -mins, 0);
            }

            unknownLocalOffset = false;
            return new TimeSpan(hour, mins, 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static DateTime ConvertWeekDateToDateTime(int year, int week, int day)
        {
            // January 4th will always be in week 1
            var ret = new DateTime(year, 1, 4, 0, 0, 0, DateTimeKind.Utc);

            if (week != 1)
            {
                ret += TimeSpan.FromDays(7 * (week - 1));
            }

            int currentDay;
            switch (ret.DayOfWeek)
            {
                case DayOfWeek.Sunday: currentDay = 7; break;
                case DayOfWeek.Monday: currentDay = 1; break;
                case DayOfWeek.Tuesday: currentDay = 2; break;
                case DayOfWeek.Wednesday: currentDay = 3; break;
                case DayOfWeek.Thursday: currentDay = 4; break;
                case DayOfWeek.Friday: currentDay = 5; break;
                case DayOfWeek.Saturday: currentDay = 6; break;
                default: throw new Exception("Unexpected DayOfWeek");
            }

            var offset = day - currentDay;
            ret += TimeSpan.FromDays(offset);

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static DateTime ReadRFC1123DateTime(ref JsonReader reader)
        {
            DayOfWeek dayOfWeek = ReadRFC1123DayOfWeek(ref reader);

            if (!reader.StrCompair(", "))
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            int day = 0;
            char c = reader.GetChar();
            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
            day += c - '0';

            c = reader.GetChar();
            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
            day *= 10;
            day += c - '0';

            c = reader.GetChar();
            if (c != ' ')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            byte month = ReadRFC1123Month(ref reader);

            c = reader.GetChar();
            if (c != ' ')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            var year = 0;
            c = reader.GetChar();
            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
            year += c - '0';

            c = reader.GetChar();
            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
            year *= 10;
            year += c - '0';

            c = reader.GetChar();
            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
            year *= 10;
            year += c - '0';

            c = reader.GetChar();
            if (c < '0' || c > '9')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
            year *= 10;
            year += c - '0';

            c = reader.GetChar();
            if (c != ' ')
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            var hour = 0;
            c = reader.GetChar();
            CheckCharFromZeroToNineInDateTime(c);
            hour += c - '0';
            c = reader.GetChar();
            CheckCharFromZeroToNineInDateTime(c);
            hour *= 10;
            hour += c - '0';

            c = reader.GetChar();
            if (c != ':') throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            var min = 0;
            c = reader.GetChar();
            CheckCharFromZeroToNineInDateTime(c);
            min += c - '0';
            c = reader.GetChar();
            CheckCharFromZeroToNineInDateTime(c);
            min *= 10;
            min += c - '0';

            c = reader.GetChar();
            if (c != ':') throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            var sec = 0;
            c = reader.GetChar();
            CheckCharFromZeroToNineInDateTime(c);
            sec += c - '0';
            c = reader.GetChar();
            CheckCharFromZeroToNineInDateTime(c);
            sec *= 10;
            sec += c - '0';

            if (!reader.StrCompair(" GMT\""))
                throw new JsonDeserializationTypeResolutionException(typeof(DateTime));

            var ret = new DateTime(year, month, day, hour, min, sec, DateTimeKind.Utc);
            return ret;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DayOfWeek ReadRFC1123DayOfWeek(ref JsonReader reader)
        {
            char c = reader.GetChar();
            // Mon
            if (c == 'M')
            {
                if (reader.StrCompair("on"))
                    return DayOfWeek.Monday;
            }

            // Tue | Thu
            if (c == 'T')
            {
                if (reader.StrCompair("ue"))
                    return DayOfWeek.Tuesday;
                if (reader.StrCompair("hu"))
                    return DayOfWeek.Thursday;
            }

            // Wed
            if (c == 'W')
            {
                if (reader.StrCompair("ed"))
                    return DayOfWeek.Wednesday;
            }

            // Fri
            if (c == 'F')
            {
                if (reader.StrCompair("ri"))
                    return DayOfWeek.Friday;
            }

            // Sat | Sun
            if (c == 'S')
            {
                if (reader.StrCompair("at"))
                    return DayOfWeek.Saturday;
                if (reader.StrCompair("un"))
                    return DayOfWeek.Sunday;
            }

            throw new JsonWrongCharacterException(reader, "Kooboo.Json is not defined by the Datetime format");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ReadRFC1123Month(ref JsonReader reader)
        {
            var c = reader.GetChar();
            if (c == -1)
                goto Throw;

            // Jan | Jun | Jul
            if (c == 'J')
            {
                c = reader.GetChar();
                if (c == 'a')
                {
                    c = reader.GetChar();
                    if (c != 'n') goto Throw;

                    return 1;
                }

                if (c != 'u') goto Throw;

                c = reader.GetChar();
                if (c == 'n') return 6;
                if (c == 'l') return 7;

                goto Throw;
            }

            // Feb
            if (c == 'F')
            {
                c = reader.GetChar();
                if (c != 'e') goto Throw;
                c = reader.GetChar();
                if (c != 'b') goto Throw;

                return 2;
            }

            // Mar | May
            if (c == 'M')
            {
                c = reader.GetChar();
                if (c != 'a') goto Throw;

                c = reader.GetChar();
                if (c == 'r') return 3;
                if (c == 'y') return 5;

                goto Throw;
            }

            // Apr | Aug
            if (c == 'A')
            {
                c = reader.GetChar();
                if (c == 'p')
                {
                    c = reader.GetChar();
                    if (c != 'r') goto Throw;

                    return 4;
                }

                if (c == 'u')
                {
                    c = reader.GetChar();
                    if (c != 'g') goto Throw;

                    return 8;
                }

                goto Throw;
            }

            // Sep
            if (c == 'S')
            {
                c = reader.GetChar();
                if (c != 'e') goto Throw;
                c = reader.GetChar();
                if (c != 'p') goto Throw;

                return 9;
            }

            // Oct
            if (c == 'O')
            {
                c = reader.GetChar();
                if (c != 'c') goto Throw;
                c = reader.GetChar();
                if (c != 't') goto Throw;

                return 10;
            }

            // Nov
            if (c == 'N')
            {
                c = reader.GetChar();
                if (c != 'o') goto Throw;
                c = reader.GetChar();
                if (c != 'v') goto Throw;

                return 11;
            }

            // Dec
            if (c == 'D')
            {
                c = reader.GetChar();
                if (c != 'e') goto Throw;
                c = reader.GetChar();
                if (c != 'c') goto Throw;

                return 12;
            }
        Throw:
            throw new JsonDeserializationTypeResolutionException(reader, typeof(DateTime));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CheckCharFromZeroToNineInDateTime(int c)
        {
            if (c < '0' || c > '9') throw new JsonDeserializationTypeResolutionException(typeof(DateTime));
        }
    }
}
