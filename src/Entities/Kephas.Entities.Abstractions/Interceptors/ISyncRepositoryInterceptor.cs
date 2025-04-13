// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.DependencyInjection;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Represents an async continuation for the next task to execute in the <see cref="IRepositorySyncInterceptor{TOperation,TResult}"/> pipeline.
/// </summary>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
/// <returns>The result.</returns>
public delegate TResult InterceptSyncNextDelegate<out TResult>();

/// <summary>
/// Marker interface for repository operation sync interceptors.
/// </summary>
public interface IRepositorySyncInterceptor;

/// <summary>
/// Service for intercepting repository operations.
/// </summary>
/// <typeparam name="TOperation">The operation arguments.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[TransientService(ServiceType = typeof(IRepositorySyncInterceptor), MetadataType = typeof(RepositoryInterceptorMetadata))]
public interface IRepositorySyncInterceptor<in TOperation, TResult> : IRepositorySyncInterceptor
    where TOperation : IRepositoryOperation<TResult>
{
    /// <summary>
    /// Invoked when the operation is executed with the provided arguments.
    /// </summary>
    /// <param name="operation">The intercepted operation.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <returns>The result.</returns>
    TResult Handle([DisallowNull] TOperation operation, InterceptSyncNextDelegate<TResult> next);    
}