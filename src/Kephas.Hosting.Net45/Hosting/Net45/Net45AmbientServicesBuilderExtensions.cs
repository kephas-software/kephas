// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net45AmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting.Net45
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class Net45AmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET 4.5 hosting environment to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNet45HostingEnvironment(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithHostingEnvironment(new Net45HostingEnvironment(ambientServicesBuilder.AmbientServices.LogManager));
        }
    }
}