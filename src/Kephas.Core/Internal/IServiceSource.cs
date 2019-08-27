// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceSource interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
{
    using System;

    /// <summary>
    /// Interface for service source.
    /// </summary>
    internal interface IServiceSource : IServiceProvider
    {
        /// <summary>
        /// Query if the contract type matches the source.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// True if match, false if not.
        /// </returns>
        bool IsMatch(Type contractType);
    }
}