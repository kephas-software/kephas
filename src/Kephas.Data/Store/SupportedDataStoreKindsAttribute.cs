// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedDataStoreKindsAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the supported data store kinds attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Injection;

    /// <summary>
    /// Metadata attribute for supported data store kinds.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SupportedDataStoreKindsAttribute : Attribute, IMetadataValue<IEnumerable<string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDataStoreKindsAttribute"/> class.
        /// </summary>
        /// <param name="dataStoreKinds">A variable-length parameters list containing data store kinds.</param>
        public SupportedDataStoreKindsAttribute(params DataStoreKind[] dataStoreKinds)
        {
            if (dataStoreKinds == null || dataStoreKinds.Length == 0) throw new System.ArgumentException("Value must not be null or empty.", nameof(dataStoreKinds));

            this.Value = dataStoreKinds.Select(k => k.ToString()).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDataStoreKindsAttribute"/> class.
        /// </summary>
        /// <param name="dataStoreKinds">A variable-length parameters list containing data store kinds.</param>
        public SupportedDataStoreKindsAttribute(params string[] dataStoreKinds)
        {
            if (dataStoreKinds == null || dataStoreKinds.Length == 0) throw new System.ArgumentException("Value must not be null or empty.", nameof(dataStoreKinds));

            this.Value = dataStoreKinds;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public IEnumerable<string> Value { get; }
    }
}