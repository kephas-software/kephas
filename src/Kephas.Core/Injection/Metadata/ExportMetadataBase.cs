// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportMetadataBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for export metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Metadata
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Base class for export metadata.
    /// </summary>
    public abstract class ExportMetadataBase : Expando
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportMetadataBase"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        protected ExportMetadataBase(IDictionary<string, object?>? metadata)
            : base(metadata ?? new Dictionary<string, object?>())
        {
        }
    }
}