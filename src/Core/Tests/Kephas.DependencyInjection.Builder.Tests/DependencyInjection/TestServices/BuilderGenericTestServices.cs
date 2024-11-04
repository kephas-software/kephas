// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuilderGenericTestServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.ServiceProviders;

namespace Kephas.Tests.DependencyInjection.TestServices
{
    #region Open Generics
    
    [SingletonService]
    public interface IOpenGenericAsDefault<T>;

    public class OpenGenericAsDefault<T> : IOpenGenericAsDefault<T>;
    
    [Priority(Priority.High)]
    public class HigherOpenGenericAsDefault<T> : IOpenGenericAsDefault<T>;
    
    [OverrideBaseImplementation]
    public class OverrideOpenGenericAsDefault<T> : OpenGenericAsDefault<T>;

    [SingletonService(RegistrationKind.AsOpenGeneric)]
    public interface IOpenGenericAsExplicit<T, U>;

    public class OpenGenericAsExplicit<T, U> : IOpenGenericAsExplicit<T, U>;
    
    #endregion
    
    #region One generic parameter collectors
    
    public interface ICollector;

    [SingletonService(ServiceType = typeof(ICollector))]
    public interface IGenericCollector<T> : ICollector;

    public class StringGenericCollector : IGenericCollector<string>;

    internal class GenericCollectorServiceInfoProvider : IServiceInfoProvider
    {
        public IEnumerable<ServiceDeclaration>? GetServiceDeclarations()
        {
            yield return new ServiceDeclaration(typeof(IGenericCollector<>));
        }
        
        public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
        {
            yield return new ServiceImplementationDeclaration(typeof(StringGenericCollector), typeof(IGenericCollector<string>));
        }
    }
    
    #endregion
    
    #region Two generic parameter collectors

    public interface ITwoCollector;

    [ScopedService(ServiceType = typeof(ITwoCollector))]
    public interface IGenericTwoCollector<TFrom, ToType> : ITwoCollector;

    public class IntBoolGenericTwoCollector : IGenericTwoCollector<int, bool>;

    #endregion
    
    internal class GenericServiceInfoProvider(Type serviceDeclarationType, Type[] implementationTypes, Type[]? serviceDeclarationTypes = null) : IServiceInfoProvider
    {
        public IEnumerable<ServiceDeclaration>? GetServiceDeclarations()
        {
            yield return new ServiceDeclaration(serviceDeclarationType);
        }
        
        public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes) =>
            implementationTypes.Select((t, i) => new ServiceImplementationDeclaration(t, serviceDeclarationTypes?[i] ?? serviceDeclarationType));
    }
}