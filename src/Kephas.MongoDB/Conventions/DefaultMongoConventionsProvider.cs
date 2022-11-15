// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMongoConventionsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB.Conventions;

using global::MongoDB.Bson.Serialization.Conventions;
using Kephas.Configuration;
using Kephas.Services;

/// <summary>
/// The default conventions provider.
/// </summary>
/// <remarks>
/// If no namespace conventions are provided in the mongoConfiguration, consider that all types are eligible.
/// </remarks>
[ProcessingPriority(Priority.Low)]
public class DefaultMongoConventionsProvider : IMongoConventionsProvider
{
    private readonly IConfiguration<MongoSettings> mongoConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMongoConventionsProvider"/> class.
    /// </summary>
    /// <param name="mongoConfiguration">The MongoDB configuration.</param>
    public DefaultMongoConventionsProvider(IConfiguration<MongoSettings> mongoConfiguration)
    {
        this.mongoConfiguration = mongoConfiguration ?? throw new ArgumentNullException(nameof(mongoConfiguration));
    }

    /// <summary>
    /// Gets the conventions for initializing the driver.
    /// </summary>
    /// <returns>The conventions.</returns>
    public MongoConventions GetConventions()
    {
        return new (this.GetConventionsName(), this.GetConventionPack(), this.IsEligibleType);
    }

    private string GetConventionsName()
    {
        var settings = this.mongoConfiguration.GetSettings();
        var namespaceConventions = settings.EntityNamespaceConventions is { Length: 0 }
            ? null
            : settings.EntityNamespaceConventions;
        var namespaces = namespaceConventions?.JoinWith(", ") ?? "<all namespaces>";
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
        var settings = this.mongoConfiguration.GetSettings();
        var namespaces = settings.EntityNamespaceConventions;
        if (namespaces is null || namespaces.Length == 0)
        {
            return true;
        }

        return namespaces.Any(ns => type.Namespace?.StartsWith(ns ?? string.Empty) ?? false);
    }
}