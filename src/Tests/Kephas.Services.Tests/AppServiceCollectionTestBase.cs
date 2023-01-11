// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Application;
using Kephas.Logging;
using Kephas.Reflection;
using Kephas.Runtime;
using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Testing;
using NSubstitute;
using NUnit.Framework;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public abstract class AppServiceCollectionTestBase : TestBase
{
    protected abstract IServiceProvider BuildServiceProvider(IAppServiceCollection appServices);

    [Test]
    public void Register_instance_cannot_set_null()
    {
        var appServices = this.CreateAppServices();
        Assert.That(() => appServices.Add(typeof(IServiceProvider), (object)null), Throws.InstanceOf<Exception>());
    }

    [Test]
    public void Register_factory_cannot_set_null()
    {
        var appServices = this.CreateAppServices();
        Assert.That(() => appServices.Add(typeof(IServiceProvider), (Func<IServiceProvider, object>)null), Throws.InstanceOf<Exception>());
    }

    [Test]
    public void Register_service_instance()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = this.BuildServiceProvider(appServices);
        Assert.AreSame(logManager, container.GetService(typeof(ILogManager)));
    }

    [Test]
    public void Register_service_type_singleton_dependency_resolved()
    {
        var appServices = this.CreateAppServices();
        var dependency = Substitute.For<IDependency>();
        appServices.Add(typeof(IService), typeof(DependentService));
        appServices.Add(typeof(IDependency), dependency);

        var container = this.BuildServiceProvider(appServices);
        var service = (DependentService)container.GetService(typeof(IService));
        Assert.AreSame(dependency, service.Dependency);
    }

    [Test]
    public void Register_service_type_singleton_optional_dependency_resolved()
    {
        var appServices = this.CreateAppServices();
        var dependency = Substitute.For<IDependency>();
        appServices.Add(typeof(IService), typeof(OptionalDependentService));
        appServices.Add(typeof(IDependency), dependency);

        var container = this.BuildServiceProvider(appServices);
        var service = (OptionalDependentService)container.GetService(typeof(IService));
        Assert.AreSame(dependency, service.Dependency);
    }

    [Test]
    public void Register_service_type_singleton_optional_dependency_not_resolved()
    {
        var appServices = this.CreateAppServices();
        appServices.Add(typeof(IService), typeof(OptionalDependentService));

        var container = this.BuildServiceProvider(appServices);
        var service = (OptionalDependentService)container.GetService(typeof(IService));
        Assert.IsNull(service.Dependency);
    }

    [Test]
    public void Register_service_type_singleton_dependency_ambiguous()
    {
        var appServices = this.CreateAppServices();
        appServices.Add(typeof(IService), typeof(AmbiguousDependentService));
        appServices.Add(typeof(IDependency), Substitute.For<IDependency>());
        appServices.Add(typeof(IAnotherDependency), Substitute.For<IAnotherDependency>());

        var container = this.BuildServiceProvider(appServices);
        Assert.Throws<AmbiguousMatchException>(() => container.GetService(typeof(IService)));
    }

    [Test]
    public void Register_service_type_singleton_dependency_non_ambiguous()
    {
        var appServices = this.CreateAppServices();
        appServices.Add(typeof(IService), typeof(AmbiguousDependentService));
        appServices.Add(typeof(IDependency), Substitute.For<IDependency>());

        var container = this.BuildServiceProvider(appServices);
        var service = (AmbiguousDependentService)container.GetService(typeof(IService));
        Assert.IsNotNull(service.Dependency);
        Assert.IsNull(service.AnotherDependency);
    }

    [Test]
    public void Register_service_type_singleton()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(IService), typeof(SimpleService));

        var container = this.BuildServiceProvider(appServices);

        var service = container.GetService(typeof(IService));
        Assert.IsInstanceOf<SimpleService>(service);

        var sameService = container.GetService(typeof(IService));
        Assert.AreSame(service, sameService);
    }

    [Test]
    public void Register_service_type_transient()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(IService), typeof(SimpleService), b => b.Transient());

        var container = this.BuildServiceProvider(appServices);

        var service = container.GetService(typeof(IService));
        Assert.IsInstanceOf<SimpleService>(service);

        var sameService = container.GetService(typeof(IService));
        Assert.AreNotSame(service, sameService);
    }

    [Test]
    public void Register_conflicting_allow_multiple_first_single_failure()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService>(typeof(SimpleService));
        Assert.Throws<InvalidOperationException>(() => appServices.Add<IService, DependentService>(b => b.AllowMultiple()));
    }

    [Test]
    public void Register_conflicting_allow_multiple_first_multiple_failure()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService>(typeof(SimpleService), b => b.AllowMultiple());
        Assert.Throws<InvalidOperationException>(() => appServices.Add<IService, DependentService>());
    }

    [Test]
    public void Register_service_factory()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), () => logManager);

        var container = this.BuildServiceProvider(appServices);
        Assert.AreSame(logManager, container.GetService(typeof(ILogManager)));
    }

    [Test]
    public void Register_service_factory_non_singleton()
    {
        var appServices = this.CreateAppServices();
        appServices.Add(typeof(ILogManager), () => Substitute.For<ILogManager>(), b => b.Transient());

        var container = this.BuildServiceProvider(appServices);
        var logManager1 = container.GetService(typeof(ILogManager));
        var logManager2 = container.GetService(typeof(ILogManager));
        Assert.AreNotSame(logManager1, logManager2);
    }

    [Test]
    public void Register_circular_dependency_singleton()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<CircularDependency1, CircularDependency1>();
        appServices.Add<CircularDependency2, CircularDependency2>();

        var container = this.BuildServiceProvider(appServices);

        Assert.Throws<CircularDependencyException>(() => container.GetService<CircularDependency1>());
    }

    [Test]
    public void Register_circular_dependency_transient()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<CircularDependency1, CircularDependency1>(b => b.Transient());
        appServices.Add<CircularDependency2, CircularDependency2>(b => b.Transient());

        var container = this.BuildServiceProvider(appServices);

        Assert.Throws<CircularDependencyException>(() => container.GetService<CircularDependency1>());
    }

    [Test]
    public Task Register_transient_is_multi_threaded()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService>(
            () =>
            {
                Thread.Sleep(100);
                return Substitute.For<IService>();
            },
            b => b.Transient());

        var container = this.BuildServiceProvider(appServices);

        var tasks = new List<Task>();
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(() => container.GetService<IService>()));
        }

        return Task.WhenAll(tasks);
    }

    [Test]
    public Task Register_singleton_is_multi_threaded()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService>(
            () =>
            {
                Thread.Sleep(100);
                return Substitute.For<IService>();
            },
            b => b.Singleton());

        var container = this.BuildServiceProvider(appServices);

        var tasks = new List<Task>();
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(() => container.GetService<IService>()));
        }

        return Task.WhenAll(tasks);
    }

    [Test]
    public void Register_service_open_generic()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(OpenGenericService<,>), typeof(OpenGenericService<,>));

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService(typeof(OpenGenericService<string, int>));
        Assert.IsInstanceOf<OpenGenericService<string, int>>(service);
    }

    [Test]
    public void Register_service_open_generic_dependency()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(OpenGenericService<,>), typeof(OpenGenericService<,>));
        appServices.Add<OpenGenericDependency, OpenGenericDependency>();

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService<OpenGenericDependency>();
        Assert.IsNotNull(service.ServiceIntString);
        Assert.IsNotNull(service.ServiceStringDouble);
    }

    [Test]
    public void GetService_exportFactory()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = this.BuildServiceProvider(appServices);
        var logManagerFactory = container.GetService<IExportFactory<ILogManager>>();
        Assert.AreSame(logManager, logManagerFactory.CreateExportedValue());
    }

    [Test]
    public void GetService_lazy()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = this.BuildServiceProvider(appServices);
        var logManagerFactory = container.GetService<Lazy<ILogManager>>();
        Assert.AreSame(logManager, logManagerFactory.Value);
    }

    [Test]
    public void GetService_collection_of_exportFactory()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, SimpleService>();
        appServices.Add<DependentCollectionService, DependentCollectionService>();

        var container = this.BuildServiceProvider(appServices);
        var dependent = container.GetService<DependentCollectionService>();
        Assert.IsNotNull(dependent.Factories);
        Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
    }

    [Test]
    public void GetService_collection_of_lazy()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, SimpleService>();
        appServices.Add<DependentCollectionService, DependentCollectionService>();

        var container = this.BuildServiceProvider(appServices);
        var factories = container.GetService<IEnumerable<Lazy<IService>>>();
        Assert.IsInstanceOf<SimpleService>(factories.Single().Value);
    }

    [Test]
    public void GetService_enumeration_of_exportFactory()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, SimpleService>();
        appServices.Add<DependentEnumerationService, DependentEnumerationService>();

        var container = this.BuildServiceProvider(appServices);
        var dependent = container.GetService<DependentEnumerationService>();
        Assert.IsNotNull(dependent.Factories);
        Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
    }

    [Test]
    public void GetService_enumeration_of_exportFactory_multiple()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, SimpleService>(b => b.AllowMultiple());
        appServices.Add<IService, OptionalDependentService>(b => b.AllowMultiple());
        appServices.Add<DependentEnumerationService, DependentEnumerationService>();

        var container = this.BuildServiceProvider(appServices);
        var dependent = container.GetService<DependentEnumerationService>();
        Assert.IsNotNull(dependent.Factories);
        var factories = dependent.Factories.ToList();
        Assert.AreEqual(2, factories.Count);
        Assert.IsTrue(factories.Any(f => f.CreateExportedValue() is SimpleService));
        Assert.IsTrue(factories.Any(f => f.CreateExportedValue() is OptionalDependentService));
    }

    [Test]
    public void GetService_exportFactory_with_priority_metadata()
    {
        var appServices = this.CreateAppServices();
        appServices.Add(
            typeof(IDependency),
            () => Substitute.For<IDependency>(),
            b => b
                .ProcessingPriority(Priority.Low)
                .OverridePriority(Priority.AboveNormal)
                .IsOverride());

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService<Lazy<IDependency, AppServiceMetadata>>();
        Assert.IsNotNull(service.Metadata);
        Assert.AreEqual(Priority.Low, service.Metadata.ProcessingPriority);
        Assert.AreEqual(Priority.AboveNormal, service.Metadata.OverridePriority);
        Assert.IsTrue(service.Metadata.IsOverride);
    }

    [Test]
    public void GetService_exportFactory_with_metadata()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, DependentService>();

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetRequiredService<IExportFactory<IService, AppServiceMetadata>>();
        Assert.IsNotNull(service.Metadata);
        Assert.AreEqual(Priority.High, service.Metadata.OverridePriority);
        Assert.AreEqual(typeof(DependentService), service.Metadata.ServiceType);
    }

    [Test]
    public void GetService_exportFactory_with_metadata_from_generic()
    {
        var appServices = this.CreateAppServices();
        appServices.AddService(typeof(IService<,>), typeof(GenericService), b => b.As<IService>());

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService<IExportFactory<IService, AppServiceMetadata>>()!;
        Assert.IsNotNull(service.Metadata);
        Assert.AreSame(typeof(string), service.Metadata["ValueType"]);
        Assert.AreSame(typeof(int), service.Metadata["Type"]);
    }

    [Test]
    public void GetService_lazy_with_metadata()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, DependentService>();
        appServices.Add<IDependency>(Substitute.For<IDependency>());

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService<Lazy<IService, AppServiceMetadata>>();
        Assert.IsNotNull(service.Metadata);
        Assert.AreEqual(Priority.High, service.Metadata.OverridePriority);
        Assert.AreEqual(typeof(DependentService), service.Metadata.ServiceType);
    }

    [Test]
    public void GetService_lazy_with_metadata_from_generic()
    {
        var appServices = this.CreateAppServices();
        appServices.AddService(typeof(IService<,>), typeof(GenericService), b => b.As<IService>());

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService<Lazy<IService, AppServiceMetadata>>();
        Assert.IsNotNull(service.Metadata);
        Assert.AreSame(typeof(string), service.Metadata["ValueType"]);
        Assert.AreSame(typeof(int), service.Metadata["Type"]);
    }

    [Test]
    public void GetService_two_levels()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<IService, DependentService>();
        appServices.Add<IDependency, DependencyWithDependency>();
        appServices.Add<IAnotherDependency>(Substitute.For<IAnotherDependency>());

        var container = this.BuildServiceProvider(appServices);
        var service = container.GetService<IService>();
        Assert.IsNotNull(((DependencyWithDependency)((DependentService)service).Dependency).AnotherDependency);
    }

    [Test]
    public void GetService_enumerable()
    {
        var appServices = this.CreateAppServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = this.BuildServiceProvider(appServices);
        var logManagerCollection = container.GetService<IEnumerable<ILogManager>>();
        Assert.AreSame(logManager, logManagerCollection.Single());
    }

    [Test]
    public void GetAppServiceInfos_default_services()
    {
        var appServices = this.CreateAppServices();
        appServices = new AppServiceCollectionBuilder(appServices)
            .WithStaticAppRuntime()
            .Build();
        var contracts = appServices.GetServiceInstance<IContractDeclarationCollection>();

        var (c, info) = contracts.SingleOrDefault(i => i.ContractDeclarationType == typeof(ILogManager));
        Assert.IsNotNull(info);
        Assert.IsNotNull(info.InstanceFactory);

        (c, info) = contracts.SingleOrDefault(i => i.ContractDeclarationType == typeof(IAppRuntime));
        Assert.IsNotNull(info);
        Assert.IsNotNull(info.InstanceFactory);
    }

    [Test]
    public void GetAppServiceInfos_no_default_services()
    {
        IAppServiceCollection appServices = new AppServiceCollection(registerDefaultServices: false);
        appServices = new AppServiceCollectionBuilder(appServices).Build();
        var contracts = appServices.GetServiceInstance<IContractDeclarationCollection>();

        Assert.AreEqual(2, contracts.Count());

        var (c, info) = contracts.SingleOrDefault(i => i.ContractDeclarationType == typeof(IAppServiceCollection));
        Assert.IsNotNull(info);
        Assert.IsNotNull(info.InstanceFactory);
        Assert.AreSame(appServices, info.InstanceFactory(null));

        (c, info) = contracts.SingleOrDefault(i => i.ContractDeclarationType == typeof(IRuntimeTypeRegistry));
        Assert.IsNotNull(info);
        Assert.IsNotNull(info.InstanceFactory);
        Assert.AreSame(RuntimeTypeRegistry.Instance, info.InstanceFactory(null));
    }

    public interface IService { }
    public interface IDependency { }
    public interface IAnotherDependency { }
    public interface IService<TValue, TType> : IService { }

    public class DependentCollectionService
    {
        public DependentCollectionService(ICollection<IExportFactory<IService>> factories)
        {
            this.Factories = factories;
        }

        public ICollection<IExportFactory<IService>> Factories { get; }
    }

    public class DependentEnumerationService
    {
        public DependentEnumerationService(IEnumerable<IExportFactory<IService>> factories)
        {
            this.Factories = factories;
        }

        public IEnumerable<IExportFactory<IService>> Factories { get; }
    }

    public class SimpleService : IService { }

    [OverridePriority(Priority.High)]
    public class DependentService : IService
    {
        public IDependency Dependency { get; }

        public DependentService(IDependency dependency)
        {
            this.Dependency = dependency;
        }
    }

    public class OptionalDependentService : IService
    {
        public IDependency? Dependency { get; }

        public OptionalDependentService(IDependency? dependency = null)
        {
            this.Dependency = dependency;
        }
    }

    public class AmbiguousDependentService : IService
    {
        public IAnotherDependency AnotherDependency { get; }

        public IDependency Dependency { get; }

        public AmbiguousDependentService(IDependency dependency)
        {
            this.Dependency = dependency;
        }

        public AmbiguousDependentService(IAnotherDependency anotherDependency)
        {
            this.AnotherDependency = anotherDependency;
        }
    }

    public class GenericService : IService<string, int> { }

    public class DependencyWithDependency : IDependency
    {
        public IAnotherDependency AnotherDependency { get; }

        public DependencyWithDependency(IAnotherDependency dependency)
        {
            this.AnotherDependency = dependency;
        }
    }

    public class CircularDependency1
    {
        public CircularDependency1(CircularDependency2 dependency) { }
    }

    public class CircularDependency2
    {
        public CircularDependency2(CircularDependency1 dependency) { }
    }

    public class OpenGenericService<T1, T2> { }

    public class OpenGenericDependency
    {
        public OpenGenericDependency(OpenGenericService<int, string> svcIntString, OpenGenericService<string, double> svcStringDouble)
        {
            this.ServiceIntString = svcIntString;
            this.ServiceStringDouble = svcStringDouble;
        }

        public OpenGenericService<int, string> ServiceIntString { get; }

        public OpenGenericService<string, double> ServiceStringDouble { get; }
    }
}