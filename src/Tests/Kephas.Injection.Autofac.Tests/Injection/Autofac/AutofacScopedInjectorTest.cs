// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacScopedInjectorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection.Autofac
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Injection;
    using Kephas.Injection.Autofac;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Injection;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacScopedServiceProvider"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacScopedInjectorTest : AutofacInjectionTestBase
    {
        [Test]
        public void CreateScopedInjector_NestedScopes()
        {
            var container = this.CreateInjectorWithBuilder(
                b => b.WithRegistration(
                    new AppServiceInfo(
                        typeof(AutofacInjectorTest.ScopeExportedClass),
                        typeof(AutofacInjectorTest.ScopeExportedClass),
                        AppServiceLifetime.Scoped)));
            using var scopedContext = container.CreateScope();
            Assert.IsInstanceOf<AutofacScopedServiceProvider>(scopedContext);
            var scopedInstance1 = scopedContext.Resolve<AutofacInjectorTest.ScopeExportedClass>();

            using var nestedContext = scopedContext.CreateScopedInjector();
            Assert.AreNotSame(scopedContext, nestedContext);

            var scopedInstance2 = nestedContext.Resolve<AutofacInjectorTest.ScopeExportedClass>();
            Assert.AreNotSame(scopedInstance1, scopedInstance2);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_root()
        {
            var container = this.CreateInjectorWithBuilder();
            var selfContainer = container.Resolve<IServiceProvider>();
            Assert.AreSame(container, selfContainer);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_scope()
        {
            var container = this.CreateInjectorWithBuilder();
            using var scopedContext = container.CreateScope();
            var selfScopedContext = scopedContext.Resolve<IServiceProvider>();
            Assert.AreSame(selfScopedContext, scopedContext);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_scope_consumers()
        {
            var container = this.CreateInjectorWithBuilder(
                b => b.WithRegistration(
                    new AppServiceInfo(
                        typeof(AutofacInjectorTest.ScopeExportedClass),
                        typeof(AutofacInjectorTest.ScopeExportedClass),
                        AppServiceLifetime.Scoped)));
            using var scopedContext = container.CreateScope();
            var scopedInstance = scopedContext.Resolve<AutofacInjectorTest.ScopeExportedClass>();
            Assert.AreSame(scopedContext, scopedInstance.Injector);
        }
    }
}