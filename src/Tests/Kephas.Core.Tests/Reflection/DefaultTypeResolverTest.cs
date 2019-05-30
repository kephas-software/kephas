// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default type resolver test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System;
    using System.Text;

    using Kephas.Logging;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultTypeResolverTest
    {
        [Test]
        public void ResolveType_DotNet_Native()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType("System.String");
            Assert.AreEqual(typeof(string), type);
        }

        [Test]
        public void ResolveType_AssemblyQualifiedName()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType(this.GetType().AssemblyQualifiedName);
            Assert.AreEqual(this.GetType(), type);
        }

        [Test]
        public void ResolveType_NotFound_Exception()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            Assert.That(() => resolver.ResolveType("bla bla", throwOnNotFound: true), Throws.InstanceOf<TypeLoadException>());
        }

        [Test]
        public void ResolveType_other_assembly_with_assembly_name()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType("Kephas.IAmbientServices, Kephas.Core");
            Assert.AreSame(typeof(IAmbientServices), type);
        }

        [Test]
        public void ResolveType_other_assembly_without_assembly_name()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType("Kephas.Interaction.ISignal");
            Assert.AreEqual("Kephas.Interaction.ISignal", type.FullName);
        }

        [Test]
        public void ResolveType_other_assembly_generic_without_assembly_name()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new DefaultTypeResolver(loader);
            var type = resolver.ResolveType("Kephas.Services.Behaviors.IServiceBehaviorContext`1[[Kephas.Interaction.ISignal]]");
            Assert.AreEqual("IServiceBehaviorContext`1", type.Name);
            Assert.AreEqual("ISignal", type.GenericTypeArguments[0].Name);
        }

        [Test]
        public void ResolveType_NotFound_Exception_not_thrown_no_log_if_no_assembly_name()
        {
            var loader = Substitute.For<IAssemblyLoader>();

            var sb = new StringBuilder();

            var logger = Substitute.For<ILogger<DefaultTypeResolver>>();
            logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => sb.Append(ci.Arg<LogLevel>()).Append(":").Append(ci.Arg<string>()));

            var resolver = new DefaultTypeResolver(loader) { Logger = logger };

            var type = resolver.ResolveType("bla bla", throwOnNotFound: false);
            Assert.IsNull(type);

            var log = sb.ToString();
            Assert.IsEmpty(log);
        }

        [Test]
        public void ResolveType_NotFound_Exception_not_thrown_log_if_bad_assembly_name()
        {
            var loader = new DefaultAssemblyLoader();

            var sb = new StringBuilder();

            var logger = Substitute.For<ILogger<DefaultTypeResolver>>();
            logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => sb.Append(ci.Arg<LogLevel>()).Append(":").Append(ci.Arg<string>()));

            var resolver = new DefaultTypeResolver(loader) { Logger = logger };

            var type = resolver.ResolveType("bla, bla", throwOnNotFound: false);
            Assert.IsNull(type);

            var log = sb.ToString();
            Assert.AreEqual("Warning:Errors occurred when trying to resolve type 'bla, bla'.", log);
        }
    }
}