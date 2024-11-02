// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Indicates a processing priority used when more services must be processed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Indicates a processing priority used when more services must be processed.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PriorityAttribute : Attribute, IServiceMetadataValue<Priority>, IHasPriority
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityAttribute" /> class.
    /// </summary>
    /// <param name="priority">The processing priority.</param>
    public PriorityAttribute(int priority) => Value = (Priority)priority;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityAttribute" /> class.
    /// </summary>
    /// <param name="priority">The processing priority.</param>
    public PriorityAttribute(Priority priority) => Value = priority;

    /// <summary>
    /// Gets the priority value.
    /// </summary>
    /// <value>
    /// The priority value.
    /// </value>
    public Priority Value { get; }

    /// <summary>
    /// Gets the processing priority.
    /// </summary>
    /// <value>
    /// The processing priority.
    /// </value>
    Priority IHasPriority.Priority => Value;
}