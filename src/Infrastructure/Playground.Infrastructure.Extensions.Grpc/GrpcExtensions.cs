using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Playground.Infrastructure.Extensions.Grpc
{
    /// <summary>
    /// Extension methods to aid working with Grpc generated types.
    /// </summary>
    public static class GrpcExtensions
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo?> s_parserCache = new ConcurrentDictionary<Type, MethodInfo?>();

        /// <summary>
        /// Convert the <see cref="decimal"/> to a Grpc <see cref="string"/> capable of being reconstituted using <see cref="FromGrpc"/>.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> to convert to.</param>
        /// <returns>Grpc <see cref="string"/> representation of the <see cref="decimal"/>.</returns>
        public static string ToGrpc(this decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert the <see cref="DateTimeOffset"/> to a Grpc <see cref="string"/> capable of being reconstituted using <see cref="FromGrpc"/>.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> to convert to.</param>
        /// <returns>Grpc <see cref="string"/> representation of the <see cref="DateTimeOffset"/>.</returns>
        public static string ToGrpc(this DateTimeOffset value)
        {
            return value.ToString("o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert the Grpc <see cref="string"/> representation to a <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The expected type to convert to</typeparam>
        /// <param name="value">The Grpc <see cref="string"/> representation to convert from</param>
        /// <returns>The <typeparamref name="TValue"/> converted from the Grpc <see cref="string"/></returns>
        /// <exception cref="NotSupportedException"><typeparamref name="TValue"/> does not support a <code>Parse(string s, IFormatProvider? provider)</code> method.</exception>
        [return: MaybeNull]
        public static TValue FromGrpc<TValue>(this string value)
        {
            if (string.IsNullOrEmpty(value))
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter. https://github.com/dotnet/roslyn/pull/39778 
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.

            Type valueType = typeof(TValue);
            MethodInfo? parse = s_parserCache.GetOrAdd(valueType, type =>
                type.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) }));

            if (parse != null)
                return (TValue)parse.Invoke(null, new object[] { value, CultureInfo.InvariantCulture })!;
            else
                throw new NotSupportedException($"Converting to {valueType.Name} from Grpc string is not supported.");
        }
    }
}
