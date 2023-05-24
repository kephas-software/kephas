// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Resources;

    /// <summary>
    /// Extension methods for <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TContract"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TContract"/>.
        /// </returns>
        public static TContract? GetService<TContract>(this IServiceProvider serviceProvider)
            where TContract : class
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            return (TContract?)serviceProvider.GetService(typeof(TContract));
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// A service object of type <paramref name="contractType"/>.
        /// </returns>
        public static object GetRequiredService(this IServiceProvider serviceProvider, Type contractType)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            var service = serviceProvider.GetService(contractType);
            if (service == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        AbstractionStrings.AmbientServices_RequiredServiceNotRegistered_Exception,
                        contractType));
            }

            return service;
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TContract"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TContract"/>.
        /// </returns>
        [return: NotNull]
        public static TContract GetRequiredService<TContract>(this IServiceProvider serviceProvider)
            where TContract : class =>
            (TContract)GetRequiredService(serviceProvider, typeof(TContract));
    }
}