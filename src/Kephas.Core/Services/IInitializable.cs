// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInitializable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides the <see cref="Initialize"/> method for service initialization.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void Initialize(IContext context = null);
    }
}