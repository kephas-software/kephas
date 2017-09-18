// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}