// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    /// <summary>
    /// Provides access to type information.
    /// </summary>
    public interface ITypeRegistry : IExpando
    {
        /// <summary>
        /// Gets the type information based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <returns>The type information.</returns>
        ITypeInfo? GetTypeInfo(object typeToken, bool throwOnNotFound = true);

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Gets the type information asynchronously based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the type information.</returns>
        Task<ITypeInfo?> GetTypeInfoAsync(object typeToken, bool throwOnNotFound = true, CancellationToken cancellationToken = default)
            => Task.FromResult(this.GetTypeInfo(typeToken, throwOnNotFound));
#endif
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Extension methods for <see cref="ITypeRegistry"/>.
    /// </summary>
    public static class TypeRegistryExtensions
    {
        /// <summary>
        /// Gets the type information asynchronously based on the type token.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the type information.</returns>
        public static Task<ITypeInfo?> GetTypeInfoAsync(this ITypeRegistry typeRegistry, object typeToken, bool throwOnNotFound = true, CancellationToken cancellationToken = default)
            => Task.FromResult(typeRegistry.GetTypeInfo(typeToken, throwOnNotFound));
    }
#endif
}
