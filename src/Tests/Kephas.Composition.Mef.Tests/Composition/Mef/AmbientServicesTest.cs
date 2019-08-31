// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Mef
{
    using Kephas.Application;
    using Kephas.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class AmbientServicesTest
    {
        [Test]
        public void CustomAmbientServices_singleton()
        {
            var ambientServices = CustomAmbientServices.CreateAmbientServices();
            var container = ambientServices
                .WithAppRuntime(new DefaultAppRuntime(assemblyFilter: n => !n.IsSystemAssembly() && !n.Name.Contains("JetBrains") && !n.Name.Contains("NUnit") && !n.Name.Contains("Test")))
                .WithMefCompositionContainer(b => b.WithPart(typeof(CustomAmbientServices))).CompositionContainer;
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
