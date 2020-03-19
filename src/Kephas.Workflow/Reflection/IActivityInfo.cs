// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivityInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow.Reflection
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Contract interface for activity metadata.
    /// </summary>
    public interface IActivityInfo : ITypeInfo, IOperationInfo
    {
        /// <summary>
        /// Executes the activity asynchronously.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        Task<object?> ExecuteAsync(
            IActivity activity,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default);
    }
}