// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInitializerAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CodeAnalysis;

/// <summary>
/// Attribute indicating assembly initializer attributes.
/// </summary>
/// <remarks>
/// This attribute is typically used in generated code to indicate initializers
/// that are also generated.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class AssemblyInitializerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyInitializerAttribute"/> class.
    /// </summary>
    /// <param name="initializerTypes">The initializer types.</param>
    public AssemblyInitializerAttribute(params Type[] initializerTypes)
    {
        this.InitializerTypes = initializerTypes ?? Array.Empty<Type>();
    }

    /// <summary>
    /// Gets the initializer types;
    /// </summary>
    public Type[] InitializerTypes { get; }
}
