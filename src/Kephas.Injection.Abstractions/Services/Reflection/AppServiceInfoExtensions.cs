// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extension methods for <see cref="IAppServiceInfo"/>.
    /// </summary>
    public static class AppServiceInfoExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the application service is shared.
        /// </summary>
        /// <param name="appServiceInfo">The application service contract information.</param>
        /// <returns>
        /// <c>true</c> if shared; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleton(this IAppServiceInfo appServiceInfo)
        {
            appServiceInfo = appServiceInfo ?? throw new ArgumentNullException(nameof(appServiceInfo));

            return appServiceInfo.Lifetime == AppServiceLifetime.Singleton;
        }

        /// <summary>
        /// Gets a value indicating whether the application service is shared within a scope.
        /// </summary>
        /// <param name="appServiceInfo">The application service contract information.</param>
        /// <returns>
        /// <c>true</c> if shared within a scope; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsScoped(this IAppServiceInfo appServiceInfo)
        {
            appServiceInfo = appServiceInfo ?? throw new ArgumentNullException(nameof(appServiceInfo));

            return appServiceInfo.Lifetime == AppServiceLifetime.Scoped;
        }

        /// <summary>
        /// Gets a value indicating whether the application service is instanced per request.
        /// </summary>
        /// <param name="appServiceInfo">The application service contract information.</param>
        /// <returns>
        /// <c>true</c> if instanced per request; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransient(this IAppServiceInfo appServiceInfo)
        {
            appServiceInfo = appServiceInfo ?? throw new ArgumentNullException(nameof(appServiceInfo));

            return appServiceInfo.Lifetime == AppServiceLifetime.Transient;
        }
    }
}