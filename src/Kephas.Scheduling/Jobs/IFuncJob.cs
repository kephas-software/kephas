// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFuncJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the function job interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Jobs
{
    using Kephas.Operations;

    /// <summary>
    /// Contract for automated pieces of work that can be performed at either a particular time, or on a recurring schedule,
    /// based on a function to be executed.
    /// </summary>
    /// <remarks>Jobs are specializations of activities.</remarks>
    public interface IFuncJob : IJob, IOperation
#if NETSTANDARD2_0
        , IAsyncOperation
#endif
    {
    }
}