// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsPlatformAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting.WindowsPlatform
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class WindowsPlatformAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the universal windows hosting environment to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithWindowsPlatformHostingEnvironment(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithHostingEnvironment(new WindowsPlatformHostingEnvironment(ambientServicesBuilder.AmbientServices.LogManager));
        }
    }
}