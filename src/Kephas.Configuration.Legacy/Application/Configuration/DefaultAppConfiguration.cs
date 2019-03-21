// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default application configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;

    using Kephas.Dynamic;

    /// <summary>
    /// Configuration based on the AppSettings found in the app.config/web.config file.
    /// </summary>
    /// <remarks>
    /// The application configuration based on the legacy ConfigurationManager is not recommended for new applications based on the .NET Core.
    /// Use instead the new configuration components from the Microsoft.Extensions.Configuration nuget package.
    /// See https://docs.microsoft.com/ro-ro/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration for more information.
    /// </remarks>
    public class DefaultAppConfiguration : ExpandoBase, IAppConfiguration
    {
        /// <summary>
        /// The sections.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> sections;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppConfiguration"/> class.
        /// </summary>
        public DefaultAppConfiguration()
            : this(new ConcurrentDictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppConfiguration"/> class.
        /// </summary>
        /// <param name="sections">The sections.</param>
        private DefaultAppConfiguration(ConcurrentDictionary<string, object> sections)
            : base(sections)
        {
            this.sections = sections;
        }

        /// <summary>
        /// Attempts to get the dynamic value with the given key.
        /// </summary>
        /// <remarks>
        /// First of all, it is tried to get a property value from the inner object, if one is set.
        /// The next try is to retrieve the property value from the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is searched by the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to get.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected override bool TryGetValue(string key, out object value)
        {
            value = this.GetSection(key);
            return true;
        }

        /// <summary>
        /// Gets a section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>
        /// The section.
        /// </returns>
        protected virtual object GetSection(string sectionName)
        {
            var settings = this.sections.GetOrAdd(
                sectionName,
                _ =>
                    {
                        var section = sectionName == AppConfigurationSections.AppSettings
                                          ? ConfigurationManager.AppSettings
                                          : ConfigurationManager.GetSection(sectionName);

                        if (section == null)
                        {
                            return null;
                        }

                        if (section is NameValueCollection nameValueCollection)
                        {
                            return new Expando(nameValueCollection.AllKeys.ToDictionary(k => k, k => (object)nameValueCollection.Get(k)));
                        }

                        return section;
                    });

            return settings;
        }
    }
}
