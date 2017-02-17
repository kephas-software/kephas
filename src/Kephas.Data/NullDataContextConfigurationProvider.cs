// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullDataContextConfigurationProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null data context configuration provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// A null data context configuration provider.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullDataContextConfigurationProvider : IDataContextConfigurationProvider
    {
        /// <summary>
        /// Tries to get the configuration for the provided data context discriminator.
        /// </summary>
        /// <param name="dataStoreName">The data context discriminator.</param>
        /// <returns>
        /// An <see cref="IDataContextConfiguration"/>
        /// .
        /// </returns>
        public IDataContextConfiguration TryGetConfiguration(string dataStoreName)
        {
            return null;
        }
    }
}