// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOOperationState.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data i/o operation state class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    /// <summary>
    /// The data exchange operation state.
    /// </summary>
    public enum DataIOOperationState
    {
        /// <summary>
        /// The not started state.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The in progress state.
        /// </summary>
        InProgress,

        /// <summary>
        /// The canceled state.
        /// </summary>
        Canceled,

        /// <summary>
        /// The completed successfully state.
        /// </summary>
        CompletedSuccessfully,

        /// <summary>
        /// The completed with errors state.
        /// </summary>
        CompletedWithErrors,
    }
}