// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullConfigurationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A configuration manager returning no configuration values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using Kephas.Services;

    /// <summary>
    /// A configuration manager returning no configuration values.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullConfigurationManager : IConfigurationManager
    {
        /// <summary>
        /// Gets the setting with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The setting with the provided key.
        /// </returns>
        /// <remarks>
        /// If the setting is not found, returns <c>null</c>.
        /// </remarks>
        public string GetSetting(string key)
        {
            return null;
        }
    }
}