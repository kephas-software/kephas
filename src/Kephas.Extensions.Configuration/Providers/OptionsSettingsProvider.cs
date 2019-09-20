// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetConfigurationProvider.cs" company="Kephas Software SRL">
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

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Configuration.Providers;
    using Kephas.Services;

    using Microsoft.Extensions.Options;

    /// <summary>
    /// Settings provider based on the <see cref="IOptions{TOptions}"/> service.
    /// </summary>
    [ProcessingPriority(Priority.BelowNormal)]
    public class OptionsSettingsProvider : ISettingsProvider
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsSettingsProvider"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public OptionsSettingsProvider(ICompositionContext compositionContext)
        {
            this.compositionContext = compositionContext;
        }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public object GetSettings(Type settingsType)
        {
            var options = this.compositionContext.GetExport(typeof(IOptions<>).MakeGenericType(settingsType));
            return options.GetPropertyValue(nameof(IOptions<AppArgs>.Value));
        }
    }
}