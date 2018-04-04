// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataHandlerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data handler metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization.Composition
{
    using System.Collections.Generic;

    using Kephas.Data.Initialization.AttributedModel;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for initial data handler services.
    /// </summary>
    public class InitialDataHandlerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialDataHandlerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public InitialDataHandlerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.InitialDataKind = this.GetMetadataValue<InitialDataKindAttribute, string>(metadata);
        }

        /// <summary>
        /// Gets the data kind.
        /// </summary>
        /// <value>
        /// The data kind.
        /// </value>
        public string InitialDataKind { get; }
    }
}