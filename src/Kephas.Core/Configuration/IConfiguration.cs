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
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Singleton service contract for getting.
    /// </summary>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    [SingletonAppServiceContract(AsOpenGeneric = true)]
    public interface IConfiguration<out TSettings> : IExpando
        where TSettings : class, new()
    {
        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        TSettings Settings { get; }
    }
}