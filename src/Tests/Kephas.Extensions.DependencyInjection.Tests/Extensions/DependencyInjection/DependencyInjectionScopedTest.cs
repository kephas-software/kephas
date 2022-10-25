// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionScopedTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Extensions.DependencyInjection
{
    using System.Diagnostics.CodeAnalysis;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacScopedServiceProvider"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DependencyInjectionScopedTest : TestBase
    {
        [Test]
        public void CreateScopedInjector_NestedScopes()
        {
            var builder = this.CreateServicesBuilder();
            builder.AmbientServices.Add(new AppServiceInfo(
                typeof(DependencyInjectionTest.ScopeExportedClass),
                typeof(DependencyInjectionTest.ScopeExportedClass),
                AppServiceLifetime.Scoped));
            var container = builder.BuildWithDependencyInjection();
            using var scopedContext = container.CreateScope();
            Assert.IsInstanceOf<ServiceProvider>(scopedContext);
            var scopedInstance1 = scopedContext.ServiceProvider.Resolve<DependencyInjectionTest.ScopeExportedClass>();

            using var nestedContext = scopedContext.ServiceProvider.CreateScope();
            Assert.AreNotSame(scopedContext, nestedContext);

            var scopedInstance2 = nestedContext.ServiceProvider.Resolve<DependencyInjectionTest.ScopeExportedClass>();
            Assert.AreNotSame(scopedInstance1, scopedInstance2);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_root()
        {
            var container = this.CreateServicesBuilder().BuildWithDependencyInjection();
            var selfContainer = container.Resolve<IServiceProvider>();
            Assert.AreSame(container, selfContainer);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_scope()
        {
            var container = this.CreateServicesBuilder().BuildWithDependencyInjection();
            using var scopedContext = container.CreateScope();
            var selfScopedContext = scopedContext.ServiceProvider.Resolve<IServiceProvider>();
            Assert.AreSame(selfScopedContext, scopedContext);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_scope_consumers()
        {
            var builder = this.CreateServicesBuilder();
            builder.AmbientServices.Add(new AppServiceInfo(
                typeof(DependencyInjectionTest.ScopeExportedClass),
                typeof(DependencyInjectionTest.ScopeExportedClass),
                AppServiceLifetime.Scoped));
            var container = builder.BuildWithDependencyInjection();
            using var scopedContext = container.CreateScope();
            var scopedInstance = scopedContext.ServiceProvider.Resolve<DependencyInjectionTest.ScopeExportedClass>();
            Assert.AreSame(scopedContext, scopedInstance.ServiceProvider);
        }
    }
}