// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionsSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the options settings provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Configuration
{
    using Kephas.Composition;
    using Kephas.Extensions.Configuration.Providers;
    using Kephas.Services;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Settings provider based on the <see cref="IOptions{TOptions}"/> service.
    /// </summary>
    [OverridePriority(Priority.Normal)]
    public class OptionsSettingsProvider : OptionsSettingsProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsSettingsProvider"/> class.
        /// </summary>
        /// <param name="injector">Context for the composition.</param>
        public OptionsSettingsProvider(IInjector injector)
            : base(injector)
        {
        }
    }
}
