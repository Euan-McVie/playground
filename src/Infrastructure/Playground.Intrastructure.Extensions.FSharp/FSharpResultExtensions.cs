using System;
using Microsoft.FSharp.Core;

namespace Playground.Intrastructure.Extensions.FSharp
{
    /// <summary>
    /// Extension methods to aid interop with an F# <see cref="FSharpResult{T, TError}"/>.
    /// </summary>
    public static class FSharpResultExtensions
    {
        /// <summary>
        /// Converts a compile-time unknown <see cref="FSharpResult{T, TError}"/> into an <c>FSharpResult&lt;TResult, string&gt;</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result you expect.</typeparam>
        /// <param name="result">An unknown instance of a <see cref="FSharpResult{T, TError}"/></param>
        /// <returns>An <c>FSharpResult&lt;TResult, string&gt;</c> where <typeparamref name="TResult"/> is your expected result type and the error is a <see cref="string"/></returns>
        /// <exception cref="ArgumentNullException">The result is null.</exception>
        /// <exception cref="NotSupportedException">The result is not one of the supported conversions.</exception>
        public static FSharpResult<TResult, string> ResultFromFSharp<TResult>(this object result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            switch (result)
            {
                case FSharpResult<TResult, string> typedResult: return typedResult;
                case FSharpResult<TResult, object> typedResult:
                    if (typedResult.IsOk)
                        return FSharpResult<TResult, string>.NewOk(typedResult.ResultValue);
                    else
                        throw new NotSupportedException($"Error value is not a 'string' it is a '{typedResult.ErrorValue.GetType().Name}'.");
                case FSharpResult<object, string> typedResult:
                    if (typedResult.IsError)
                        return FSharpResult<TResult, string>.NewError(typedResult.ErrorValue);
                    else
                        throw new NotSupportedException($"Result value is not a '{typeof(TResult).Name}' it is a '{typedResult.ErrorValue.GetType().Name}'.");
                default:
                    throw new NotSupportedException($"Unknown result type '{result.GetType().Name}'.");
            }
        }
    }
}
