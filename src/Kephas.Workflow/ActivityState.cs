// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity state class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    /// <summary>
    /// A bit-field of flags for specifying activity states.
    /// </summary>
    [Flags]
    public enum ActivityState
    {
        /// <summary>
        /// The activity is created, but the execution did not start.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// The activity started the execution.
        /// </summary>
        Started = 0x0001,

        /// <summary>
        /// The execution is paused.
        /// </summary>
        Paused = 0x0002,

        /// <summary>
        /// The execution failed.
        /// </summary>
        Failed = 0x0010,

        /// <summary>
        /// The execution was canceled by the user.
        /// </summary>
        Canceled = 0x0020,

        /// <summary>
        /// The execution was aborted.
        /// </summary>
        Aborted = 0x0040,

        /// <summary>
        /// The operation timed out.
        /// </summary>
        TimedOut = 0x0080,

        /// <summary>
        /// The activity's execution completed.
        /// </summary>
        Completed = 0x0100,
    }
}