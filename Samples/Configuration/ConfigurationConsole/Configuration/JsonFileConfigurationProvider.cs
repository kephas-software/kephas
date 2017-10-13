// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonFileConfigurationProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A JSON file configuration provider.
    /// </summary>
    public class JsonFileConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// The application runtime.
        /// </summary>
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// The serialization service.
        /// </summary>
        private readonly ISerializationService serializationService;

        /// <summary>
        /// The fallback.
        /// </summary>
        private readonly IConfigurationProvider fallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        public JsonFileConfigurationProvider(
            IAppRuntime appRuntime,
            ISerializationService serializationService,
            IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            this.appRuntime = appRuntime;
            this.serializationService = serializationService;
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

            var serializationContext = new SerializationContext(this.serializationService, typeof(JsonMediaType))
            {
                RootObjectType = settingsType,
            };

            var settings = this.serializationService.JsonDeserializeAsync(settingsContent, serializationContext)
                .GetResultNonLocking(TimeSpan.FromSeconds(30));

            return settings;
        }
    }
}