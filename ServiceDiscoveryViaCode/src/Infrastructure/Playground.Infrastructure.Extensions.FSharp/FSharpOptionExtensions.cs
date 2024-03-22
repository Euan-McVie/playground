using System;
using Microsoft.FSharp.Core;

namespace Playground.Infrastructure.Extensions.FSharp
{
    /// <summary>
    /// Extension methods to aid interop with an F# <see cref="FSharpOption{T}"/>.
    /// </summary>
    public static class FSharpOptionExtensions
    {
        /// <summary>
        /// Converts a <see cref="Nullable{T}"/> to a <see cref="FSharpOption{T}"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The <see cref="Nullable{T}"/> value to convert to a <see cref="FSharpOption{T}"/> option.</param>
        /// <returns>A <see cref="FSharpOption{T}"/> wrapping the <typeparamref name="TValue"/> value.</returns>
        public static FSharpOption<TValue> ToFSharpOption<TValue>(this TValue? value)
            where TValue : struct
            => OptionModule.OfNullable(value);

        /// <summary>
        /// Wraps a <see langword="class"/> value with a <see cref="FSharpOption{T}"/>. A <see langword="null"/> value is mapped to <see cref="FSharpOption{T}.None"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The <see langword="class"/> value to wrap in the <see cref="FSharpOption{T}"/>.</param>
        /// <returns>A <see cref="FSharpOption{T}.Some(T)"/> containing the value.</returns>
        public static FSharpOption<TValue> ToFSharpOption<TValue>(this TValue value)
            where TValue : class
            => OptionModule.OfObj(value);

        /// <summary>
        /// Converts a <see cref="FSharpOption{T}"/> to a <see cref="Nullable{T}"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="option">The <see cref="FSharpOption{T}"/> option to convert to a <see cref="Nullable{T}"/> value.</param>
        /// <returns>A <see cref="Nullable{T}"/> vwrapping the <typeparamref name="TValue"/> value.</returns>
        public static TValue? ToNullable<TValue>(this FSharpOption<TValue> option)
            where TValue : struct
            => OptionModule.ToNullable(option);
    }
}
