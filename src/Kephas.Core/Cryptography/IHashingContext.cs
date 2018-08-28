// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHashingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHashingContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using Kephas.Services;

    /// <summary>
    /// Interface for hashing context.
    /// </summary>
    public interface IHashingContext : IContext
    {
        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        byte[] Salt { get; set; }
    }
}