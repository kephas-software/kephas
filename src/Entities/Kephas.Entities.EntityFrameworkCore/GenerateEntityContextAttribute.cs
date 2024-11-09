// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateEntityContextAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities;

/// <summary>
/// Declares in a relational assembly that a <see cref="EntityContextBase{TDbContext}"/>
/// will be generated with the provided name and will include the indicated entity types.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class GenerateEntityContextAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateEntityContextAttribute"/> class.
    /// </summary>
    /// <param name="name">The data context name.</param>
    /// <param name="entityTypes">The entity types served by this data context.</param>
    public GenerateEntityContextAttribute(string name, params Type[] entityTypes)
    {
        Name = name;
        EntityTypes = entityTypes;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateEntityContextAttribute"/> class.
    /// </summary>
    /// <param name="name">The data context name.</param>
    /// <param name="priority">The data context priority used by the dependency injection.</param>
    /// <param name="entityTypes">The entity types served by this data context.</param>
    public GenerateEntityContextAttribute(string name, Priority priority, params Type[] entityTypes)
    {
        Name = name;
        Priority = priority;
        EntityTypes = entityTypes;
    }

    /// <summary>
    /// Gets the data context name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the data context priority used by the dependency injection.
    /// </summary>
    public Priority Priority { get; } = Priority.Normal;

    /// <summary>
    /// Gets the entity types used in the context.
    /// </summary>
    public Type[] EntityTypes { get; }
}
