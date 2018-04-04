// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingModelRegistryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging model registry test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.Tests.Runtime.ModelRegistries
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging.Model.Elements;
    using Kephas.Messaging.Model.Runtime.ModelRegistries;
    using Kephas.Messaging.Ping;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    using IMessage = Kephas.Messaging.IMessage;

    [TestFixture]
    public class MessagingModelRegistryTest
    {
        [Test]
        public async Task GetRuntimeElementsAsync()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssembliesAsync(Arg.Any<Func<AssemblyName, bool>>(), Arg.Any<CancellationToken>())
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetLoadableExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IMessage), typeof(Model.IMessage), typeof(Message), typeof(string), typeof(PingMessage) });

            var registry = new MessagingModelRegistry(appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(typeof(PingMessage), result[0]);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_ExcludeFromModel()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssembliesAsync(Arg.Any<Func<AssemblyName, bool>>(), Arg.Any<CancellationToken>())
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetLoadableExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IMessage), typeof(Model.IMessage), typeof(Message), typeof(string), typeof(ExcludedMessage) });

            var registry = new MessagingModelRegistry(appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(0, result.Count);
        }

        [ExcludeFromModel]
        public class ExcludedMessage : IMessage
        {
        }
    }
}