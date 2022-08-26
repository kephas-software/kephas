// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyExpando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lazy expando class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Expando class for evaluating the internal values on demand.
    /// </summary>
    public class LazyExpando : Expando<object?>
    {
        /// <summary>
        /// The lock dictionary.
        /// </summary>
        private readonly IDictionary<string, object> lockDictionary = new Dictionary<string, object>();

        /// <summary>
        /// The inner dictionary.
        /// </summary>
        private readonly IDictionary<string, object?> innerDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyExpando"/> class.
        /// </summary>
        /// <param name="valueResolver">The value resolver (optional).</param>
        public LazyExpando(Func<string, object?>? valueResolver = null)
            : this(new Dictionary<string, object?>(), valueResolver)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyExpando"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="valueResolver">The value resolver (optional).</param>
        public LazyExpando(IDictionary<string, object?> dictionary, Func<string, object?>? valueResolver = null)
            : base(dictionary)
        {
            dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            this.innerDictionary = dictionary;
            this.ValueResolver = valueResolver;
        }

        /// <summary>
        /// Gets or sets the value resolver.
        /// </summary>
        /// <value>
        /// The value resolver.
        /// </value>
        protected Func<string, object?>? ValueResolver { get; set; }

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
        protected override bool TryGetValue(string key, out object? value)
        {
            if (base.TryGetValue(key, out value))
            {
                return true;
            }

            var valueResolver = this.ValueResolver;
            if (valueResolver != null)
            {
                if (this.lockDictionary.ContainsKey(key))
                {
                    return this.HandleCircularDependency(key, out value);
                }

                try
                {
                    this.lockDictionary[key] = true;
                    value = valueResolver(key);
                    this.innerDictionary[key] = value;
                }
                finally
                {
                    this.lockDictionary.Remove(key);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the circular dependency described by requested key.
        /// </summary>
        /// <param name="key">The key involved in the circular dependency.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected virtual bool HandleCircularDependency(string key, out object? value)
        {
            throw new CircularDependencyException($"Circular dependency among values involving '{key}'.");
        }
    }
}