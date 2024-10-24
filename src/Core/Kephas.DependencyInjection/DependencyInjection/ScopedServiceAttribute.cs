// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopedAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scoped application service contract attribute class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a scoped application service contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false)]
public sealed class ScopedServiceAttribute : ServiceAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedServiceAttribute"/> class.
    /// </summary>
    public ScopedServiceAttribute()
        : base(AppServiceLifetime.Scoped)
    {
    }
}