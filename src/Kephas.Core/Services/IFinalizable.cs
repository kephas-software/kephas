// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFinalizable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFinalizable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides the <see cref="Finalize"/> method for service finalization.
    /// </summary>
    public interface IFinalizable
    {
        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        void Finalize(IContext context = null);
    }
}