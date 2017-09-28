// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;

    using Kephas.Dynamic;

    /// <summary>
    /// Configuration based on the AppSettings found in the app.config/web.config file.
    /// </summary>
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
                        var section = sectionName == AppSettingsSections.Default
                                          ? ConfigurationManager.AppSettings
                                          : ConfigurationManager.GetSection(sectionName);

                        if (section == null)
                        {
                            return null;
                        }

                        if (section is NameValueCollection nameValueCollection)
                        {
                            return new NameValueCollectionExpando(nameValueCollection);
                        }

                        return section;
                    });

            return settings;
        }

        /// <summary>
        /// Expando wrapper for a <see cref="NameValueCollection"/>.
        /// </summary>
        internal class NameValueCollectionExpando : ExpandoBase
        {
            /// <summary>
            /// Collection of name values.
            /// </summary>
            private readonly NameValueCollection nameValueCollection;

            /// <summary>
            /// Initializes a new instance of the <see cref="NameValueCollectionExpando"/> class.
            /// </summary>
            /// <param name="nameValueCollection">Collection of name values.</param>
            public NameValueCollectionExpando(NameValueCollection nameValueCollection)
            {
                this.nameValueCollection = nameValueCollection;
            }

            /// <summary>
            /// Returns the enumeration of all dynamic member names.
            /// </summary>
            /// <returns>
            /// A sequence that contains dynamic member names.
            /// </returns>
            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return this.nameValueCollection.AllKeys;
            }

            /// <summary>
            /// Converts the expando to a dictionary having as keys the property names and as values the respective properties' values.
            /// </summary>
            /// <returns>
            /// A dictionary of property values with their associated names.
            /// </returns>
            public override IDictionary<string, object> ToDictionary()
            {
                return this.nameValueCollection.AllKeys.ToDictionary(k => k, k => (object)this.nameValueCollection.Get(k));
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
                value = this.nameValueCollection[key];
                return true;
            }
        }
    }
}