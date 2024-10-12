// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMetadataBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for export metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Metadata
{
    /// <summary>
    /// Base class for service metadata.
    /// </summary>
    public abstract class ServiceMetadataBase : Dictionary<string, object?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMetadataBase"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        protected ServiceMetadataBase(IDictionary<string, object?>? metadata)
            : base(metadata ?? new Dictionary<string, object?>())
        {
        }
    }
}
