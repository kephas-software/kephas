// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCoreAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.NetCore
{
    using System.Diagnostics.Contracts;

    using Kephas.Application;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class NetCoreAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET Core application environment to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNetCoreAppEnvironment(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithAppEnvironment(new NetCoreAppEnvironment());
        }
    }
}