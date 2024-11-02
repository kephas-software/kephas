// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransientAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a transient application service contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false)]
public sealed class TransientServiceAttribute : ServiceAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedServiceAttribute"/> class.
    /// </summary>
    public TransientServiceAttribute(GenericRegistration genericRegistration = GenericRegistration.Default)
        : base(AppServiceLifetime.Transient, genericRegistration)
    {
    }
}