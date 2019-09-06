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
    using NUnit.Framework;

    [TestFixture]
    public class LiteCompositionContainerBuilderTest
    {
        [Test]
        public void WithLiteCompositionContainer_app_manager()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(an => !an.Name.Contains("Test") && !an.Name.Contains("NUnit") && !an.Name.Contains("Mono") && !an.Name.Contains("Castle"));

            ambientServices.WithLiteCompositionContainer();

            Assert.IsInstanceOf<CompositionContextAdapter>(ambientServices.CompositionContainer);

            var appManager = ambientServices.CompositionContainer.GetExport<IAppManager>();
            Assert.IsInstanceOf<DefaultAppManager>(appManager);
        }
    }
}
