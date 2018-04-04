// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAppConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A configuration returning no configuration values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// A dynamic application configuration returning for each requested section an <see cref="Expando"/> object.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class DynamicAppConfiguration : Expando, IAppConfiguration
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly IDictionary<string, object> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAppConfiguration"/> class.
        /// </summary>
        public DynamicAppConfiguration()
            : this(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAppConfiguration"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DynamicAppConfiguration(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <summary>Attempts to get the dynamic value with the given key.</summary>
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
            if (!base.TryGetValue(key, out value))
            {
                value = new Expando();
                this.dictionary[key] = value;
            }

            return true;
        }
    }
}