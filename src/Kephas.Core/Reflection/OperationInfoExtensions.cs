// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationInfoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

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
            operationInfo = operationInfo ?? throw new ArgumentNullException(nameof(operationInfo));

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