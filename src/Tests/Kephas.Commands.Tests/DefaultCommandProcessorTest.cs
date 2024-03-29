﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command processor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultCommandProcessorTest : CommandsTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.CreateServicesBuilder().BuildWithDependencyInjection();
            var processor = container.Resolve<ICommandProcessor>();

            Assert.IsInstanceOf<DefaultCommandProcessor>(processor);
        }

        [Test]
        public async Task ProcessAsync_return_object()
        {
            var contextFactory = this.CreateInjectableFactoryMock(() => new Context(Substitute.For<IServiceProvider>()));
            var cmdInfo = Substitute.For<IOperationInfo>();
            cmdInfo.Invoke(Arg.Any<object?>(), Arg.Any<IEnumerable<object?>>())
                .Returns(ci => ((IDynamic)ci.Arg<IEnumerable<object?>>().ToArray()[0])["hi"].ToString() + " gigi!");
            var resolver = Substitute.For<ICommandResolver>();
            resolver.ResolveCommand("help", Arg.Any<IDynamic>(), throwOnNotFound: true)
                .Returns(cmdInfo);
            var processor = new DefaultCommandProcessor(resolver, Substitute.For<ICommandIdentityResolver>(), contextFactory);

            var args = new Expando { ["hi"] = "there" };
            var result = await processor.ProcessAsync("help", args);
            Assert.AreEqual("there gigi!", result);
        }

        [Test]
        public async Task ProcessAsync_return_task()
        {
            var contextFactory = this.CreateInjectableFactoryMock(() => new Context(Substitute.For<IServiceProvider>()));
            var cmdInfo = Substitute.For<IOperationInfo>();
            cmdInfo.Invoke(Arg.Any<object?>(), Arg.Any<IEnumerable<object?>>())
                .Returns(ci => Task.FromResult<object?>(((IDynamic)ci.Arg<IEnumerable<object?>>().ToArray()[0])["hi"].ToString() + " gigi!"));
            var resolver = Substitute.For<ICommandResolver>();
            resolver.ResolveCommand("help", Arg.Any<IDynamic>(), throwOnNotFound: true)
                .Returns(cmdInfo);
            var processor = new DefaultCommandProcessor(resolver, Substitute.For<ICommandIdentityResolver>(), contextFactory);

            var args = new Expando { ["hi"] = "there" };
            var result = await processor.ProcessAsync("help", args);
            Assert.AreEqual("there gigi!", result);
        }
    }
}
