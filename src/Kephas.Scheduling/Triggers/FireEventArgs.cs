// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FireEventArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Triggers
{
    using System;

    using Kephas.Operations;

    /// <summary>
    /// Event arguments for the <see cref="ITrigger.Fire"/> event.
    /// </summary>
    public class FireEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireEventArgs"/> class.
        /// </summary>
        /// <param name="completeCallback">Optional. The callback on completion.</param>
        public FireEventArgs(Action<IOperationResult>? completeCallback = null)
        {
            this.CompleteCallback = completeCallback;
        }

        /// <summary>
        /// Gets the callback on completion.
        /// </summary>
        public Action<IOperationResult>? CompleteCallback { get; }
    }
}