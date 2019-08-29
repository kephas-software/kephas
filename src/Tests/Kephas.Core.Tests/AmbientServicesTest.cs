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
    using Kephas.Composition;
    using Kephas.Composition.Lightweight;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServices"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesTest
    {
        [Test]
        public void Register_instance_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.Register(typeof(ICompositionContext), (object)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void Register_factory_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.Register(typeof(ICompositionContext), (Func<object>)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void Register_service_instance()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void Register_service_type_singleton_dependency_resolved()
        {
            var ambientServices = new AmbientServices();
            var dependency = Substitute.For<IDependency>();
            ambientServices.Register(typeof(IService), typeof(DependentService));
            ambientServices.Register(typeof(IDependency), dependency);

            var service = (DependentService)ambientServices.GetService(typeof(IService));
            Assert.AreSame(dependency, service.Dependency);
        }

        [Test]
        public void Register_service_type_singleton_optional_dependency_resolved()
        {
            var ambientServices = new AmbientServices();
            var dependency = Substitute.For<IDependency>();
            ambientServices.Register(typeof(IService), typeof(OptionalDependentService));
            ambientServices.Register(typeof(IDependency), dependency);

            var service = (OptionalDependentService)ambientServices.GetService(typeof(IService));
            Assert.AreSame(dependency, service.Dependency);
        }

        [Test]
        public void Register_service_type_singleton_optional_dependency_not_resolved()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register(typeof(IService), typeof(OptionalDependentService));

            var service = (OptionalDependentService)ambientServices.GetService(typeof(IService));
            Assert.IsNull(service.Dependency);
        }

        [Test]
        public void Register_service_type_singleton_dependency_ambiguous()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register(typeof(IService), typeof(AmbiguousDependentService));
            ambientServices.Register(typeof(IDependency), Substitute.For<IDependency>());
            ambientServices.Register(typeof(IAnotherDependency), Substitute.For<IAnotherDependency>());

            Assert.Throws<AmbiguousMatchException>(() => ambientServices.GetService(typeof(IService)));
        }

        [Test]
        public void Register_service_type_singleton_dependency_non_ambiguous()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register(typeof(IService), typeof(AmbiguousDependentService));
            ambientServices.Register(typeof(IDependency), Substitute.For<IDependency>());

            var service = (AmbiguousDependentService)ambientServices.GetService(typeof(IService));
            Assert.IsNotNull(service.Dependency);
            Assert.IsNull(service.AnotherDependency);
        }

        [Test]
        public void Register_service_type_singleton()
        {
            var ambientServices = new AmbientServices();
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
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterTransient(typeof(IService), typeof(SimpleService));

            var service = ambientServices.GetService(typeof(IService));
            Assert.IsInstanceOf<SimpleService>(service);

            var sameService = ambientServices.GetService(typeof(IService));
            Assert.AreNotSame(service, sameService);
        }

        [Test]
        public void Register_conflicting_allow_multiple_first_single_failure()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService>(b => b.WithType<SimpleService>());
            Assert.Throws<InvalidOperationException>(() => ambientServices.Register<IService>(b => b.WithType<DependentService>().AllowMultiple()));
        }

        [Test]
        public void Register_conflicting_allow_multiple_first_multiple_failure()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService>(b => b.WithType<SimpleService>().AllowMultiple());
            Assert.Throws<InvalidOperationException>(() => ambientServices.Register<IService>(b => b.WithType<DependentService>()));
        }

        [Test]
        public void Register_service_factory()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), () => logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void Register_service_factory_non_singleton()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterTransient(typeof(ILogManager), () => Substitute.For<ILogManager>());
            var logManager1 = ambientServices.GetService(typeof(ILogManager));
            var logManager2 = ambientServices.GetService(typeof(ILogManager));
            Assert.AreNotSame(logManager1, logManager2);
        }

        [Test]
        public void Register_circular_dependency_singleton()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<CircularDependency1>(b => b.WithType<CircularDependency1>());
            ambientServices.Register<CircularDependency2>(b => b.WithType<CircularDependency2>());

            Assert.Throws<CircularDependencyException>(() => ambientServices.GetService<CircularDependency1>());
        }

        [Test]
        public void Register_circular_dependency_transient()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<CircularDependency1>(b => b.WithType<CircularDependency1>().AsTransient());
            ambientServices.Register<CircularDependency2>(b => b.WithType<CircularDependency2>().AsTransient());

            Assert.Throws<CircularDependencyException>(() => ambientServices.GetService<CircularDependency1>());
        }

        [Test]
        public Task Register_transient_is_multi_threaded()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService>(
                b => b.WithFactory(
                    () =>
                        {
                            Thread.Sleep(100);
                            return Substitute.For<IService>();
                        }).AsTransient());

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
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService>(
                b => b.WithFactory(
                    () =>
                        {
                            Thread.Sleep(100);
                            return Substitute.For<IService>();
                        }).AsSingleton());

            var tasks = new List<Task>();
            for (var i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => ambientServices.GetService<IService>()));
            }

            return Task.WhenAll(tasks);
        }

        [Test]
        public void GetService_exportFactory()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);

            var logManagerFactory = ambientServices.GetService<IExportFactory<ILogManager>>();
            Assert.AreSame(logManager, logManagerFactory.CreateExportedValue());
        }

        [Test]
        public void GetService_lazy()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);

            var logManagerFactory = ambientServices.GetService<Lazy<ILogManager>>();
            Assert.AreSame(logManager, logManagerFactory.Value);
        }

        [Test]
        public void GetService_collection_of_exportFactory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService, SimpleService>();
            ambientServices.Register<DependentCollectionService, DependentCollectionService>();

            var dependent = ambientServices.GetService<DependentCollectionService>();
            Assert.IsNotNull(dependent.Factories);
            Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
        }

        [Test]
        public void GetService_collection_of_lazy()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService, SimpleService>();
            ambientServices.Register<DependentCollectionService, DependentCollectionService>();

            var factories = ambientServices.GetService<IEnumerable<Lazy<IService>>>();
            Assert.IsInstanceOf<SimpleService>(factories.Single().Value);
        }

        [Test]
        public void GetService_enumeration_of_exportFactory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService, SimpleService>();
            ambientServices.Register<DependentEnumerationService, DependentEnumerationService>();

            var dependent = ambientServices.GetService<DependentEnumerationService>();
            Assert.IsNotNull(dependent.Factories);
            Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
        }

        [Test]
        public void GetService_enumeration_of_exportFactory_multiple()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService>(b => b.WithType<SimpleService>().AllowMultiple());
            ambientServices.Register<IService>(b => b.WithType<OptionalDependentService>().AllowMultiple());
            ambientServices.Register<DependentEnumerationService, DependentEnumerationService>();

            var dependent = ambientServices.GetService<DependentEnumerationService>();
            Assert.IsNotNull(dependent.Factories);
            var factories = dependent.Factories.ToList();
            Assert.AreEqual(2, factories.Count);
            Assert.IsTrue(factories.Any(f => f.CreateExportedValue() is SimpleService));
            Assert.IsTrue(factories.Any(f => f.CreateExportedValue() is OptionalDependentService));
        }

        [Test]
        public void GetService_exportFactory_with_metadata()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService, DependentService>();
            ambientServices.Register<IDependency>(Substitute.For<IDependency>());

            var service = ambientServices.GetService<IExportFactory<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreEqual((int)Priority.High, service.Metadata.OverridePriority);
            Assert.AreEqual(typeof(DependentService), service.Metadata.AppServiceImplementationType);
        }

        [Test]
        public void GetService_exportFactory_with_metadata_from_generic()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register(typeof(IService<,>), b => b.WithType<GenericService>().RegisterAs<IService>());

            var service = ambientServices.GetService<IExportFactory<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreSame(typeof(string), service.Metadata["ValueType"]);
            Assert.AreSame(typeof(int), service.Metadata["Type"]);
        }

#if NET45
#else
        [Test]
        public void GetService_lazy_with_metadata()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService, DependentService>();
            ambientServices.Register<IDependency>(Substitute.For<IDependency>());

            var service = ambientServices.GetService<Lazy<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreEqual((int)Priority.High, service.Metadata.OverridePriority);
            Assert.AreEqual(typeof(DependentService), service.Metadata.AppServiceImplementationType);
        }

        [Test]
        public void GetService_lazy_with_metadata_from_generic()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register(typeof(IService<,>), b => b.WithType<GenericService>().RegisterAs<IService>());

            var service = ambientServices.GetService<Lazy<IService, AppServiceMetadata>>();
            Assert.IsNotNull(service.Metadata);
            Assert.AreSame(typeof(string), service.Metadata["ValueType"]);
            Assert.AreSame(typeof(int), service.Metadata["Type"]);
        }
#endif

        [Test]
        public void GetService_two_levels()
        {
            var ambientServices = new AmbientServices();
            ambientServices.Register<IService, DependentService>();
            ambientServices.Register<IDependency, DependencyWithDependency>();
            ambientServices.Register<IAnotherDependency>(Substitute.For<IAnotherDependency>());

            var service = ambientServices.GetService<IService>();
            Assert.IsNotNull(((DependencyWithDependency)((DependentService)service).Dependency).AnotherDependency);
        }

        [Test]
        public void GetService_enumerable()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.Register(typeof(ILogManager), logManager);

            var logManagerCollection = ambientServices.GetService<IEnumerable<ILogManager>>();
            Assert.AreSame(logManager, logManagerCollection.Single());
        }

        [Test]
        public void CompositionContainer_works_fine_when_explicitely_set()
        {
            var ambientServices = new AmbientServices();
            var compositionContextMock = Substitute.For<ICompositionContext>();
            compositionContextMock.TryGetExport<ICompositionContext>(null).Returns((ICompositionContext)null);
            ambientServices.Register(compositionContextMock);
            var noService = ambientServices.CompositionContainer.TryGetExport<ICompositionContext>();
            Assert.IsNull(noService);
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
            public IDependency Dependency { get; }

            public OptionalDependentService(IDependency dependency = null)
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
    }
}