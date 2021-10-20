// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Public interface for the injector.
    /// </summary>
    public interface IInjector : IDisposable
    {
        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        object Resolve(Type contractType);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        IEnumerable<object> ResolveMany(Type contractType)
            => (IEnumerable<object>)this.Resolve(
                typeof(IEnumerable<>).MakeGenericType(
                    contractType ??
                        throw new ArgumentNullException(nameof(contractType))));

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        T Resolve<T>()
            where T : class
            => (T)this.Resolve(typeof(T));

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        IEnumerable<T> ResolveMany<T>()
            where T : class =>
            this.Resolve<IEnumerable<T>>();

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the provided contract was not found.</returns>
        object? TryResolve(Type contractType);

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        T? TryResolve<T>()
            where T : class
            => (T?)this.TryResolve(typeof(T));

        /// <summary>
        /// Creates a new scoped injector.
        /// </summary>
        /// <returns>
        /// The new scoped injector.
        /// </returns>
        IInjector CreateScopedInjector();
    }
}