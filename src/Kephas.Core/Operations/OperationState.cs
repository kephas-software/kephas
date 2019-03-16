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
    using System;

    /// <summary>
    /// The operation state.
    /// </summary>
    [Flags]
    public enum OperationState
    {
        /// <summary>
        /// The operation is not yet started.
        /// </summary>
        NotStarted = 0x0000,

        /// <summary>
        /// The operation started and is in progress.
        /// </summary>
        InProgress = 0x0001,

        /// <summary>
        /// The operation is paused.
        /// </summary>
        Paused = 0x0002,

        /// <summary>
        /// The operation completed.
        /// </summary>
        Completed = 0x0010,

        /// <summary>
        /// The operation failed.
        /// </summary>
        Failed = 0x0100,

        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        Canceled = 0x0200,

        /// <summary>
        /// The operation was aborted.
        /// </summary>
        Aborted = 0x0400,

        /// <summary>
        /// The operation timed out.
        /// </summary>
        TimedOut = 0x0800,
    }
}