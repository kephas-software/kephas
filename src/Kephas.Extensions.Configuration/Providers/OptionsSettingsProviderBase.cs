﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionsSettingsProviderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net configuration provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Configuration.Providers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Configuration.Providers;
    using Kephas.Services;
    using Kephas.Logging;
    using Kephas.Services;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Settings provider based on the <see cref="IOptions{TOptions}"/> service.
    /// </summary>
    public abstract class OptionsSettingsProviderBase : ISettingsProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Lazy<ISettingsProvider> lazyFileSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsSettingsProviderBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        protected OptionsSettingsProviderBase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            this.lazyFileSettingsProvider = new Lazy<ISettingsProvider>(this.CreateFileSettingsProvider);
        }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public virtual object? GetSettings(Type settingsType, IContext? context)
        {
            var options = this.serviceProvider.TryResolve(typeof(IOptions<>).MakeGenericType(settingsType));
            return options?.GetPropertyValue(nameof(IOptions<NullLogManager>.Value));
        }

        /// <summary>
        /// Updates the settings asynchronously.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task UpdateSettingsAsync(object settings, IContext? context, CancellationToken cancellationToken = default)
        {
            return this.lazyFileSettingsProvider.Value.UpdateSettingsAsync(settings, context, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="FileSettingsProvider"/> instance.
        /// </summary>
        /// <returns>The newly created <see cref="FileSettingsProvider"/> instance.</returns>
        protected virtual ISettingsProvider CreateFileSettingsProvider()
        {
            return this.serviceProvider.Resolve<ISettingsProvider>(FileSettingsProvider.ServiceName);
        }
    }
}