// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesktopAppsAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.DesktopApps
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class DesktopAppsAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the desktop apps platform manager to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithDesktopAppsPlatform(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithPlatformManager(new DesktopAppsPlatformManager(ambientServicesBuilder.AmbientServices.LogManager));
        }
    }
}