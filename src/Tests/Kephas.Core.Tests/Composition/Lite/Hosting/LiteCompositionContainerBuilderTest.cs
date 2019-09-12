// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteCompositionContainerBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite composition container builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition.Lite.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Composition.Internal;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;

    using NUnit.Framework;

    [TestFixture]
    public class LiteCompositionContainerBuilderTest
    {
        [Test]
        public void WithLiteCompositionContainer_app_manager()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(an => !this.IsTestAssembly(an));

            ambientServices.WithLiteCompositionContainer();

            Assert.IsInstanceOf<CompositionContextAdapter>(ambientServices.CompositionContainer);

            var appManager = ambientServices.CompositionContainer.GetExport<IAppManager>();
            Assert.IsInstanceOf<DefaultAppManager>(appManager);
        }

        [Test]
        public void WithLiteCompositionContainer_closed_generic()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.WithLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGeneric<>), typeof(IntGeneric) }));

            var service = ambientServices.GetService(typeof(IGeneric<int>));
            Assert.IsInstanceOf<IntGeneric>(service);
        }

        [Test]
        public void WithLiteCompositionContainer_closed_generic_dependency()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.WithLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGeneric<>), typeof(IntGeneric), typeof(IntGenericDepedent) }));

            var service = ambientServices.GetService<IntGenericDepedent>();
            Assert.IsNotNull(service);
        }

        [Test]
        public void WithLiteCompositionContainer_closed_generic_with_non_generic_contract_metadata()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.WithLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGenericSvc<>), typeof(INonGenericSvc), typeof(IntGenericSvc) }));

            var serviceFactory = ambientServices.GetService<IExportFactory<INonGenericSvc, GenericSvcMetadata>>();
            Assert.AreSame(typeof(int), serviceFactory.Metadata.ServiceType);
        }

        [Test]
        public void WithLiteCompositionContainer_disposable_closed_generic_with_non_generic_contract_metadata()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.WithLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGenericSvc<>), typeof(INonGenericSvc), typeof(DisposableIntGenericSvc) }));

            var serviceFactory = ambientServices.GetService<IExportFactory<INonGenericSvc, GenericSvcMetadata>>();
            Assert.AreSame(typeof(int), serviceFactory.Metadata.ServiceType);
        }

        private bool IsTestAssembly(AssemblyName assemblyName)
        {
            return assemblyName.Name.Contains("Test") || assemblyName.Name.Contains("NUnit") || assemblyName.Name.Contains("Mono") || assemblyName.Name.Contains("Proxy");
        }

        private bool IsAppAssembly(AssemblyName assemblyName)
        {
            return !this.IsTestAssembly(assemblyName) && !ReflectionHelper.IsSystemAssembly(assemblyName);
        }

        [AppServiceContract(AsOpenGeneric = false)]
        public interface IGeneric<T> { }

        public class IntGeneric : IGeneric<int> { }

        [AppServiceContract]
        public class IntGenericDepedent
        {
            public IntGenericDepedent(IGeneric<int> intGeneric) { }
        }

        public interface INonGenericSvc { }

        [AppServiceContract(ContractType = typeof(INonGenericSvc))]
        public interface IGenericSvc<TService> : INonGenericSvc { }

        public class IntGenericSvc : IGenericSvc<int> { }

        // The order of the interfaces is important, as a test case
        // includes finding the first generic interface
        public class DisposableIntGenericSvc : IDisposable, IGenericSvc<int>
        {
            public void Dispose() { }
        }

        public class GenericSvcMetadata : AppServiceMetadata
        {
            public GenericSvcMetadata(IDictionary<string, object> metadata) : base(metadata)
            {
                if (metadata == null) { return; }
                this.ServiceType = (Type)metadata.TryGetValue(nameof(this.ServiceType));
            }

            public Type ServiceType { get; }
        }
    }
}
