// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Lite.Conventions;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    internal static class AmbientServicesExtensions
    {
        private const string AppServiceInfoProvidersKey = "__" + nameof(AppServiceInfoProvidersKey);
        private const string AppServiceTypesProvidersKey = "__" + nameof(AppServiceTypesProvidersKey);
        private const string AppServiceInfosKey = "__" + nameof(AppServiceInfosKey);

        /// <summary>
        /// Gets the ordered application service type providers.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceTypesProvider"/>.
        /// </returns>
        internal static IEnumerable<IAppServiceTypesProvider> GetAppServiceTypesProviders(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices[AppServiceTypesProvidersKey] as IEnumerable<IAppServiceTypesProvider>
                   ?? (IEnumerable<IAppServiceTypesProvider>)(ambientServices[AppServiceTypesProvidersKey] = ComputeAppServiceTypesProviders(ambientServices));
        }

        /// <summary>
        /// Gets the ordered application service info providers.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfoProvider"/>.
        /// </returns>
        internal static IEnumerable<IAppServiceInfoProvider> GetAppServiceInfoProviders(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices[AppServiceInfoProvidersKey] as IEnumerable<IAppServiceInfoProvider>
                   ?? (IEnumerable<IAppServiceInfoProvider>)(ambientServices[AppServiceInfoProvidersKey] = ComputeAppServiceInfoProviders(ambientServices));
        }

        /// <summary>
        /// Gets the registered application service contracts.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// An enumeration of key-value pairs, where the key is the <see cref="T:TypeInfo"/> and the
        /// value is the <see cref="IAppServiceInfo"/>.
        /// </returns>
        internal static IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices[AppServiceInfosKey] as IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)>
                ?? Array.Empty<(Type contractType, IAppServiceInfo appServiceInfo)>();
        }

        /// <summary>
        /// Gets the registered application service contracts.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appServiceInfos">An enumeration of key-value pairs, where the key is the <see cref="T:TypeInfo"/> and the
        /// value is the <see cref="IAppServiceInfo"/>.</param>
        internal static void SetAppServiceInfos(this IAmbientServices ambientServices, IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> appServiceInfos)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            // Lite composition container exclude its own services, so add them now.
            // CAUTION: this assumes that the app service infos from the other registration sources
            // did not add them already, so after that do not call SetAppServiceInfos!
            if ((bool?)ambientServices[LiteConventionsBuilder.LiteCompositionKey] ?? false)
            {
                var liteServiceInfos = (ambientServices as IAppServiceInfoProvider)?.GetAppServiceInfos(null);
                var allServiceInfos = new List<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>();
                if (liteServiceInfos != null)
                {
                    allServiceInfos.AddRange(liteServiceInfos);
                }

                if (appServiceInfos != null)
                {
                    allServiceInfos.AddRange(appServiceInfos);
                }

                appServiceInfos = allServiceInfos;
            }

            ambientServices[AppServiceInfosKey] = appServiceInfos;
        }

        /// <summary>
        /// Computes the <see cref="IAppServiceInfoProvider"/> services in order of their priority.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>An enumeration of <see cref="IAppServiceInfoProvider"/>.</returns>
        internal static IEnumerable<IAppServiceInfoProvider> ComputeAppServiceInfoProviders(IAmbientServices ambientServices)
        {
            return ambientServices.AppRuntime.GetAppAssemblies()
                .SelectMany(a => a.GetCustomAttributes().OfType<IAppServiceInfoProvider>())
                .OrderBy(p => (p as IHasProcessingPriority)?.ProcessingPriority ?? Priority.Normal)
                .ToList();
        }

        /// <summary>
        /// Computes the <see cref="IAppServiceTypesProvider"/> services in order of their priority.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>An enumeration of <see cref="IAppServiceTypesProvider"/>.</returns>
        internal static IEnumerable<IAppServiceTypesProvider> ComputeAppServiceTypesProviders(IAmbientServices ambientServices)
        {
            return ambientServices.AppRuntime.GetAppAssemblies()
                .SelectMany(a => a.GetCustomAttributes().OfType<IAppServiceTypesProvider>())
                .OrderBy(p => (p as IHasProcessingPriority)?.ProcessingPriority ?? Priority.Normal)
                .ToList();
        }
    }
}