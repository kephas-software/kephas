// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportMetadataBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for export metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Metadata
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class for export metadata.
    /// </summary>
    public abstract class ExportMetadataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportMetadataBase"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        protected ExportMetadataBase(IDictionary<string, object> metadata)
        {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public IDictionary<string, object> Metadata { get; private set; }
    }
}