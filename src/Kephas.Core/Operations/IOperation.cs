// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOperation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Defines the contract of an executable operation.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Executes the operation in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// An object.
        /// </returns>
#if NETSTANDARD2_1
        object? Execute(IContext? context = null)
        {
            return this.ExecuteAsync(context).GetResultNonLocking();
        }
#else
        object? Execute(IContext? context = null);
#endif

#if NETSTANDARD2_1
        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        Task<object?> ExecuteAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            return ((Func<object?>)(() => this.Execute(context))).AsAsync(cancellationToken);
        }
#endif
    }
}