// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Lite.Conventions;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    internal static class AmbientServicesExtensions
    {
        private const string AppServiceInfosKey = "__" + nameof(AppServiceInfosKey);

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

            return ambientServices[AppServiceInfosKey] as
                       IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)>;
        }

        /// <summary>
        /// Gets the registered application service contracts.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appServiceInfos">An enumeration of key-value pairs, where the key is the <see cref="T:TypeInfo"/> and the
        /// value is the <see cref="IAppServiceInfo"/></param>
        internal static void SetAppServiceInfos(this IAmbientServices ambientServices, IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> appServiceInfos)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            // Lite composition container exclude its own services, so add them now.
            // CAUTION: this assumes that the app service infos from the other registration sources
            // did not add them already, so after that do not call SetAppServiceInfos!
            if ((bool?)ambientServices[LiteConventionsBuilder.LiteCompositionKey] ?? false)
            {
                var liteServiceInfos = (ambientServices as IAppServiceInfoProvider)?.GetAppServiceInfos(null, null);
                var allServiceInfos = new List<(Type contractType, IAppServiceInfo appServiceInfo)>();
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
    }
}