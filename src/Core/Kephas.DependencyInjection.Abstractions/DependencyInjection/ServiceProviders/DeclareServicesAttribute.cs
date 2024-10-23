// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection.ServiceProviders;

/// <summary>
/// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
/// </summary>
/// <typeparam name="T">The type providing the application services.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareServicesAttribute<T>(Priority priority = Priority.Normal) : Attribute, IServiceInfoProvider, IHasPriority
    where T : IServiceInfoProvider, new()
{
    /// <summary>
    /// Gets the processing priority.
    /// </summary>
    public Priority Priority => priority;

    /// <summary>
    /// Gets an enumeration of application service information objects and their contract declaration type.
    /// The contract declaration type is the type declaring the contract: if the <see cref="ServiceAttributeBase.ServiceType"/>
    /// is not provided, the contract declaration type is also the contract type.
    /// </summary>
    /// <returns>
    /// An enumeration of application service information objects and their contract declaration type.
    /// </returns>
    public IEnumerable<ServiceDeclarationInfo> GetServiceInfos()
    {
        try
        {
            var provider = new T();
            var contracts = provider.GetServiceInfos() ?? [];
            return contracts.ToArray();
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </summary>
    /// <returns>
    /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </returns>
    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations()
    {
        try
        {
            var services = new T().GetImplementationDeclarations() ?? [];
            return services.ToArray();
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"DeclareServices/{typeof(T)}/{Priority}";
}
