// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingletonAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the singleton application service contract attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a singleton application service contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false)]
public sealed class SingletonServiceAttribute : ServiceAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonServiceAttribute"/> class.
    /// </summary>
    public SingletonServiceAttribute(GenericRegistration genericRegistration = GenericRegistration.Default)
        : base(AppServiceLifetime.Singleton, genericRegistration)
    {
    }
}
