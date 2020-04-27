// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Autofac
{
    using Kephas.Logging;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AmbientServicesTest : TestBase
    {
        [Test]
        public void CustomAmbientServices_singleton()
        {
            var ambientServices = CustomAmbientServices.CreateAmbientServices();
            var container = ambientServices
                .WithAppRuntime(this.CreateDefaultAppRuntime(Substitute.For<ILogManager>()))
                .BuildWithAutofac(b => b.WithPart(typeof(CustomAmbientServices))).CompositionContainer;
            var otherAmbientServices = container.GetExport<IAmbientServices>();

            Assert.AreSame(ambientServices, otherAmbientServices);
        }

        public class CustomAmbientServices : AmbientServices
        {
            private CustomAmbientServices()
            {

            }

            public static CustomAmbientServices CreateAmbientServices() => new CustomAmbientServices();
        }
    }
}
