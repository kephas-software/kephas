// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContextTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System.ComponentModel;

    using Kephas.Application;
    using Kephas.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppContextTest
    {
        [Test]
        public void Constructor_default_AppManifest_is_from_di_container()
        {
            var ambientServices = Substitute.For<IAmbientServices>();
            var container = Substitute.For<ICompositionContext>();
            var appManifest = Substitute.For<IAppManifest>();

            ambientServices.CompositionContainer.Returns(container);
            container.GetExport<IAppManifest>().Returns(appManifest);
            var appContext = new AppContext(ambientServices);
            Assert.AreSame(appManifest, appContext.AppManifest);
        }
    }
}