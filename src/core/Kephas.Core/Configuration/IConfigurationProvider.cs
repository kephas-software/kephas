// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a contract for providing component configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using Kephas.Services;

    /// <summary>
    /// Defines a contract for providing component configuration.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the configuration.</typeparam>
    [SharedAppServiceContract]
    public interface IConfigurationProvider<out TConfiguration>
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="throwOnError">If set to <c>true</c> throws an exception on error, otherwise not.</param>
        /// <returns>The configuration of the requested type.</returns>
        TConfiguration GetConfiguration(bool throwOnError = true);
    }
}