// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAsyncOperation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

#if NETSTANDARD2_1
#else

namespace Kephas.Operations
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Defines the contract of an executable asynchronous operation.
    /// </summary>
    public interface IAsyncOperation
    {
        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        Task<object?> ExecuteAsync(IContext? context = null, CancellationToken cancellationToken = default);
    }
}

#endif