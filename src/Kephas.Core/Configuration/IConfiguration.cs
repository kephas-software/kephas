// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConfiguration interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using Kephas.Configuration.Providers;
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Shared service contract for getting.
    /// </summary>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    [SharedAppServiceContract(AsOpenGeneric = true)]
    public interface IConfiguration<out TSettings> : IExpando
    {
        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        TSettings Settings { get; }

        /// <summary>
        /// Gets the configuration provider.
        /// </summary>
        /// <value>
        /// The configuration provider.
        /// </value>
        IConfigurationProvider Provider { get; }
    }
}