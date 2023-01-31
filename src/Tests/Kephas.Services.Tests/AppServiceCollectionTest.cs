// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AppServiceCollection" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas;
    using Kephas.Reflection;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceCollection"/>.
    /// </summary>
    [TestFixture]
    public class AppServiceCollectionTest : AppServiceCollectionTestBase
    {
        protected override IServiceProvider BuildServiceProvider(IAppServiceCollection appServices)
        {
            return this.CreateServicesBuilder(appServices).BuildWithDependencyInjection();
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
    }
}