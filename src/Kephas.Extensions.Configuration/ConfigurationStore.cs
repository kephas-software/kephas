// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetConfigurationStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net configuration store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Configuration
{
    using System.ComponentModel;

    using Kephas.Configuration;
    using Kephas.Dynamic;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// A configuration store based on the extensions configuration.
    /// </summary>
    public class ConfigurationStore : ConfigurationStoreBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStore"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ConfigurationStore(IConfiguration configuration)
            : base(new ConfigurationAdapter(configuration))
        {
        }

        private class ConfigurationAdapter : IIndexable
        {
            public ConfigurationAdapter(IConfiguration configuration)
            {
                this.Configuration = configuration;
            }

            public object this[string key]
            {
                get => this.Configuration[key];
                set => this.SetFlattenedValue(key, value);
            }

            public IConfiguration Configuration { get; }

            private void SetFlattenedValue(string key, object value)
            {
                if (value == null)
                {
                    this.Configuration[key] = null;
                    return;
                }

                if (value is string stringValue)
                {
                    this.Configuration[key] = stringValue;
                    return;
                }

                var valueType = value.GetType();
                var typeConverter = TypeDescriptor.GetConverter(valueType);
                if (typeConverter.CanConvertTo(typeof(string)))
                {
                    this.Configuration[key] = typeConverter.ConvertToString(value);
                    return;
                }

                var flattenedValue = new Expando(value).ToDictionary();
                foreach (var keyValue in flattenedValue)
                {
                    this.SetFlattenedValue($"{key}:{keyValue.Key}", keyValue.Value);
                }
            }
        }
    }
}