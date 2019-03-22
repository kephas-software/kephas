// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetConfigurationProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net configuration provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Configuration.Providers
{
    using System;

    using Kephas.Composition;
    using Kephas.Configuration.Providers;
    using Kephas.Services;

    using Microsoft.Extensions.Options;

    /// <summary>
    /// An ASP net configuration provider.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class AspNetConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetConfigurationProvider"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public AspNetConfigurationProvider(ICompositionContext compositionContext)
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
            return this.compositionContext.GetExport(typeof(IOptions<>).MakeGenericType(settingsType));
        }
    }
}