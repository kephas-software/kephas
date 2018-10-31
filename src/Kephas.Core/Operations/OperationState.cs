// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation state class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    /// <summary>
    /// The operation state.
    /// </summary>
    public enum OperationState
    {
        /// <summary>
        /// The operation is not started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The operation is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// The operation is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The operation completed.
        /// </summary>
        Completed,

        /// <summary>
        /// The operation was canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// The operation failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The operation timed out.
        /// </summary>
        TimedOut,
    }
}