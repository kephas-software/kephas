// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunqConfiguratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the funq configurator test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Tests.Hosting.Configurators
{
    using System.Reflection;

    using Funq;

    using global::ServiceStack;

    using Kephas.ServiceStack.Composition;
    using Kephas.ServiceStack.Hosting;
    using Kephas.ServiceStack.Hosting.Configurators;

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