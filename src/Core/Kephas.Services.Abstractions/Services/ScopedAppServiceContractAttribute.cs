// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopedAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scoped application service contract attribute class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

/// <summary>
/// Marks an interface or a class to be a scoped application service contract, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TContractDeclaration">The typed annotated as a contract declaration.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ScopedAppServiceContractAttribute<TContractDeclaration> : AppServiceContractAttribute<TContractDeclaration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedAppServiceContractAttribute{TContractDeclaration}"/> class.
    /// </summary>
    public ScopedAppServiceContractAttribute()
        : base(AppServiceLifetime.Scoped)
    {
    }
}

/// <summary>
/// Marks an interface or a class to be a scoped application service contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ScopedAppServiceContractAttribute : AppServiceContractAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedAppServiceContractAttribute"/> class.
    /// </summary>
    public ScopedAppServiceContractAttribute()
        : base(AppServiceLifetime.Scoped)
    {
    }
}
