// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetStandard14AmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net45
{
    using System.Diagnostics.Contracts;

    using Kephas.Application;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class NetStandard14AmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET Standard 1.4 application environment to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNetStandard14AppEnvironment(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithAppEnvironment(new NetStandard14AppEnvironment());
        }
    }
}