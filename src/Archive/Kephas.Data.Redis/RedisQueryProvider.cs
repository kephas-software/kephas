// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisQueryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis;

using System;
using System.Linq;

using Kephas.Data;
using Kephas.Data.Capabilities;
using Kephas.Data.Linq;

/// <summary>
/// The redis query provider.
/// </summary>
public class RedisQueryProvider : DataContextQueryProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisQueryProvider"/> class.
    /// </summary>
    /// <param name="queryOperationContext">Context for the query operation.</param>
    /// <param name="nativeQueryProvider">The native query provider.</param>
    public RedisQueryProvider(IQueryOperationContext queryOperationContext, IQueryProvider nativeQueryProvider)
        : base(queryOperationContext, nativeQueryProvider)
    {
    }

    /// <summary>Indicates whether an entity is attachable.</summary>
    /// <param name="entity">The entity.</param>
    /// <returns>True if the entity is attachable, false if not.</returns>
    protected override bool IsAttachable(object entity)
    {
        return entity is IEntityEntryAware;
    }

    /// <summary>Indicates whether an entity type is attachable.</summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>True if the entity type is attachable, false if not.</returns>
    protected override bool IsAttachableType(Type entityType)
    {
        return typeof(IEntityEntryAware).IsAssignableFrom(entityType);
    }
}