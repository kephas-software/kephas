// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetStandardAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.NetStandard
{
    using System.Diagnostics.Contracts;
    using Kephas.Application;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class NetStandardAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET Standard 1.4 application environment to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNetStandardAppEnvironment(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithAppEnvironment(new NetStandardAppEnvironment());
        }
    }
}