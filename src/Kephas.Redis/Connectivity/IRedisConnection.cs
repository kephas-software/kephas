// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRedisConnection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Connectivity;

using Kephas.Connectivity;
using StackExchange.Redis;

/// <summary>
/// Interface for a connection to Redis.
/// </summary>
/// <seealso cref="IConnection" />
/// <seealso cref="IAdapter{IConnectionMultiplexer}" />
public interface IRedisConnection : IConnection, IAdapter<IConnectionMultiplexer>
{
}