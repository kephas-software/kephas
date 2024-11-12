// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Marker interface for repository operations.
/// </summary>
public interface IRepositoryOperation;

/// <summary>
/// Marker interface for repository operations yielding a specific result.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IRepositoryOperation<TResult> : IRepositoryOperation;
