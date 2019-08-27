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

    using Kephas;
    using Kephas.Composition;
    using Kephas.Logging;

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
        public void RegisterService_instance_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.RegisterService(typeof(ICompositionContext), (object)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void RegisterService_factory_cannot_set_null()
        {
            var ambientServices = new AmbientServices();
            Assert.That(() => ambientServices.RegisterService(typeof(ICompositionContext), (Func<object>)null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void RegisterService_service_instance()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(ILogManager), logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void RegisterService_service_type_singleton_dependency_resolved()
        {
            var ambientServices = new AmbientServices();
            var dependency = Substitute.For<IDependency>();
            ambientServices.RegisterService(typeof(IService), typeof(DependentService), isSingleton: true);
            ambientServices.RegisterService(typeof(IDependency), dependency);

            var service = (DependentService)ambientServices.GetService(typeof(IService));
            Assert.AreSame(dependency, service.Dependency);
        }

        [Test]
        public void RegisterService_service_type_singleton_optional_dependency_resolved()
        {
            var ambientServices = new AmbientServices();
            var dependency = Substitute.For<IDependency>();
            ambientServices.RegisterService(typeof(IService), typeof(OptionalDependentService), isSingleton: true);
            ambientServices.RegisterService(typeof(IDependency), dependency);

            var service = (OptionalDependentService)ambientServices.GetService(typeof(IService));
            Assert.AreSame(dependency, service.Dependency);
        }

        [Test]
        public void RegisterService_service_type_singleton_optional_dependency_not_resolved()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService(typeof(IService), typeof(OptionalDependentService), isSingleton: true);

            var service = (OptionalDependentService)ambientServices.GetService(typeof(IService));
            Assert.IsNull(service.Dependency);
        }

        [Test]
        public void RegisterService_service_type_singleton_dependency_ambiguous()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService(typeof(IService), typeof(AmbiguousDependentService), isSingleton: true);
            ambientServices.RegisterService(typeof(IDependency), Substitute.For<IDependency>());
            ambientServices.RegisterService(typeof(IAnotherDependency), Substitute.For<IAnotherDependency>());

            Assert.Throws<AmbiguousMatchException>(() => ambientServices.GetService(typeof(IService)));
        }

        [Test]
        public void RegisterService_service_type_singleton_dependency_non_ambiguous()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService(typeof(IService), typeof(AmbiguousDependentService), isSingleton: true);
            ambientServices.RegisterService(typeof(IDependency), Substitute.For<IDependency>());

            var service = (AmbiguousDependentService)ambientServices.GetService(typeof(IService));
            Assert.IsNotNull(service.Dependency);
            Assert.IsNull(service.AnotherDependency);
        }

        [Test]
        public void RegisterService_service_type_singleton()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(IService), typeof(SimpleService), isSingleton: true);

            var service = ambientServices.GetService(typeof(IService));
            Assert.IsInstanceOf<SimpleService>(service);

            var sameService = ambientServices.GetService(typeof(IService));
            Assert.AreSame(service, sameService);
        }

        [Test]
        public void RegisterService_service_type_transient()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(IService), typeof(SimpleService), isSingleton: false);

            var service = ambientServices.GetService(typeof(IService));
            Assert.IsInstanceOf<SimpleService>(service);

            var sameService = ambientServices.GetService(typeof(IService));
            Assert.AreNotSame(service, sameService);
        }

        [Test]
        public void RegisterService_service_factory()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(ILogManager), () => logManager);
            Assert.AreSame(logManager, ambientServices.GetService(typeof(ILogManager)));
        }

        [Test]
        public void RegisterService_service_factory_non_singleton()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService(typeof(ILogManager), () => Substitute.For<ILogManager>());
            var logManager1 = ambientServices.GetService(typeof(ILogManager));
            var logManager2 = ambientServices.GetService(typeof(ILogManager));
            Assert.AreNotSame(logManager1, logManager2);
        }

        [Test]
        public void GetService_exportFactory()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(ILogManager), logManager);

            var logManagerFactory = ambientServices.GetService<IExportFactory<ILogManager>>();
            Assert.AreSame(logManager, logManagerFactory.CreateExportedValue());
        }

        [Test]
        public void GetService_collection_of_exportFactory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService<IService, SimpleService>();
            ambientServices.RegisterService<DependentCollectionService, DependentCollectionService>();

            var dependent = ambientServices.GetService<DependentCollectionService>();
            Assert.IsNotNull(dependent.Factories);
            Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
        }

        [Test]
        public void GetService_enumeration_of_exportFactory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService<IService, SimpleService>();
            ambientServices.RegisterService<DependentEnumerationService, DependentEnumerationService>();

            var dependent = ambientServices.GetService<DependentEnumerationService>();
            Assert.IsNotNull(dependent.Factories);
            Assert.IsInstanceOf<SimpleService>(dependent.Factories.Single().CreateExportedValue());
        }

        [Test]
        public void GetService_enumerable()
        {
            var ambientServices = new AmbientServices();
            var logManager = Substitute.For<ILogManager>();
            ambientServices.RegisterService(typeof(ILogManager), logManager);

            var logManagerCollection = ambientServices.GetService<IEnumerable<ILogManager>>();
            Assert.AreSame(logManager, logManagerCollection.Single());
        }

        [Test]
        public void CompositionContainer_works_fine_when_explicitely_set()
        {
            var ambientServices = new AmbientServices();
            var compositionContextMock = Substitute.For<ICompositionContext>();
            compositionContextMock.TryGetExport<ICompositionContext>(null).Returns((ICompositionContext)null);
            ambientServices.RegisterService(compositionContextMock);
            var noService = ambientServices.CompositionContainer.TryGetExport<ICompositionContext>();
            Assert.IsNull(noService);
        }

        public interface IService { }
        public interface IDependency { }
        public interface IAnotherDependency { }

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
    }
}