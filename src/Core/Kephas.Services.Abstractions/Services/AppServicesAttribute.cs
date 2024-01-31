// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
/// </summary>
/// <typeparam name="T">The type providing the application services.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class AppServicesAttribute(Type providerType, Priority priority = Priority.Normal) : Attribute, IAppServiceInfoProvider, IHasPriority
{
    /// <summary>
    /// Gets the processing priority.
    /// </summary>
    public Priority Priority => priority;

    /// <summary>
    /// Gets an enumeration of application service information objects and their contract declaration type.
    /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
    /// is not provided, the contract declaration type is also the contract type.
    /// </summary>
    /// <returns>
    /// An enumeration of application service information objects and their contract declaration type.
    /// </returns>
    public IEnumerable<ContractDeclaration> GetAppServiceContracts()
    {
        try
        {
            var provider = (IAppServiceInfoProvider)Activator.CreateInstance(providerType)!;
            var contracts = provider.GetAppServiceContracts() ?? Array.Empty<ContractDeclaration>();
            return contracts.ToArray();
        }
        catch (Exception)
        {
            return Enumerable.Empty<ContractDeclaration>();
        }
    }

    /// <summary>
    /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </summary>
    /// <returns>
    /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </returns>
    public IEnumerable<ServiceDeclaration> GetAppServices()
    {
        try
        {
            var provider = (IAppServiceInfoProvider)Activator.CreateInstance(providerType)!;
            var services = provider.GetAppServices() ?? Enumerable.Empty<ServiceDeclaration>();
            return services.ToArray();
        }
        catch (Exception)
        {
            return Enumerable.Empty<ServiceDeclaration>();
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.ToString?view=netstandard-2.1">`Object.ToString` on docs.microsoft.com</a></footer>
    public override string ToString()
    {
        return $"AppServices/{providerType}/{this.Priority}";
    }
}

/// <summary>
/// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
/// </summary>
/// <typeparam name="T">The type providing the application services.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class AppServicesAttribute<T>(Priority priority = Priority.Normal) : Attribute, IAppServiceInfoProvider, IHasPriority
    where T : IAppServiceInfoProvider, new()
{
    /// <summary>
    /// Gets the processing priority.
    /// </summary>
    public Priority Priority => priority;

    /// <summary>
    /// Gets an enumeration of application service information objects and their contract declaration type.
    /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
    /// is not provided, the contract declaration type is also the contract type.
    /// </summary>
    /// <returns>
    /// An enumeration of application service information objects and their contract declaration type.
    /// </returns>
    public IEnumerable<ContractDeclaration> GetAppServiceContracts()
    {
        try
        {
            var provider = new T();
            var contracts = provider.GetAppServiceContracts() ?? Array.Empty<ContractDeclaration>();
            return contracts.ToArray();
        }
        catch (Exception)
        {
            return Enumerable.Empty<ContractDeclaration>();
        }
    }

    /// <summary>
    /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </summary>
    /// <returns>
    /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </returns>
    public IEnumerable<ServiceDeclaration> GetAppServices()
    {
        try
        {
            var provider = new T();
            var services = provider.GetAppServices() ?? Enumerable.Empty<ServiceDeclaration>();
            return services.ToArray();
        }
        catch (Exception)
        {
            return Enumerable.Empty<ServiceDeclaration>();
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.ToString?view=netstandard-2.1">`Object.ToString` on docs.microsoft.com</a></footer>
    public override string ToString()
    {
        return $"AppServices/{typeof(T)}/{this.Priority}";
    }
}
