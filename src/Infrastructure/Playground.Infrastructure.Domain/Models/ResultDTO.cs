using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Playground.Infrastructure.Domain.Models
{
    /// <summary>
    /// Represents a result containing either <typeparamref name="TSuccess"/> success data or <typeparamref name="TError"/> error data.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success data.</typeparam>
    /// <typeparam name="TError">the type of the error data.</typeparam>
    [DataContract]
    public class ResultDTO<TSuccess, TError>
    {
        private readonly bool _isSuccessful;
        [AllowNull]
        private readonly TSuccess _successData;
        [AllowNull]
        private readonly TError _errorData;

        internal ResultDTO(bool isSuccessful, [AllowNull]TSuccess successData, [AllowNull]TError errorData)
        {
            _isSuccessful = isSuccessful;
            _successData = successData;
            _errorData = errorData;
        }

        /// <summary>
        /// A <see cref="ValueTuple"/> consisting of an <c>IsSuccessful</c> flag and the <typeparamref name="TSuccess"/> success data.
        /// If the <c>IsSuccessful</c> flag is false then the success data will be the default for <typeparamref name="TSuccess"/>.
        /// </summary>
        public (bool IsSuccessful, TSuccess SuccessData) Successful => (_isSuccessful, _successData);

        /// <summary>
        /// A <see cref="ValueTuple"/> consisting of an <c>IsError</c> flag and the <typeparamref name="TError"/> error data.
        /// If the <c>IsError</c> flag is false then the error data will be the default for <typeparamref name="TError"/>.
        /// </summary>
        public (bool IsError, TError ErrorData) Error => (!_isSuccessful, _errorData);

        /// <summary>
        /// Allows deconstruction of the <see cref="ResultDTO{TSuccess, TError}"/> to enable pattern matching.
        /// </summary>
        /// <param name="IsSuccessful"></param>
        /// <param name="SuccessData"></param>
        /// <param name="ErrorData"></param>
        public void Deconstruct(out bool IsSuccessful, out TSuccess SuccessData, out TError ErrorData)
        {
            IsSuccessful = _isSuccessful;
            SuccessData = _successData;
            ErrorData = _errorData;
        }
    }

    /// <summary>
    /// Factory methods to create instances of a <see cref="ResultDTO{TSuccess, TError}"/>.
    /// </summary>
    public static class ResultDTO
    {
        /// <summary>
        /// Creates a new success result of <see cref="ResultDTO{TSuccess, TError}"/> containing the provided <typeparamref name="TSuccess"/> success data.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success data.</typeparam>
        /// <typeparam name="TError">the type of the error data.</typeparam>
        /// <param name="successData">The <typeparamref name="TSuccess"/> success data.</param>
        /// <returns>A new success result of <see cref="ResultDTO{TSuccess, TError}"/>.</returns>
        public static ResultDTO<TSuccess, TError> NewSuccessResult<TSuccess, TError>(TSuccess successData)
            => new ResultDTO<TSuccess, TError>(true, successData, default);

        /// <summary>
        /// Creates a new success result of <see cref="ResultDTO{TSuccess, TError}"/> containing the provided <typeparamref name="TSuccess"/> success data
        /// and with a <c>TError</c> of type <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success data.</typeparam>
        /// <param name="successData">The <typeparamref name="TSuccess"/> success data.</param>
        /// <returns>A new success result of <see cref="ResultDTO{TSuccess, TError}"/>.</returns>
        public static ResultDTO<TSuccess, string> NewSuccessResult<TSuccess>(TSuccess successData)
            => new ResultDTO<TSuccess, string>(true, successData, default);

        /// <summary>
        /// Creates a new error result of <see cref="ResultDTO{TSuccess, TError}"/> containing the provided <typeparamref name="TError"/> error data.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success data.</typeparam>
        /// <typeparam name="TError">the type of the error data.</typeparam>
        /// <param name="errorData">The <typeparamref name="TError"/> error data.</param>
        /// <returns>A new error result of <see cref="ResultDTO{TSuccess, TError}"/>.</returns>
        public static ResultDTO<TSuccess, TError> NewErrorResult<TSuccess, TError>(TError errorData)
            => new ResultDTO<TSuccess, TError>(false, default, errorData);

        /// <summary>
        /// Creates a new error result of <see cref="ResultDTO{TSuccess, TError}"/> containing the provided <see cref="string"/> error message.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success data.</typeparam>
        /// <param name="errorData">The <see cref="string"/> error message.</param>
        /// <returns>A new error result of <see cref="ResultDTO{TSuccess, TError}"/> with a <c>TError</c> of type <see cref="string"/>.</returns>
        public static ResultDTO<TSuccess, string> NewErrorResult<TSuccess>(string errorData)
            => new ResultDTO<TSuccess, string>(false, default, errorData);
    }
}
