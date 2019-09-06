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
    using Kephas.Application;
    using Kephas.Composition.Internal;
    using Kephas.Reflection;
    using Kephas.Services;
    using NUnit.Framework;
    using System.Reflection;

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
    }
}
