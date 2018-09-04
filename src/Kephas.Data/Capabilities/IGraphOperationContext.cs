// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IGraphOperationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// Contract for graph operations contexts.
    /// </summary>
    public interface IGraphOperationContext : IDataOperationContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether the loose parts should be loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if loose parts should be loaded, <c>false</c> if not.
        /// </value>
        bool LoadLooseParts { get; set; }
    }
}