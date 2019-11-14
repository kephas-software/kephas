// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransitionState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITransitionState interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitions
{
    using System;

    /// <summary>
    /// Provides information about a transition's state.
    /// </summary>
    public interface ITransitionState
    {
        /// <summary>
        /// Gets a value indicating whether the transition is not started.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is not started; otherwise, <c>false</c>.
        /// </value>
        bool IsNotStarted { get; }

        /// <summary>
        /// Gets a value indicating whether the transition is in progress.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is in progress; otherwise, <c>false</c>.
        /// </value>
        bool IsInProgress { get; }

        /// <summary>
        /// Gets a value indicating whether the transition is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is completed; otherwise, <c>false</c>.
        /// </value>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets a value indicating whether the transition is completed succcessfully.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition  is completed succcessfully; otherwise, <c>false</c>.
        /// </value>
        bool IsCompletedSuccessfully { get; }

        /// <summary>
        /// Gets a value indicating whether the transition is faulted.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is faulted; otherwise, <c>false</c>.
        /// </value>
        bool IsFaulted { get; }

        /// <summary>
        /// Gets the exception in the case the transition failed.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; }
    }
}