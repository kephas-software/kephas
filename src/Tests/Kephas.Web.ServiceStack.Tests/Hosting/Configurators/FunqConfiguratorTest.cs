// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunqConfiguratorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the funq configurator test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Tests.Hosting.Configurators
{
    using System.Linq;
    using System.Reflection;

    using Funq;

    using global::ServiceStack;

    using Kephas.Web.ServiceStack.Composition;
    using Kephas.Web.ServiceStack.Hosting;
    using Kephas.Web.ServiceStack.Hosting.Configurators;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class FunqConfiguratorTest
    {
        [Test]
        public void Configure()
        {
            var adapter = Substitute.For<IComposableContainerAdapter>();
            var configurator = new FunqConfigurator(adapter);
            var configContext = Substitute.For<IHostConfigurationContext>();
            var host = Substitute.For<ServiceStackHost>("name", new Assembly[0]);
            configContext.Host.Returns(host);

            var container = host.Container;
            if (container == null)
            {
                container = new Container();
                host.Container.Returns(container);
            }

            configurator.Configure(configContext);

            Assert.AreSame(adapter, container.Adapter);
        }
    }
}