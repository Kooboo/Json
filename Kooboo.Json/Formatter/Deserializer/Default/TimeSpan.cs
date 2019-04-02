using System;
using System.Runtime.CompilerServices;

namespace Kooboo.Json.Deserialize
{
    internal class TimeSpanResolve : DefaultJsonResolve
    {
        [FuncLable(FuncType.SameType)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe TimeSpan ReadTimeSpan( JsonReader reader, JsonDeserializeHandler handler)
        {
            /*
             \"P10675199DT2H48M5.4775807S\"  => ISO
             \"10675199.02:48:05.4775807\"   => Microsoft
             */
            reader.ReadQuotes();
            char* ip = reader.Pointer;
            char c = ip[0];
            if (c == '-')
                c = ip[1];

            if (c == 'P')
                return _ReadISO8601TimeSpan(reader);
            else
                return ReadMicrosoftTimeSpan(reader);
        }

        static readonly ulong MinTicks = (ulong)-TimeSpan.MinValue.Ticks;
        static readonly ulong MaxTicks = (ulong)TimeSpan.MaxValue.Ticks;

        static readonly double[] DivideFractionBy =
          {
                10,
                100,
                1000,
                10000,
                100000,
                1000000,
                10000000,
                100000000
          };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe TimeSpan ReadMicrosoftTimeSpan(JsonReader reader)
        {
            char* ip = reader.Pointer;
            int start = reader.Remaining;
            int strLen = -1;
            for (int z = 0; z < reader.Remaining; z++)
            {
                if (ip[z] == '"')
                {
                    strLen = z;
                    break;
                }
            }

            if (strLen < 1)
                throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan));

            reader.Pointer += strLen + 1;// "
            reader.Remaining -= strLen + 1;// "

            int days, hours, minutes, seconds, fraction;
            days = hours = minutes = seconds = fraction = 0;

            bool isNegative, pastDays, pastHours, pastMinutes, pastSeconds;
            isNegative = pastDays = pastHours = pastMinutes = pastSeconds = false;

            var ixOfLastPeriod = -1;
            var part = 0;

            int i;

            if (ip[0] == '-')
            {
                isNegative = true;
                i = 1;
            }
            else
            {
                i = 0;
            }

            for (; i < strLen; i++)
            {
                var c = ip[i];
                if (c == '.')
                {
                    ixOfLastPeriod = i;

                    if (!pastDays)
                    {
                        days = part;
                        part = 0;
                        pastDays = true;
                        continue;
                    }

                    if (!pastSeconds)
                    {
                        seconds = part;
                        part = 0;
                        pastSeconds = true;
                        continue;
                    }

                    throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan));
                }

                if (c == ':')
                {
                    if (!pastHours)
                    {
                        hours = part;
                        part = 0;
                        pastHours = true;
                        continue;
                    }

                    if (!pastMinutes)
                    {
                        minutes = part;
                        part = 0;
                        pastMinutes = true;
                        continue;
                    }

                    throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan));
                }

                if (c < '0' || c > '9')
                {
                    throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan));
                }

                part *= 10;
                part += c - '0';
            }

            if (!pastSeconds)
            {
                seconds = part;
                pastSeconds = true;
            }
            else
            {
                fraction = part;
            }

            if (!pastHours || !pastMinutes || !pastSeconds)
            {
                throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Missing required portion of TimeSpan");
            }

            var msInt = 0;
            if (fraction != 0)
            {
                var sizeOfFraction = strLen - (ixOfLastPeriod + 1);

                if (sizeOfFraction > 7)
                {
                    throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Fractional part of TimeSpan too large");
                }

                var fracOfSecond = part / DivideFractionBy[sizeOfFraction - 1];
                var ms = fracOfSecond * 1000.0;
                msInt = (int)ms;
                if (ms > msInt)
                    return TimeSpan.Parse(reader.SubString(reader.Length - start, strLen));
            }

            var ret = new TimeSpan(days, hours, minutes, seconds, msInt);
            if (isNegative)
            {
                ret = ret.Negate();
            }

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe TimeSpan _ReadISO8601TimeSpan(JsonReader reader)
        {
            const ulong ticksPerDay = 864000000000;

            const ulong ticksPerWeek = ticksPerDay * 7;
            const ulong ticksPerMonth = ticksPerDay * 30;
            const ulong ticksPerYear = ticksPerDay * 365;

            // Format goes like so:
            // - (-)P(([n]Y)([n]M)([n]D))(T([n]H)([n]M)([n]S))
            // - P[n]W
            char* ip = reader.Pointer;
            int len = -1;
            for (int i = 0; i < reader.Remaining; i++)
            {
                if (ip[i] == '"')
                {
                    len = i;
                    break;
                }
            }

            if (len < 1)
                throw new JsonWrongCharacterException(reader);

            reader.Pointer += len + 1;
            reader.Remaining -= len + 1;

            var ix = 0;
            var isNegative = false;

            var c = ip[ix];
            if (c == '-')
            {
                isNegative = true;
                ix++;
            }

            if (ix >= len)
            {
                throw new JsonWrongCharacterException("Expected P, instead TimeSpan string ended");
            }

            c = ip[ix];
            if (c != 'P')
            {
                throw new JsonWrongCharacterException("Expected P, found " + c.ToString());
            }

            ix++;   // skip 'P'

            var hasTimePart = ISO8601TimeSpan_ReadDatePart(ip, len, ref ix, out long year, out long month, out long week, out long day);

            if (week != -1 && (year != -1 || month != -1 || day != -1))
            {
                throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Week part of TimeSpan defined along with one or more of year, month, or day");
            }

            if (week != -1 && hasTimePart)
            {
                throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "TimeSpans with a week defined cannot also have a time defined");
            }

            if (year == -1) year = 0;
            if (month == -1) month = 0;
            if (week == -1) week = 0;
            if (day == -1) day = 0;

            ulong timeTicks;

            if (hasTimePart)
            {
                ix++;   // skip 'T'
                ISO8601TimeSpan_ReadTimePart(ip, len, ref ix, out timeTicks);
            }
            else
            {
                timeTicks = 0;
            }

            ulong ticks = 0;
            if (year != 0)
            {
                ticks += ((ulong)year) * ticksPerYear;
            }

            if (month != 0)
            {
                // .NET (via XmlConvert) converts months to years
                // This isn't inkeeping with the spec, but of the bad choices... I choose this one
                var yearsFromMonths = ((ulong)month) / 12;
                var monthsAfterYears = ((ulong)month) % 12;
                ticks += yearsFromMonths * ticksPerYear + monthsAfterYears * ticksPerMonth;
            }

            if (week != 0)
            {
                // ISO8601 defines weeks as 7 days, so don't convert weeks to months or years (even if that may seem more sensible)
                ticks += ((ulong)week) * ticksPerWeek;
            }

            ticks += ((ulong)day) * ticksPerDay + timeTicks;

            if (ticks >= MaxTicks && !isNegative)
            {
                return TimeSpan.MaxValue;
            }

            if (ticks >= MinTicks && isNegative)
            {
                return TimeSpan.MinValue;
            }

            var ret = new TimeSpan((long)ticks);
            if (isNegative)
            {
                ret = ret.Negate();
            }



            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool ISO8601TimeSpan_ReadDatePart(char* ip, int strLen, ref int ix, out long year, out long month, out long week, out long day)
        {
            year = month = week = day = -1;

            bool yearSeen, monthSeen, weekSeen, daySeen;
            yearSeen = monthSeen = weekSeen = daySeen = false;

            while (ix != strLen)
            {
                if (ip[ix] == 'T')
                {
                    return true;
                }

                var part = ISO8601TimeSpan_ReadPart(ip, strLen, ref ix, out int whole, out int fraction, out int fracLen);

                if (fracLen != 0)
                {
                    throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Fractional values are not supported in the year, month, day, or week parts of an ISO8601 TimeSpan");
                }

                if (part == 'Y')
                {
                    if (yearSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Year part of TimeSpan seen twice");
                    }

                    if (monthSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Year part of TimeSpan seen after month already parsed");
                    }

                    if (daySeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Year part of TimeSpan seen after day already parsed");
                    }

                    year = whole;
                    yearSeen = true;
                    continue;
                }

                if (part == 'M')
                {
                    if (monthSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Month part of TimeSpan seen twice");
                    }

                    if (daySeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Month part of TimeSpan seen after day already parsed");
                    }

                    month = whole;
                    monthSeen = true;
                    continue;
                }

                if (part == 'W')
                {
                    if (weekSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Week part of TimeSpan seen twice");
                    }

                    week = whole;
                    weekSeen = true;
                    continue;
                }

                if (part == 'D')
                {
                    if (daySeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Day part of TimeSpan seen twice");
                    }

                    day = whole;
                    daySeen = true;
                    continue;
                }

                throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Expected Y, M, W, or D but found: " + part);
            }

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe char ISO8601TimeSpan_ReadPart(char* ip, int strLen, ref int ix, out int whole, out int fraction, out int fracLen)
        {
            var part = 0;
            while (true)
            {
                var c = ip[ix];

                if (c == '.' || c == ',')
                {
                    whole = part;
                    break;
                }

                ix++;
                if (c < '0' || c > '9' || ix == strLen)
                {
                    whole = part;
                    fraction = 0;
                    fracLen = 0;
                    return c;
                }

                part *= 10;
                part += c - '0';
            }

            var ixOfPeriod = ix;

            ix++;   // skip the '.' or ','
            part = 0;
            while (true)
            {
                var c = ip[ix];

                ix++;
                if (c < '0' || c > '9' || ix == strLen)
                {
                    fraction = part;
                    fracLen = ix - 1 - (ixOfPeriod + 1);
                    return c;
                }

                part *= 10;
                part += c - '0';
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void ISO8601TimeSpan_ReadTimePart(char* str, int strLen, ref int ix, out ulong ticks)
        {
            const ulong ticksPerHour = 36000000000;
            const ulong ticksPerMinute = 600000000;
            const ulong ticksPerSecond = 10000000;

            ticks = 0;

            bool hourSeen, minutesSeen, secondsSeen;
            hourSeen = minutesSeen = secondsSeen = false;

            var fracSeen = false;

            while (ix != strLen)
            {
                if (fracSeen)
                {
                    throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Expected Time part of TimeSpan to end");
                }

                char part = ISO8601TimeSpan_ReadPart(str, strLen, ref ix, out int whole, out int fraction, out int fracLen);

                if (fracLen != 0)
                {
                    fracSeen = true;
                }

                if (part == 'H')
                {
                    if (hourSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Hour part of TimeSpan seen twice");
                    }

                    if (minutesSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Hour part of TimeSpan seen after minutes already parsed");
                    }

                    if (secondsSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Hour part of TimeSpan seen after seconds already parsed");
                    }

                    ticks += (ulong)whole * ticksPerHour + ISO8601TimeSpan_FractionToTicks(9, fraction * 36, fracLen);
                    hourSeen = true;
                    continue;
                }

                if (part == 'M')
                {
                    if (minutesSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Minute part of TimeSpan seen twice");
                    }

                    if (secondsSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Minute part of TimeSpan seen after seconds already parsed");
                    }

                    ticks += (ulong)whole * ticksPerMinute + ISO8601TimeSpan_FractionToTicks(8, fraction * 6, fracLen);
                    minutesSeen = true;
                    continue;
                }

                if (part == 'S')
                {
                    if (secondsSeen)
                    {
                        throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Seconds part of TimeSpan seen twice");
                    }

                    ticks += (ulong)whole * ticksPerSecond + ISO8601TimeSpan_FractionToTicks(7, fraction, fracLen);
                    secondsSeen = true;
                    continue;
                }

                throw new JsonDeserializationTypeResolutionException(typeof(TimeSpan), "Expected H, M, or S but found: " + part);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong ISO8601TimeSpan_FractionToTicks(int maxLen, int fraction, int fracLen)
        {
            if (fracLen == 0)
            {
                return 0;
            }

            if (fracLen > maxLen)
            {
                fraction /= (int)Pow10(fracLen - maxLen);
                fracLen = maxLen;
            }

            return (ulong)(fraction * Pow10(maxLen - fracLen));
        }
        static readonly long[] PowersOf10 = {
            1L,
            10L,
            100L,
            1000L,
            10000L,
            100000L,
            1000000L,
            10000000L,
            100000000L
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Pow10(int power)
        {
            if (power < PowersOf10.Length)
                return PowersOf10[power];
            return (long)Math.Pow(10, power);
        }
    }
}
