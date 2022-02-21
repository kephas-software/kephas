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
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Kephas.Resources;
    using Kephas.Services;

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
        object Resolve([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        IEnumerable<object> ResolveMany([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType)
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
        T Resolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
            where T : class
            => (T)this.Resolve(typeof(T));

        /// <summary>
        /// Tries to resolve the service with the provided name.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>
        /// The named service.
        /// </returns>
        public T? TryResolve<T>(string serviceName)
        {
            var exportFactories = this
                .ResolveMany<Lazy<T, AppServiceMetadata>>()
                .Order()
                .Where(f => f.Metadata.ServiceName == serviceName)
                .ToList();
            if (exportFactories.Count == 0)
            {
                return default;
            }

            if (exportFactories.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(AbstractionStrings.DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception, serviceName, typeof(T)));
            }

            return exportFactories[0].Value;
        }

        /// <summary>
        /// Resolves the service with the provided name.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>
        /// The named service.
        /// </returns>
        [return: NotNull]
        public T Resolve<T>(string serviceName)
        {
            var service = this.TryResolve<T>(serviceName);
            if (service is null)
            {
                throw new ServiceException(string.Format(AbstractionStrings.DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception, serviceName, typeof(T)));
            }

            return service;
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        IEnumerable<T> ResolveMany<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
            where T : class =>
            this.Resolve<IEnumerable<T>>();

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the provided contract was not found.</returns>
        object? TryResolve([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType);

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        T? TryResolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
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