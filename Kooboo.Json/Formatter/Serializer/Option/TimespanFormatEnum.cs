namespace Kooboo.Json
{
    /// <summary>
    ///     Timespan格式化枚举
    ///     Timespan Format Enumeration
    /// </summary>
    public enum TimespanFormatEnum
    {
        /// <summary>
        /// TimeSpans will be formatted as ISO8601 durations.
        /// Examples: P123DT11H30M2.3S
        /// </summary>
        ISO8601,
        /// <summary>
        /// TimeSpans will be formatted as "days.hours:minutes:seconds.fractionalSeconds"
        /// </summary>
        Microsoft
    }
}