// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAppSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Server.Extensions
{
    using System;
    using System.Collections.Generic;
    using Kephas.Application;
    using Kephas.Application.AspNetCore.Configuration;
    using Kephas.Configuration.Providers;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// A web settings provider.
    /// Override the <see cref="OptionsSettingsProvider"/>.
    /// </summary>
    [OverridePriority(Priority.AboveNormal)]
    public class WebAppSettingsProvider : FileSettingsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaTypes">List of types of the medias.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public WebAppSettingsProvider(
            IAppRuntime appRuntime,
            ISerializationService serializationService,
            ICollection<Lazy<IMediaType, MediaTypeMetadata>> mediaTypes,
            ILogManager? logManager = null)
            : base(appRuntime, serializationService, mediaTypes, logManager)
        {
        }
    }
}
