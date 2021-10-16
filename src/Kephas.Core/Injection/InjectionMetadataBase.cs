// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionMetadataBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for export metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Data.Formatting;
    using Kephas.Dynamic;

    /// <summary>
    /// Base class for export metadata.
    /// </summary>
    public abstract class InjectionMetadataBase : IDynamic, IDataFormattable
    {
        private IDictionary<string, object?>? metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionMetadataBase"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        protected InjectionMetadataBase(IDictionary<string, object?>? metadata)
        {
            this.metadata = metadata;
        }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Members collection AND the strongly typed
        /// members of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key identifying the member name.</param>
        /// <returns>The requested member value.</returns>
        public object? this[string key]
        {
            get => this.metadata?.TryGetValue(key);
            set => (this.metadata ??= new Dictionary<string, object?>())[key] = value;
        }

        /// <summary>
        /// Converts this object to a serialization friendly representation.
        /// </summary>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        public object? ToData(IDataFormattingContext? context = null) => this.metadata;
    }
}