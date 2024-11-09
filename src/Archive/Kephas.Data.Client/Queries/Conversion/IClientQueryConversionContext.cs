// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryConversionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IClientQueryConversionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    /// <summary>
    /// Context for a client query conversion.
    /// </summary>
    public interface IClientQueryConversionContext : IDataOperationContext
    {
        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        object? Options { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object uses the member access convention.
        /// </summary>
        /// <remarks>
        /// The member access convention considers all strings starting with . (dot) as member access expressions.
        /// </remarks>
        /// <value>
        /// True if the member access convention should be used, false if not.
        /// </value>
        bool UseMemberAccessConvention { get; set; }
    }
}