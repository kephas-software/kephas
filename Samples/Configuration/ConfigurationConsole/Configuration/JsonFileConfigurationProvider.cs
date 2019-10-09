// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonFileConfigurationProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the JSON file configuration provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ConfigurationConsole.Configuration
{
    using System;
    using System.IO;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// A JSON file configuration provider.
    /// </summary>
    public class JsonFileConfigurationProvider : Loggable, ISettingsProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly ISerializationService serializationService;
        private readonly IContextFactory contextFactory;
        private readonly ISettingsProvider fallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        public JsonFileConfigurationProvider(
            IAppRuntime appRuntime,
            ISerializationService serializationService,
            IContextFactory contextFactory,
            IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            this.appRuntime = appRuntime;
            this.serializationService = serializationService;
            this.contextFactory = contextFactory;
            this.fallback = new AppConfigurationProvider(appConfiguration);
        }

        /// <summary>Gets the settings with the provided type.</summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>The settings.</returns>
        public object GetSettings(Type settingsType)
        {
            var fileName = $"{settingsType.Name.ToCamelCase()}.json";
            var filePath = Path.Combine(this.appRuntime.GetAppLocation(), fileName);
            if (!File.Exists(filePath))
            {
                return this.fallback.GetSettings(settingsType);
            }

            var settingsContent = File.ReadAllText(filePath);

            var serializationContext = this.contextFactory.CreateContext<SerializationContext>(typeof(JsonMediaType));
            serializationContext.RootObjectType = settingsType;

            var settings = this.serializationService.JsonDeserialize(settingsContent, serializationContext);
            return settings;
        }
    }
}