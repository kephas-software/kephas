// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for method information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Contract for operation information.
    /// </summary>
    public interface IOperationInfo : IElementInfo
    {
        /// <summary>
        /// Gets the return type of the operation.
        /// </summary>
        /// <value>
        /// The return type of the operation.
        /// </value>
        ITypeInfo? ReturnType { get; }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        IEnumerable<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Gets the return type of the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the return type of the operation.
        /// </returns>
        Task<ITypeInfo?> GetReturnTypeAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(this.ReturnType);

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        object? Invoke(object? instance, IEnumerable<object?> args);
    }

    /// <summary>
    /// Extension methods for <see cref="IOperationInfo"/>.
    /// </summary>
    public static class OperationInfoExtensions
    {
        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="operationInfo">The operation info.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public static async Task<object?> InvokeAsync(this IOperationInfo operationInfo, object? instance, IEnumerable<object?> args)
        {
            Requires.NotNull(operationInfo, nameof(operationInfo));

            var result = operationInfo.Invoke(instance, args);
            if (result is Task taskResult)
            {
                await taskResult.PreserveThreadContext();
                result = taskResult.GetResult();
            }
            else if (result is ValueTask valueTaskResult)
            {
                taskResult = valueTaskResult.AsTask();
                await taskResult.PreserveThreadContext();
                result = taskResult.GetResult();
            }

            return result;
        }
    }
}