// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestartSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Interaction;

    /// <summary>
    /// Signal for restarting the application. This class cannot be inherited.
    /// </summary>
    public sealed class RestartSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestartSignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RestartSignal(string? message = null)
            : base(message ?? "Signalled an application restart.")
        {
        }
    }
}