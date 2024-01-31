// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Metadata;
using Kephas.Services.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentAssertions;
using Kephas.Reflection;
using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Testing;
using NSubstitute;
using Xunit;

namespace Kephas.Tests;

public class AppServiceCollectionTest
{
    [Fact]
    public void Register_circular_dependency_singleton()
    {
        var appServices = new AppServiceCollection();
        appServices.Add<CircularDependency1, CircularDependency1>();
        appServices.Add<CircularDependency2, CircularDependency2>();

        var container = appServices.BuildServiceProvider();

        container.Invoking(c => c.GetService<CircularDependency1>())
            .Should().Throw<InvalidOperationException>()
            .And.Message.Should().StartWith("A circular dependency was detected for the service of type");
    }

    [Fact]
    public void Register_circular_dependency_transient()
    {
        var appServices = new AppServiceCollection();
        appServices.Add<CircularDependency1, CircularDependency1>(b => b.Transient());
        appServices.Add<CircularDependency2, CircularDependency2>(b => b.Transient());

        var container = appServices.BuildServiceProvider();

        container.Invoking(c => c.GetService<CircularDependency1>())
            .Should().Throw<InvalidOperationException>()
            .And.Message.Should().StartWith("A circular dependency was detected for the service of type");
    }

    [Fact]
    public void Register_instance_cannot_set_null()
    {
        var appServices = Test.CreateTestServices();
        Assert.ThrowsAny<Exception>(() => appServices.Add(typeof(IServiceProvider), (object)null!));
    }

    [Fact]
    public void Register_factory_cannot_set_null()
    {
        var appServices = Test.CreateTestServices();
        Assert.ThrowsAny<Exception>(() => appServices.Add(typeof(IServiceProvider), (Func<IServiceProvider, object>)null!));
    }

    [Fact]
    public void Register_service_instance()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = appServices.BuildServiceProvider();
        Assert.Same(logManager, container.GetService(typeof(ILogManager)));
    }

    [Fact]
    public void Register_service_type_singleton_dependency_resolved()
    {
        var appServices = Test.CreateTestServices();
        var dependency = Substitute.For<IDependency>();
        appServices.Add(typeof(IService), typeof(DependentService));
        appServices.Add(typeof(IDependency), dependency);

        var container = appServices.BuildServiceProvider();
        var service = (DependentService)container.GetService(typeof(IService));
        Assert.Same(dependency, service.Dependency);
    }

    [Fact]
    public void Register_service_type_singleton_optional_dependency_resolved()
    {
        var appServices = Test.CreateTestServices();
        var dependency = Substitute.For<IDependency>();
        appServices.Add(typeof(IService), typeof(OptionalDependentService));
        appServices.Add(typeof(IDependency), dependency);

        var container = appServices.BuildServiceProvider();
        var service = (OptionalDependentService)container.GetService(typeof(IService));
        Assert.Same(dependency, service.Dependency);
    }

    [Fact]
    public void Register_service_type_singleton_optional_dependency_not_resolved()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(typeof(IService), typeof(OptionalDependentService));

        var container = appServices.BuildServiceProvider();
        var service = (OptionalDependentService)container.GetService(typeof(IService));
        Assert.Null(service.Dependency);
    }

    [Fact]
    public void Register_service_type_singleton_dependency_ambiguous()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(typeof(IService), typeof(AmbiguousDependentService));
        appServices.Add(typeof(IDependency), Substitute.For<IDependency>());
        appServices.Add(typeof(IAnotherDependency), Substitute.For<IAnotherDependency>());

        var container = appServices.BuildServiceProvider();
        Action getService = (() => container.GetService(typeof(IService)));
        getService.Should().Throw<InvalidOperationException>()
            .And.Message.Should()
            .StartWith("Unable to activate type 'Kephas.Tests.AppServiceCollectionTest+AmbiguousDependentService'. The following constructors are ambiguous");
    }

    [Fact]
    public void Register_service_type_singleton_dependency_non_ambiguous()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(typeof(IService), typeof(AmbiguousDependentService));
        appServices.Add(typeof(IDependency), Substitute.For<IDependency>());

        var container = appServices.BuildServiceProvider();
        var service = (AmbiguousDependentService)container.GetService(typeof(IService));
        Assert.NotNull(service.Dependency);
        Assert.Null(service.AnotherDependency);
    }

    [Fact]
    public void Register_service_type_singleton()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(IService), typeof(SimpleService));

        var container = appServices.BuildServiceProvider();

        var service = container.GetService(typeof(IService));
        Assert.IsType<SimpleService>(service);

        var sameService = container.GetService(typeof(IService));
        Assert.Same(service, sameService);
    }

    [Fact]
    public void Register_service_type_transient()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(IService), typeof(SimpleService), b => b.Transient());

        var container = appServices.BuildServiceProvider();

        var service = container.GetService(typeof(IService));
        Assert.IsType<SimpleService>(service);

        var sameService = container.GetService(typeof(IService));
        Assert.NotSame(service, sameService);
    }

    [Fact]
    public void Register_conflicting_allow_multiple_first_single_failure()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService>(typeof(SimpleService));
        appServices.Add<IService, DependentService>(b => b.AllowMultiple = true);
        Assert.Throws<InvalidOperationException>(() => appServices.BuildServiceProvider());
    }

    [Fact]
    public void Register_conflicting_allow_multiple_first_multiple_failure()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService>(typeof(SimpleService), b => b.AllowMultiple = true);
        Assert.Throws<InvalidOperationException>(() => appServices.Add<IService, DependentService>());
    }

    [Fact]
    public void Register_service_factory()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), () => logManager);

        var container = appServices.BuildServiceProvider();
        Assert.Same(logManager, container.GetService(typeof(ILogManager)));
    }

    [Fact]
    public void Register_service_factory_non_singleton()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(typeof(ILogManager), () => Substitute.For<ILogManager>(), b => b.Transient());

        var container = appServices.BuildServiceProvider();
        var logManager1 = container.GetService(typeof(ILogManager));
        var logManager2 = container.GetService(typeof(ILogManager));
        Assert.NotSame(logManager1, logManager2);
    }

    [Fact]
    public Task Register_transient_is_multi_threaded()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService>(
            () =>
            {
                Thread.Sleep(100);
                return Substitute.For<IService>();
            },
            b => b.Transient());

        var container = appServices.BuildServiceProvider();

        var tasks = new List<Task>();
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(() => container.GetService<IService>()));
        }

        return Task.WhenAll(tasks);
    }

    [Fact]
    public Task Register_singleton_is_multi_threaded()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService>(
            () =>
            {
                Thread.Sleep(100);
                return Substitute.For<IService>();
            },
            b => b.Singleton());

        var container = appServices.BuildServiceProvider();

        var tasks = new List<Task>();
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(() => container.GetService<IService>()));
        }

        return Task.WhenAll(tasks);
    }

    [Fact]
    public void Register_service_open_generic()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(OpenGenericService<,>), typeof(OpenGenericService<,>));

        var container = appServices.BuildServiceProvider();
        var service = container.GetService(typeof(OpenGenericService<string, int>));
        Assert.IsType<OpenGenericService<string, int>>(service);
    }

    [Fact]
    public void Register_service_open_generic_dependency()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(OpenGenericService<,>), typeof(OpenGenericService<,>));
        appServices.Add<OpenGenericDependency, OpenGenericDependency>();

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<OpenGenericDependency>();
        Assert.NotNull(service.ServiceIntString);
        Assert.NotNull(service.ServiceStringDouble);
    }

    [Fact]
    public void GetService_exportFactory()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = appServices.BuildServiceProvider();
        var logManagerFactory = container.GetService<IExportFactory<ILogManager>>();
        Assert.Same(logManager, logManagerFactory.CreateExportedValue());
    }

    [Fact]
    public void GetService_lazy()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = appServices.BuildServiceProvider();
        var logManagerFactory = container.GetService<Lazy<ILogManager>>();
        Assert.Same(logManager, logManagerFactory.Value);
    }

    [Fact]
    public void GetService_collection_of_exportFactory()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, SimpleService>();
        appServices.Add<DependentCollectionService, DependentCollectionService>();

        var container = appServices.BuildServiceProvider();
        var dependent = container.GetService<DependentCollectionService>();
        Assert.NotNull(dependent.Factories);
        Assert.IsType<SimpleService>(dependent.Factories.Single().CreateExportedValue());
    }

    [Fact]
    public void GetService_collection_of_lazy()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, SimpleService>();
        appServices.Add<DependentCollectionService, DependentCollectionService>();

        var container = appServices.BuildServiceProvider();
        var factories = container.GetService<IEnumerable<Lazy<IService>>>();
        Assert.IsType<SimpleService>(factories.Single().Value);
    }

    [Fact]
    public void GetService_enumeration_of_exportFactory()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, SimpleService>();
        appServices.Add<DependentEnumerationService, DependentEnumerationService>();

        var container = appServices.BuildServiceProvider();
        var dependent = container.GetService<DependentEnumerationService>();
        Assert.NotNull(dependent.Factories);
        Assert.IsType<SimpleService>(dependent.Factories.Single().CreateExportedValue());
    }

    [Fact]
    public void GetService_enumeration_of_exportFactory_multiple()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, SimpleService>(b => b.AllowMultiple = true);
        appServices.Add<IService, OptionalDependentService>(b => b.AllowMultiple = true);
        appServices.Add<DependentEnumerationService, DependentEnumerationService>();

        var container = appServices.BuildServiceProvider();
        var dependent = container.GetService<DependentEnumerationService>();
        Assert.NotNull(dependent.Factories);
        var factories = dependent.Factories.ToList();
        Assert.Equal(2, factories.Count);
        Assert.True(factories.Any(f => f.CreateExportedValue() is SimpleService));
        Assert.True(factories.Any(f => f.CreateExportedValue() is OptionalDependentService));
    }

    [Fact]
    public void GetService_exportFactory_with_priority_metadata()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(
            typeof(IDependency),
            () => Substitute.For<IDependency>(),
            b => b
                .AddMetadata("Priority", Priority.Low)
                .AddMetadata("IsOverride", true));

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<Lazy<IDependency, AppServiceMetadata>>();
        Assert.NotNull(service.Metadata);
        Assert.Equal(Priority.Low, service.Metadata.Priority);
        Assert.True(service.Metadata.IsOverride);
    }

    [Fact]
    public void GetService_exportFactory_with_metadata()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, DependentService>();

        var container = appServices.BuildServiceProvider();
        var service = container.GetRequiredService<IExportFactory<IService, AppServiceMetadata>>();
        Assert.NotNull(service.Metadata);
        Assert.Equal(Priority.High, service.Metadata.Priority);
        Assert.Equal(typeof(DependentService), service.Metadata.ServiceType);
    }

    [Fact]
    public void GetService_exportFactory_with_metadata_from_generic()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(new AppServiceInfo(
            new AppServiceInfo(typeof(IService<,>), typeof(GenericService)),
            typeof(IService),
            typeof(IService<,>),
            typeof(GenericService)));

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<IExportFactory<IService, AppServiceMetadata>>()!;
        Assert.NotNull(service.Metadata);
        Assert.Same(typeof(string), service.Metadata["ValueType"]);
        Assert.Same(typeof(int), service.Metadata["Type"]);
    }

    [Fact]
    public void GetService_lazy_with_metadata()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(typeof(IService), typeof(DependentService));
        appServices.Add<IDependency>(Substitute.For<IDependency>());

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<Lazy<IService, AppServiceMetadata>>();
        Assert.NotNull(service.Metadata);
        Assert.Equal(Priority.High, service.Metadata.Priority);
        Assert.Equal(typeof(DependentService), service.Metadata.ServiceType);
    }

    [Fact]
    public void GetService_lazy_with_metadata_generic()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, DependentService>();
        appServices.Add<IDependency>(Substitute.For<IDependency>());

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<Lazy<IService, AppServiceMetadata>>();
        Assert.NotNull(service.Metadata);
        Assert.Equal(Priority.High, service.Metadata.Priority);
        Assert.Equal(typeof(DependentService), service.Metadata.ServiceType);
    }

    [Fact]
    public void GetService_lazy_with_metadata_from_generic()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(new AppServiceInfo(
            new AppServiceInfo(typeof(IService<,>), typeof(GenericService)),
            typeof(IService),
            typeof(IService<,>),
            typeof(GenericService)));

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<Lazy<IService, AppServiceMetadata>>();
        Assert.NotNull(service.Metadata);
        Assert.Same(typeof(string), service.Metadata["ValueType"]);
        Assert.Same(typeof(int), service.Metadata["Type"]);
    }

    [Fact]
    public void GetService_two_levels()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add<IService, DependentService>();
        appServices.Add<IDependency, DependencyWithDependency>();
        appServices.Add<IAnotherDependency>(Substitute.For<IAnotherDependency>());

        var container = appServices.BuildServiceProvider();
        var service = container.GetService<IService>();
        Assert.NotNull(((DependencyWithDependency)((DependentService)service).Dependency).AnotherDependency);
    }

    [Fact]
    public void GetService_enumerable()
    {
        var appServices = Test.CreateTestServices();
        var logManager = Substitute.For<ILogManager>();
        appServices.Add(typeof(ILogManager), logManager);

        var container = appServices.BuildServiceProvider();
        var logManagerCollection = container.GetService<IEnumerable<ILogManager>>();
        Assert.Same(logManager, logManagerCollection.Single());
    }

    [Fact]
    public void GetAppServiceInfos_default_services()
    {
        var appServices = Test.CreateTestServices();
        appServices = new AppServiceCollectionBuilder(appServices).Build();
        var contracts = appServices.GetServiceInstance<IContractDeclarationCollection>();

        var (c, info) = contracts.Single(i => i.ContractDeclarationType == typeof(IServiceFactory));
        Assert.NotNull(info);
        Assert.Null(info.ContractType);
        Assert.Null(info.Metadata);
    }

    [Fact]
    public void GetAppServiceInfos_no_default_services()
    {
        IAppServiceCollection appServices = new AppServiceCollection();
        appServices = new AppServiceCollectionBuilder(appServices).Build();
        var contracts = appServices.GetServiceInstance<IContractDeclarationCollection>();

        contracts.Should().HaveCount(3);

        var appServicesDuplicate = appServices.GetServiceInstance<IAppServiceCollection>();
        appServicesDuplicate.Should().BeSameAs(appServices);
    }

    public interface ILogManager {}

    public interface IService { }
    public interface IDependency { }
    public interface IAnotherDependency { }
    public interface IService<TValue, TType> : IService { }

    public class DependentCollectionService
    {
        public DependentCollectionService(ICollection<IExportFactory<IService>> factories)
        {
            Factories = factories;
        }

        public ICollection<IExportFactory<IService>> Factories { get; }
    }

    public class DependentEnumerationService
    {
        public DependentEnumerationService(IEnumerable<IExportFactory<IService>> factories)
        {
            Factories = factories;
        }

        public IEnumerable<IExportFactory<IService>> Factories { get; }
    }

    public class SimpleService : IService { }

    [Priority(Priority.High)]
    public class DependentService : IService
    {
        public IDependency Dependency { get; }

        public DependentService(IDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public class OptionalDependentService : IService
    {
        public IDependency? Dependency { get; }

        public OptionalDependentService(IDependency? dependency = null)
        {
            Dependency = dependency;
        }
    }

    public class AmbiguousDependentService : IService
    {
        public IAnotherDependency AnotherDependency { get; }

        public IDependency Dependency { get; }

        public AmbiguousDependentService(IDependency dependency)
        {
            Dependency = dependency;
        }

        public AmbiguousDependentService(IAnotherDependency anotherDependency)
        {
            AnotherDependency = anotherDependency;
        }
    }

    public class GenericService : IService<string, int> { }

    public class DependencyWithDependency : IDependency
    {
        public IAnotherDependency AnotherDependency { get; }

        public DependencyWithDependency(IAnotherDependency dependency)
        {
            AnotherDependency = dependency;
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
            ServiceIntString = svcIntString;
            ServiceStringDouble = svcStringDouble;
        }

        public OpenGenericService<int, string> ServiceIntString { get; }

        public OpenGenericService<string, double> ServiceStringDouble { get; }
    }
}
