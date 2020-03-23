using System;

namespace Playground.Infrastructure.Domain.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="Nullable{T}"/> <see cref="DateTime"/> to a <see cref="Nullable{T}"/> <see cref="DateTimeOffset"/> at UTC.
        /// </summary>
        /// <param name="dateTime">The <see cref="Nullable{T}"/> <see cref="DateTime"/> to convert.</param>
        /// <returns>A <see cref="Nullable{T}"/> <see cref="DateTimeOffset"/> at UTC.</returns>
        public static DateTimeOffset? ToUtcDateTimeOffset(this DateTime? dateTime)
            => dateTime.HasValue ? new DateTimeOffset(dateTime.Value.ToUniversalTime(), TimeSpan.Zero) : default;
    }
}
