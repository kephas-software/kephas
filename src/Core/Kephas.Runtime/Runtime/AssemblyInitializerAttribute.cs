// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInitializerAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime;

/// <summary>
/// Attribute indicating assembly initializer attributes.
/// </summary>
/// <remarks>
/// This attribute is typically used in generated code to indicate initializers
/// that are also generated.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class AssemblyInitializerAttribute<T> : Attribute, IAssemblyInitializer
    where T : IAssemblyInitializer, new()
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void InitializeAssembly() => new T().InitializeAssembly();
}
