// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsProviderSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISettingsProviderSelector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for the singleton settings provider selector.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ISettingsProviderSelector
    {
        /// <summary>
        /// Tries to get the provider handling a specific settings type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The provider or <c>null</c>.
        /// </returns>
        ISettingsProvider TryGetProvider(Type settingsType);
    }
}
