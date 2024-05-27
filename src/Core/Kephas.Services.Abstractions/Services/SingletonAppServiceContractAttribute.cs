// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingletonAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the singleton application service contract attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

/// <summary>
/// Marks an interface or a class to be a singleton application service contract.
/// </summary>
/// <typeparam name="TContractDeclaration">The typed annotated as a contract declaration.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class SingletonAppServiceContractAttribute<TContractDeclaration> : AppServiceContractAttribute<TContractDeclaration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonAppServiceContractAttribute{TContractDeclaration}"/> class.
    /// </summary>
    public SingletonAppServiceContractAttribute()
        : base(AppServiceLifetime.Singleton)
    {
    }
}

/// <summary>
/// Marks an interface or a class to be a singleton application service contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SingletonAppServiceContractAttribute : AppServiceContractAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonAppServiceContractAttribute"/> class.
    /// </summary>
    public SingletonAppServiceContractAttribute()
        : base(AppServiceLifetime.Singleton)
    {
    }
}
