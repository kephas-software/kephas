// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextConfigurationProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataContextConfigurationProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// Provider for data context configurations.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataContextConfigurationProvider
    {
        /// <summary>
        /// Tries to get the configuration for the provided data context discriminator.
        /// </summary>
        /// <param name="dataStoreName">The data store name.</param>
        /// <returns>
        /// An <see cref="IDataContextConfiguration"/>.
        /// </returns>
        IDataContextConfiguration TryGetConfiguration(string dataStoreName);
    }
}