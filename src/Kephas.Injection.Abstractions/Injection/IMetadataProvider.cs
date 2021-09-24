// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetadataProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides metadata for injection.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Gets the metadata as an enumeration of (name, value) pairs.
        /// </summary>
        /// <returns>An enumeration of (name, value) pairs.</returns>
        IEnumerable<(string name, object? value)> GetMetadata();
    }
}