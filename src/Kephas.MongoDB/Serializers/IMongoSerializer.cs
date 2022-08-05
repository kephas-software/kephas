namespace Kephas.MongoDB.Serializers;

using global::MongoDB.Bson.Serialization;
using Kephas.Services;

/// <summary>
/// Base interface for serializer services.
/// </summary>
public interface IMongoSerializer
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