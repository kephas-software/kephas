// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutdownSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the shutdown signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// Signal for terminating the application. This class cannot be inherited.
    /// </summary>
    public sealed class ShutdownSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownSignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">Optional. The severity.</param>
        public ShutdownSignal(string? message = null, SeverityLevel severity = SeverityLevel.Info)
            : base(message ?? "Signalling an application shutdown.")
        {
        }
    }
}
