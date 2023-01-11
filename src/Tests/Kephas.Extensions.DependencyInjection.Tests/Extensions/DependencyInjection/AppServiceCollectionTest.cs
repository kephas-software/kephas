// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Extensions.DependencyInjection
{
    using Kephas.Logging;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppServiceCollectionTest : TestBase
    {
        [Test]
        public void CustomAppServiceCollection_singleton()
        {
            var appServices = CustomAppServiceCollection.CreateAppServices();
            var container = new AppServiceCollectionBuilder(appServices)
                .WithAppRuntime(this.CreateDefaultAppRuntime(Substitute.For<ILogManager>()))
                .BuildWithDependencyInjection();
            var otherAppServices = container.Resolve<IAppServiceCollection>();

            Assert.AreSame(appServices, otherAppServices);
        }

        public class CustomAppServiceCollection : AppServiceCollection
        {
            private CustomAppServiceCollection()
            {
            }

            public static CustomAppServiceCollection CreateAppServices() => new();
        }
    }
}
