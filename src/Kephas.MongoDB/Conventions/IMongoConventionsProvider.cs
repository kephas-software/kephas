using MongoDB.Bson.Serialization.Conventions;

namespace Kephas.MongoDB.Conventions;

using Kephas.Services;

/// <summary>
/// Provides conventions for MongoDB client.
/// </summary>
/// <param name="Name">The registration name.</param>
/// <param name="ConventionPack">The conventions pack.</param>
/// <param name="Filter">Filter function for entity types.</param>
public record MongoConventions(string Name, ConventionPack ConventionPack, Func<Type, bool>? Filter);

[AppServiceContract(AllowMultiple = true)]
public interface IMongoConventionsProvider
{
    MongoConventions GetConventions();
}
