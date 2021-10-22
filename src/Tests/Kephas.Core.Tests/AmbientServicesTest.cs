// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServices" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Injection;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServices"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesTest : TestBase
    {
        [Test]
        public void Register_instance_cannot_set_null()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            Assert.That(() => ambientServices.Register(typeof(IInjector), (object)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void Register_factory_cannot_set_null()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            Assert.That(() => ambientServices.Register(typeof(IInjector), (Func<IInjector, object>)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void Register_service_instance()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void Register_service_type_singleton_dependency_resolved()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var dependency = Substitute.For<IDependency>();
            ambientServices.Register(typeof(IService), typeof(DependentService));
            ambientServices.Register(typeof(IDependency), dependency);

            var service = (DependentService)ambientServices.GetService(typeof(IService));
            Assert.AreSame(dependency, service.Dependency);
        }

        [Test]
        public void Register_service_type_singleton_optional_dependency_resolved()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var dependency = Substitute.For<IDependency>();
            ambientServices.Register(typeof(IService), typeof(OptionalDependentService));
            ambientServices.Register(typeof(IDependency), dependency);

            var service = (OptionalDependentService)ambientServices.GetService(typeof(IService));
            Assert.AreSame(dependency, service.Dependency);
        }

        [Test]
        public void Register_service_type_singleton_optional_dependency_not_resolved()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register(typeof(IService), typeof(OptionalDependentService));

            var service = (OptionalDependentService)ambientServices.GetService(typeof(IService));
            Assert.IsNull(service.Dependency);
        }

        [Test]
        public void Register_service_type_singleton_dependency_ambiguous()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register(typeof(IService), typeof(AmbiguousDependentService));
            ambientServices.Register(typeof(IDependency), Substitute.For<IDependency>());
            ambientServices.Register(typeof(IAnotherDependency), Substitute.For<IAnotherDependency>());

            Assert.Throws<AmbiguousMatchException>(() => ambientServices.GetService(typeof(IService)));
        }

        [Test]
        public void Register_service_type_singleton_dependency_non_ambiguous()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register(typeof(IService), typeof(AmbiguousDependentService));
            ambientServices.Register(typeof(IDependency), Substitute.For<IDependency>());

            var service = (AmbiguousDependentService)ambientServices.GetService(typeof(IService));
            Assert.IsNotNull(service.Dependency);
            Assert.IsNull(service.AnotherDependency);
        }

        [Test]
        public void Register_service_type_singleton()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(IService), typeof(SimpleService));

            var service = ambientServices.GetService(typeof(IService));
            Assert.IsInstanceOf<SimpleService>(service);

            var sameService = ambientServices.GetService(typeof(IService));
            Assert.AreSame(service, sameService);
        }

        [Test]
        public void Register_service_type_transient()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(IService), typeof(SimpleService), b => b.Transient());

            var service = ambientServices.GetService(typeof(IService));
            Assert.IsInstanceOf<SimpleService>(service);

            var sameService = ambientServices.GetService(typeof(IService));
            Assert.AreNotSame(service, sameService);
        }

        [Test]
        public void Register_conflicting_allow_multiple_first_single_failure()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService>(typeof(SimpleService));
            Assert.Throws<InvalidOperationException>(() => ambientServices.Register<IService, DependentService>(b => b.AllowMultiple()));
        }

        [Test]
        public void Register_conflicting_allow_multiple_first_multiple_failure()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService>(typeof(SimpleService), b => b.AllowMultiple());
            Assert.Throws<InvalidOperationException>(() => ambientServices.Register<IService, DependentService>());
        }

        [Test]
        public void Register_service_factory()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), () => logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void Register_service_factory_non_singleton()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register(typeof(ILogManager), () => Substitute.For<ILogManager>(), b => b.Transient());
            var logManager1 = ambientServices.GetService(typeof(ILogManager));
            var logManager2 = ambientServices.GetService(typeof(ILogManager));
            Assert.AreNotSame(logManager1, logManager2);
        }

        [Test]
        public void Register_circular_dependency_singleton()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<CircularDependency1, CircularDependency1>();
            ambientServices.Register<CircularDependency2, CircularDependency2>();

            Assert.Throws<CircularDependencyException>(() => ambientServices.GetService<CircularDependency1>());
        }

        [Test]
        public void Register_circular_dependency_transient()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<CircularDependency1, CircularDependency1>(b => b.Transient());
            ambientServices.Register<CircularDependency2, CircularDependency2>(b => b.Transient());

            Assert.Throws<CircularDependencyException>(() => ambientServices.GetService<CircularDependency1>());
        }

        [Test]
        public Task Register_transient_is_multi_threaded()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService>(
                () =>
                {
                    Thread.Sleep(100);
                    return Substitute.For<IService>();
                },
                b => b.Transient());

            var tasks = new List<Task>();
            for (var i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => ambientServices.GetService<IService>()));
            }

            return Task.WhenAll(tasks);
        }

        [Test]
        public Task Register_singleton_is_multi_threaded()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService>(
                () =>
                {
                    Thread.Sleep(100);
                    return Substitute.For<IService>();
                },
                b => b.Singleton());

            var tasks = new List<Task>();
            for (var i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => ambientServices.GetService<IService>()));
            }

            return Task.WhenAll(tasks);
        }

        [Test]
        public void Register_service_open_generic()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(OpenGenericService<,>), typeof(OpenGenericService<,>));

            var service = ambientServices.GetService(typeof(OpenGenericService<string, int>));
            Assert.IsInstanceOf<OpenGenericService<string, int>>(service);
        }

        [Test]
        public void Register_service_open_generic_dependency()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(OpenGenericService<,>), typeof(OpenGenericService<,>));
            ambientServices.Register<OpenGenericDependency, OpenGenericDependency>();

            var service = ambientServices.GetService<OpenGenericDependency>();
            Assert.IsNotNull(service.ServiceIntString);
            Assert.IsNotNull(service.ServiceStringDouble);
        }

        [Test]
        public void GetService_exportFactory()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);

            var logManagerFactory = ambientServices.GetService<IExportFactory<ILogManager>>();
            Assert.AreSame(logManager, logManagerFactory.CreateExportedValue());
        }

        [Test]
        public void GetService_lazy()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);

            var logManagerFactory = ambientServices.GetService<Lazy<ILogManager>>();
            Assert.AreSame(logManager, logManagerFactory.Value);
        }

        [Test]
        public void GetService_collection_of_exportFactory()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, SimpleService>();
            ambientServices.Register<DependentCollectionService, DependentCollectionService>();

            var dependent = ambientServices.GetService<DependentCollectionService>();
            Assert.IsNotNull(dependent.Factories);
            Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
        }

        [Test]
        public void GetService_collection_of_lazy()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, SimpleService>();
            ambientServices.Register<DependentCollectionService, DependentCollectionService>();

            var factories = ambientServices.GetService<IEnumerable<Lazy<IService>>>();
            Assert.IsInstanceOf<SimpleService>(factories.Single().Value);
        }

        [Test]
        public void GetService_enumeration_of_exportFactory()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, SimpleService>();
            ambientServices.Register<DependentEnumerationService, DependentEnumerationService>();

            var dependent = ambientServices.GetService<DependentEnumerationService>();
            Assert.IsNotNull(dependent.Factories);
            Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
        }

        [Test]
        public void GetService_enumeration_of_exportFactory_multiple()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, SimpleService>(b => b.AllowMultiple());
            ambientServices.Register<IService, OptionalDependentService>(b => b.AllowMultiple());
            ambientServices.Register<DependentEnumerationService, DependentEnumerationService>();

            var dependent = ambientServices.GetService<DependentEnumerationService>();
            Assert.IsNotNull(dependent.Factories);
            var factories = dependent.Factories.ToList();
            Assert.AreEqual(2, factories.Count);
            Assert.IsTrue(factories.Any(f => f.CreateExportedValue() is SimpleService));
            Assert.IsTrue(factories.Any(f => f.CreateExportedValue() is OptionalDependentService));
        }

        [Test]
        public void GetService_exportFactory_with_priority_metadata()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register(
                typeof(IDependency),
                () => Substitute.For<IDependency>(),
                b => b
                    .ProcessingPriority(Priority.Low)
                    .OverridePriority(Priority.AboveNormal)
                    .IsOverride());

            var service = ambientServices.GetService<Lazy<IDependency, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreEqual(Priority.Low, service.Metadata.ProcessingPriority);
            Assert.AreEqual(Priority.AboveNormal, service.Metadata.OverridePriority);
            Assert.IsTrue(service.Metadata.IsOverride);
        }

        [Test]
        public void GetService_exportFactory_with_metadata()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, DependentService>();

            var service = ambientServices.GetRequiredService<IExportFactory<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreEqual(Priority.High, service.Metadata.OverridePriority);
            Assert.AreEqual(typeof(DependentService), service.Metadata.ServiceType);
        }

        [Test]
        public void GetService_exportFactory_with_metadata_from_generic()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.RegisterService(typeof(IService<,>), typeof(GenericService), b => b.As<IService>());

            var service = ambientServices.GetService<IExportFactory<IService, AppServiceMetadata>>()!;
            Assert.IsNotNull(service.Metadata);
            Assert.AreSame(typeof(string), service.Metadata["ValueType"]);
            Assert.AreSame(typeof(int), service.Metadata["Type"]);
        }

        [Test]
        public void GetService_lazy_with_metadata()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, DependentService>();
            ambientServices.Register<IDependency>(Substitute.For<IDependency>());

            var service = ambientServices.GetService<Lazy<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreEqual(Priority.High, service.Metadata.OverridePriority);
            Assert.AreEqual(typeof(DependentService), service.Metadata.ServiceType);
        }

        [Test]
        public void GetService_lazy_with_metadata_from_generic()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.RegisterService(typeof(IService<,>), typeof(GenericService), b => b.As<IService>());

            var service = ambientServices.GetService<Lazy<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreSame(typeof(string), service.Metadata["ValueType"]);
            Assert.AreSame(typeof(int), service.Metadata["Type"]);
        }

        [Test]
        public void GetService_two_levels()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.Register<IService, DependentService>();
            ambientServices.Register<IDependency, DependencyWithDependency>();
            ambientServices.Register<IAnotherDependency>(Substitute.For<IAnotherDependency>());

            var service = ambientServices.GetService<IService>();
            Assert.IsNotNull(((DependencyWithDependency)((DependentService)service).Dependency).AnotherDependency);
        }

        [Test]
        public void GetService_enumerable()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);

            var logManagerCollection = ambientServices.GetService<IEnumerable<ILogManager>>();
            Assert.AreSame(logManager, logManagerCollection.Single());
        }

        [Test]
        public void InjectionContainer_works_fine_when_explicitely_set()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var injector = Substitute.For<IInjector>();
            injector.TryResolve<IInjector>().Returns((IInjector)null);
            ambientServices.Register(injector);
            var noService = ambientServices.Injector.TryResolve<IInjector>();
            Assert.IsNull(noService);
        }

        [Test]
        public void GetAppServiceInfos_default_services()
        {
            var ambientServices = (AmbientServices)this.CreateAmbientServices().WithStaticAppRuntime();
            var appServiceInfos = ambientServices.GetAppServiceInfos(new List<Type>());

            var (c, info) = appServiceInfos.SingleOrDefault(i => i.contractDeclarationType == typeof(ILogManager));
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.InstanceFactory);

            (c, info) = appServiceInfos.SingleOrDefault(i => i.contractDeclarationType == typeof(IAppRuntime));
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.InstanceFactory);
        }

        [Test]
        public void GetAppServiceInfos_no_default_services()
        {
            var ambientServices = new AmbientServices(registerDefaultServices: false);
            var appServiceInfos = ambientServices.GetAppServiceInfos(new List<Type>());

            Assert.AreEqual(2, appServiceInfos.Count());

            var (c, info) = appServiceInfos.SingleOrDefault(i => i.contractDeclarationType == typeof(IAmbientServices));
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.InstanceFactory);
            Assert.AreSame(ambientServices, info.InstanceFactory(null));

            (c, info) = appServiceInfos.SingleOrDefault(i => i.contractDeclarationType == typeof(IRuntimeTypeRegistry));
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.InstanceFactory);
            Assert.AreSame(RuntimeTypeRegistry.Instance, info.InstanceFactory(null));
        }

        [Test]
        public void GetAppServiceInfos_no_services_for_lite_injection()
        {
            var ambientServices = (AmbientServices)this.CreateAmbientServices();
            ambientServices[LiteInjectorBuilder.LiteInjectionKey] = true;
            var appServiceInfos = ambientServices.GetAppServiceInfos(new List<Type>());

            Assert.IsFalse(appServiceInfos.Any());
        }

        [Test]
        public void GetAppServiceInfos_all_services_for_lite_injection_when_null_registration_context()
        {
            var ambientServices = (AmbientServices)this.CreateAmbientServices().WithStaticAppRuntime();
            ambientServices[LiteInjectorBuilder.LiteInjectionKey] = true;
            var appServiceInfos = ambientServices.GetAppServiceInfos(null);

            var (c, info) = appServiceInfos.SingleOrDefault(i => i.contractDeclarationType == typeof(IAppRuntime));
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.InstanceFactory);
            Assert.AreSame(((IAmbientServices)ambientServices).GetAppRuntime(), info.InstanceFactory(null));
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
}