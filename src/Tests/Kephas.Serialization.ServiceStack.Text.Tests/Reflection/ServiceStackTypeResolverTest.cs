// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceStackTypeResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service stack type resolver test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Tests.Reflection
{
    using System;
    using System.Text;

    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Serialization.ServiceStack.Text.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ServiceStackTypeResolverTest
    {
        [Test]
        public void ResolveType_DotNet_Native()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new ServiceStackTypeResolver(loader);
            var type = resolver.ResolveType("System.String");
            Assert.AreEqual(typeof(string), type);
        }

        [Test]
        public void ResolveType_AssemblyQualifiedName()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new ServiceStackTypeResolver(loader);
            var type = resolver.ResolveType(this.GetType().AssemblyQualifiedName);
            Assert.AreEqual(this.GetType(), type);
        }

        [Test]
        public void ResolveType_FullName_generic_list_of_decimal()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new ServiceStackTypeResolver(loader);
            var type = resolver.ResolveType("System.Collections.Generic.List`1[[System.Decimal]]");
            Assert.AreEqual("List`1", type.Name);
            Assert.AreSame(typeof(decimal), type.GenericTypeArguments[0]);
        }

        [Test]
        public void ResolveType_FullName_generic_kephas_serializer()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new ServiceStackTypeResolver(loader);
            var type = resolver.ResolveType("Kephas.Serialization.ISerializer`1[[Kephas.Net.Mime.IMediaType]]");
            Assert.AreEqual("ISerializer`1", type.Name);
            Assert.AreSame(typeof(IMediaType), type.GenericTypeArguments[0]);
        }

        [Test]
        public void ResolveType_NotFound_Exception()
        {
            var loader = Substitute.For<IAssemblyLoader>();
            var resolver = new ServiceStackTypeResolver(loader);
            Assert.That(() => resolver.ResolveType("bla bla", throwOnNotFound: true), Throws.InstanceOf<TypeLoadException>());
        }

        [Test]
        public void ResolveType_NotFound_Exception_not_thrown_no_log_if_no_assembly_name()
        {
            var loader = Substitute.For<IAssemblyLoader>();

            var sb = new StringBuilder();

            var logger = Substitute.For<ILogger<DefaultTypeResolver>>();
            logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => sb.Append(ci.Arg<LogLevel>()).Append(":").Append(ci.Arg<string>()));

            var resolver = new ServiceStackTypeResolver(loader) { Logger = logger };

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

            var resolver = new ServiceStackTypeResolver(loader) { Logger = logger };

            var type = resolver.ResolveType("bla, bla", throwOnNotFound: false);
            Assert.IsNull(type);

            var log = sb.ToString();
            Assert.AreEqual("Warning:Errors occurred when trying to resolve type 'bla, bla'.", log);
        }
    }
}