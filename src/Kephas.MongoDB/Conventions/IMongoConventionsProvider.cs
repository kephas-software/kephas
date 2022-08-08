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

/// <summary>
/// The default conventions provider.
/// </summary>
/// <remarks>
/// If no namespace conventions are provided in the options, consider that all types are eligible.
/// </remarks>
[ProcessingPriority(Priority.Low)]
public class DefaultMongoConventionsProvider : IMongoConventionsProvider
{
    private readonly IConfiguration<MongoOptions> options;

    public DefaultMongoConventionsProvider(IConfiguration<MongoOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public MongoConventions GetConventions()
    {
        return new(GetConventionsName(), GetConventionPack(), this.IsEligibleType);
    }

    private string GetConventionsName()
    {
        var namespaceConventions = options.Value.ModelNamespaceConventions is { Length: 0 }
            ? null
            : options.Value.ModelNamespaceConventions;
        var namespaces = namespaceConventions?.Join(", ") ?? "<all namespaces>";
        return $"Conventions for {namespaces}";
    }

    private ConventionPack GetConventionPack()
    {
        return new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
        };
    }

    private bool IsEligibleType(Type type)
    {
        // if no namespace conventions are provided, consider that all types are eligible
        var namespaces = options.Value.ModelNamespaceConventions;
        if (namespaces is null || namespaces.Length == 0)
        {
            return true;
        }

        return namespaces.Any(ns => type.Namespace?.StartsWith(ns ?? string.Empty) ?? false);
    }
}
