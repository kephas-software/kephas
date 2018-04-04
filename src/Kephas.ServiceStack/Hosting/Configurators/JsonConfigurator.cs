// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting.Configurators
{
    using Kephas.Logging;
    using Kephas.Serialization.ServiceStack.Text;

    /// <summary>
    /// A JSON support configurator.
    /// </summary>
    public class JsonConfigurator : IHostConfigurator
    {
        /// <summary>
        /// The JSON serializer configurator.
        /// </summary>
        private readonly IJsonSerializerConfigurator jsonSerializerConfigurator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfigurator"/> class.
        /// </summary>
        /// <param name="jsonSerializerConfigurator">The JSON serializer configurator.</param>
        public JsonConfigurator(IJsonSerializerConfigurator jsonSerializerConfigurator)
        {
            this.jsonSerializerConfigurator = jsonSerializerConfigurator;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<JsonConfigurator> Logger { get; set; }

        /// <summary>
        /// Configure application host.
        /// </summary>
        /// <param name="configContext">Context for the configuration.</param>
        public void Configure(IHostConfigurationContext configContext)
        {
            configContext.Host.Config.AllowFileExtensions.Add("json");
            this.jsonSerializerConfigurator.ConfigureJsonSerialization();
        }
    }
}