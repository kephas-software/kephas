// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using Kephas.Services;

    /// <summary>
    /// Provides the application settings for the executing instance.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IAppSettingsProvider
    {
        /// <summary>
        /// Gets the application settings for the executing instance.
        /// </summary>
        /// <returns>The application settings.</returns>
        AppSettings GetAppSettings();
    }
}