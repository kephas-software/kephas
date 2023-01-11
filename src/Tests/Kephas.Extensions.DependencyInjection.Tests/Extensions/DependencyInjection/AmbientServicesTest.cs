// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Extensions.DependencyInjection
{
    using Kephas.Logging;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AmbientServicesTest : TestBase
    {
        [Test]
        public void CustomAmbientServices_singleton()
        {
            var ambientServices = CustomAppServiceCollection.CreateAmbientServices();
            var container = new AppServiceCollectionBuilder(ambientServices)
                .WithAppRuntime(this.CreateDefaultAppRuntime(Substitute.For<ILogManager>()))
                .BuildWithDependencyInjection();
            var otherAmbientServices = container.Resolve<IAmbientServices>();

            Assert.AreSame(ambientServices, otherAmbientServices);
        }

        public class CustomAppServiceCollection : AppServiceCollection
        {
            private CustomAppServiceCollection()
            {
            }

            public static CustomAppServiceCollection CreateAmbientServices() => new CustomAppServiceCollection();
        }
    }
}
