// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMongoSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB.Serializers;

using global::MongoDB.Bson.Serialization;
using Kephas.Services;

/// <summary>
/// Base interface for serializer services.
/// </summary>
public interface IMongoSerializer : IBsonSerializer
{
}

/// <summary>
/// Service contract for serializer services.
/// </summary>
/// <typeparam name="TValue">The serialized value type.</typeparam>
[AppServiceContract(AllowMultiple = true, ContractType = typeof(IMongoSerializer))]
public interface IMongoSerializer<TValue> : IBsonSerializer<TValue>
{
}
