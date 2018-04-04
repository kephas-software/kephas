// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedDataStoreKindsAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Metadata attribute for supported data store kinds.
    /// </summary>
    public class SupportedDataStoreKindsAttribute : Attribute, IMetadataValue<IEnumerable<string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDataStoreKindsAttribute"/> class.
        /// </summary>
        /// <param name="dataStoreKinds">A variable-length parameters list containing data store kinds.</param>
        public SupportedDataStoreKindsAttribute(params DataStoreKind[] dataStoreKinds)
        {
            Requires.NotNullOrEmpty(dataStoreKinds, nameof(dataStoreKinds));

            this.Value = dataStoreKinds.Select(k => k.ToString()).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDataStoreKindsAttribute"/> class.
        /// </summary>
        /// <param name="dataStoreKinds">A variable-length parameters list containing data store kinds.</param>
        public SupportedDataStoreKindsAttribute(params string[] dataStoreKinds)
        {
            Requires.NotNullOrEmpty(dataStoreKinds, nameof(dataStoreKinds));

            this.Value = dataStoreKinds;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public IEnumerable<string> Value { get; }
    }
}