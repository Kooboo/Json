 
namespace Kooboo.Json
{
    /// <summary>
    /// Datetime格式化枚举
    /// Datetime Format Enumeration
    /// </summary>
    public enum DatetimeFormatEnum
    {
        /// <summary>
        /// DateTimes will be formatted as "yyyy-MM-ddThh:mm:ssZ" where
        /// yyyy is the year, MM is the month (starting at 01), dd is the day (starting at 01),
        /// hh is the hour (starting at 00, continuing to 24), mm is the minute (start at 00),
        /// and ss is the second (starting at 00).
        /// 
        /// Examples:
        ///     2011-07-14T19:43:37Z
        ///     2012-01-02T03:04:05Z
        /// </summary>
        ISO8601,
        /// <summary>
        /// DateTimes will be formatted as "ddd, dd MMM yyyy HH:mm:ss GMT" where
        /// ddd is the abbreviation of a day, dd is the day (starting at 01), MMM is the abbreviation of a month,
        /// yyyy is the year, HH is the hour (starting at 00, continuing to 24), mm is the minute (start at 00),
        /// and ss is the second (starting at 00), and GMT is a literal indicating the timezone (always GMT).
        /// 
        /// Examples:
        ///     Thu, 10 Apr 2008 13:30:00 GMT
        ///     Tue, 10 Mar 2015 00:14:34 GMT
        /// </summary>
        RFC1123,
        /// <summary>
        /// DateTimes will be formatted as "\/Date(##...##)\/" where ##...## is the 
        /// number of milliseconds since the unix epoch (January 1st, 1970 UTC).
        /// See: https://msdn.microsoft.com/en-us/library/bb299886.aspx
        /// 
        /// Example:
        ///     "\/Date(628318530718)\/"
        ///     
        /// TimeSpans will be formatted as "days.hours:minutes:seconds.fractionalSeconds"
        /// </summary>
        Microsoft
    }
}