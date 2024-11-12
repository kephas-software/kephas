// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryOperationInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.DependencyInjection;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Represents an async continuation for the next task to execute in the <see cref="IRepositoryOperationInterceptor{TOperation,TResult}"/> pipeline.
/// </summary>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
/// <returns>An awaitable task.</returns>
public delegate Task<TResult> InterceptNextDelegate<TResult>();

/// <summary>
/// Marker interface for repository operation interceptors.
/// </summary>
public interface IRepositoryOperationInterceptor;

/// <summary>
/// Service for intercepting repository operations.
/// </summary>
/// <typeparam name="TOperation">The operation arguments.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[TransientService(ServiceType = typeof(IRepositoryOperationInterceptor))]
public interface IRepositoryOperationInterceptor<in TOperation, TResult> : IRepositoryOperationInterceptor
    where TOperation : IRepositoryOperation<TResult>
{
    /// <summary>
    /// Invoked when the operation is executed with the provided arguments.
    /// </summary>
    /// <param name="operation">The intercepted operation.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    Task<TResult> HandleAsync([DisallowNull] TOperation operation, InterceptNextDelegate<TResult> next, CancellationToken cancellationToken = default);    
}