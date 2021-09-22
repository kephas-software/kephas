// --------------------------------------------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Services;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Settings provider based on the <see cref="IOptions{TOptions}"/> service.
    /// </summary>
    public abstract class OptionsSettingsProviderBase : ISettingsProvider
    {
        private readonly IInjector injector;
        private readonly Lazy<FileSettingsProvider> lazyFileSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsSettingsProviderBase"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        protected OptionsSettingsProviderBase(IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            this.injector = injector;
            this.lazyFileSettingsProvider = new Lazy<FileSettingsProvider>(this.CreateFileSettingsProvider);
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
            var options = this.injector.TryResolve(typeof(IOptions<>).MakeGenericType(settingsType));
            return options?.GetPropertyValue(nameof(IOptions<CoreSettings>.Value));
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
        protected virtual FileSettingsProvider CreateFileSettingsProvider()
        {
            return new FileSettingsProvider(
                this.injector.Resolve<IAppRuntime>(),
                this.injector.Resolve<ISerializationService>(),
                this.injector.Resolve<ICollection<Lazy<IMediaType, MediaTypeMetadata>>>(),
                this.injector.Resolve<ILogManager>());
        }
    }
}