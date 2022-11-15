// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo application initializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Application;

using System.Threading;
using System.Threading.Tasks;

using global::MongoDB.Bson;
using Kephas.Application;
using Kephas.Data;
using Kephas.Operations;

/// <summary>
/// A MongoDB application lifecycle behavior.
/// </summary>
/// <remarks>
/// Adds the <see cref="Id"/> temporary and empty values the ObjectId correspondents.
/// </remarks>
public class MongoAppLifecycleBehavior : IAppLifecycleBehavior
{
    /// <summary>
    /// Interceptor called before the application starts its asynchronous initialization.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    public Task<IOperationResult> BeforeAppInitializeAsync(CancellationToken cancellationToken = default)
    {
        Id.AddEmptyValue(ObjectId.Empty);
        Id.AddTemporaryValueCheck(value => value is ObjectId id && id < ObjectId.Empty);

        return Task.FromResult((IOperationResult)true.ToOperationResult());
    }
}