// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedDataStoreKindsAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the supported data store kinds attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Composition.Metadata;

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
            Contract.Requires(dataStoreKinds != null);
            Contract.Requires(dataStoreKinds.Length > 0);

            this.Value = dataStoreKinds.Select(k => k.ToString()).ToArray();
        }

        object IMetadataValue.Value => this.Value;

        public IEnumerable<string> Value { get; }
    }
}