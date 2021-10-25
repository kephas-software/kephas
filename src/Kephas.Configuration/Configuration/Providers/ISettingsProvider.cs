// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConfigurationProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for configuration provider.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface ISettingsProvider
    {
        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        object? GetSettings(Type settingsType, IContext? context);

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <typeparam name="T">Type of the settings.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        T? GetSettings<T>(IContext? context) =>
            (T?)this.GetSettings(typeof(T), context);

        /// <summary>
        /// Updates the settings asynchronously.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateSettingsAsync(object settings, IContext? context, CancellationToken cancellationToken = default);
    }
}