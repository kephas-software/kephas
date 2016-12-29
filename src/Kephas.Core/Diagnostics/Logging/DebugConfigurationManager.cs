// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugConfigurationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Configuration manager for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Logging
{
    using Kephas.Configuration;

    /// <summary>
    /// Configuration manager for debugging.
    /// </summary>
    public class DebugConfigurationManager : IConfigurationManager
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