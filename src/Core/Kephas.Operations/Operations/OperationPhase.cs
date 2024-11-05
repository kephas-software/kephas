// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationStatusCategory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Enumerates the values for the operation phases.
/// </summary>
[Flags]
public enum OperationPhase
{
    /// <summary>
    /// The operation is not yet started.
    /// </summary>
    None = 0x0000,
    
    /// <summary>
    /// The operation is executing.
    /// </summary>
    /// <remarks>
    /// This is a flag marking all the status values which indicate execution.
    /// </remarks>
    Executing = 0x0001,

    /// <summary>
    /// The operation execution has been halted, and is waiting for an event to resume execution.
    /// </summary>
    /// <remarks>
    /// This is a flag marking all the status values which indicate a wait status.
    /// </remarks>
    Waiting = 0x0010,

    /// <summary>
    /// The operation failed. This indicates a final status.
    /// </summary>
    /// <remarks>
    /// This is a flag marking all the status values which indicate failure.
    /// </remarks>
    Failed = 0x0100,

    /// <summary>
    /// The operation completed. This indicates a final status.
    /// </summary>
    /// <remarks>
    /// This is a flag marking all the status values which indicate completion.
    /// </remarks>
    Completed = 0x1000,
}