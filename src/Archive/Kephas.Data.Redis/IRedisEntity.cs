// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRedisEntity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis;

using Kephas.Data.Capabilities;

/// <summary>
/// Interface gathering constraints for Redis persisted entities.
/// </summary>
/// <seealso cref="IIdentifiable" />
public interface IRedisEntity : IIdentifiable, IEntityEntryAware
{
}
