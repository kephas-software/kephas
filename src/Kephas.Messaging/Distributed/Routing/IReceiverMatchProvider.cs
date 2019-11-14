// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReceiverMatchProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IReceiverMatchProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using Kephas.Services;

    /// <summary>
    /// Interface for receiver match provider.
    /// </summary>
    /// <remarks>
    /// The receiver match providers may have constructors importing application services.
    /// </remarks>
    public interface IReceiverMatchProvider
    {
        /// <summary>
        /// Gets the receiver match expression.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The receiver match expression.
        /// </returns>
        string GetReceiverMatch(IContext context = null);
    }
}
