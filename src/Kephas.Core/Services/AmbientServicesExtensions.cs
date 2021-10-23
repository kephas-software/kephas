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

    using Kephas.Injection.Lite.Builder;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    internal static class AmbientServicesExtensions
    {
        /// <summary>
        /// Gets the registered application service contracts.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appServiceInfos">An enumeration of key-value pairs, where the key is the <see cref="T:TypeInfo"/> and the
        /// value is the <see cref="IAppServiceInfo"/>.</param>
        internal static void SetAppServiceInfos(this IAmbientServices ambientServices, IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> appServiceInfos)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            // Lite injector exclude its own services, so add them now.
            // CAUTION: this assumes that the app service infos from the other registration sources
            // did not add them already, so after that do not call SetAppServiceInfos!
            if ((bool?)ambientServices[LiteInjectorBuilder.LiteInjectionKey] ?? false)
            {
                var liteServiceInfos = (ambientServices as IAppServiceInfosProvider)?.GetAppServiceInfos(null);
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

            ambientServices[InjectionAmbientServicesExtensions.AppServiceInfosKey] = appServiceInfos;
        }
    }
}