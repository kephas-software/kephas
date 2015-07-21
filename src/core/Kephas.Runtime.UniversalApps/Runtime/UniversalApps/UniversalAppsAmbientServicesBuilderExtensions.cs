// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniversalAppsAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.UniversalApps
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class UniversalAppsAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the universal apps platform manager to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithUniversalAppsPlatformManager(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithPlatformManager(new UniversalAppsPlatformManager(ambientServicesBuilder.AmbientServices.LogManager));
        }
    }
}