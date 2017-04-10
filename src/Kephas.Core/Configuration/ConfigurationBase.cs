// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the configuration base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// A base for configuration services.
    /// </summary>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    public abstract class ConfigurationBase<TSettings> : Expando, IConfiguration<TSettings>
        where TSettings : class
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private TSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBase{TSettings}"/> class.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        protected ConfigurationBase(IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            this.AppConfiguration = appConfiguration;
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IAppConfiguration AppConfiguration { get; }

        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public TSettings Settings => this.settings ?? (this.settings = (TSettings)this.AppConfiguration.GetSettings(this.GetSettingsPattern(), typeof(TSettings)));

        /// <summary>
        /// Gets the settings pattern.
        /// </summary>
        /// <remarks>
        /// By default, it returns the section having the same name as the settings type name
        /// and the settings having the name starting with the settings type name.
        /// </remarks>
        /// <returns>
        /// The settings pattern.
        /// </returns>
        protected virtual string GetSettingsPattern()
        {
            var settingsTypeName = typeof(TSettings).Name.ToCamelCase();
            return $":{settingsTypeName}:*;{settingsTypeName}*";
        }
    }
}