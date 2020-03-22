using System;

namespace Playground.Infrastructure.Domain.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Converts a <see cref="Nullable{T}"/> <see cref="DateTimeOffset"/> to a <see cref="Nullable{T}"/> <see cref="DateTime"/> at UTC.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="Nullable{T}"/> <see cref="DateTimeOffset"/> to convert.</param>
        /// <returns>A <see cref="Nullable{T}"/> <see cref="DateTime"/> at UTC.</returns>
        public static DateTime? ToUtcDateTime(this DateTimeOffset? dateTimeOffset)
            => dateTimeOffset.HasValue ? dateTimeOffset.Value.UtcDateTime : default;
    }
}
