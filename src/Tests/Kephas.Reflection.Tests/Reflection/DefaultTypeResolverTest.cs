﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default type resolver test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultTypeResolverTest
    {
        [Test]
        public void ResolveType_DotNet_Native()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType("System.String");
            Assert.AreEqual(typeof(string), type);
        }

        [Test]
        public void ResolveType_DotNet_Native_only_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType("String");
            Assert.AreEqual(typeof(string), type);
        }

        [Test]
        public void ResolveType_AssemblyQualifiedName()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType(this.GetType().AssemblyQualifiedName);
            Assert.AreEqual(this.GetType(), type);
        }

        [Test]
        public void ResolveType_NotFound_Exception()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            Assert.That(() => resolver.ResolveType("bla bla", throwOnNotFound: true), Throws.InstanceOf<TypeLoadException>());
        }

        [Test]
        public void ResolveType_other_assembly_with_assembly_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType("Kephas.IAppServiceCollection, Kephas.Services");
            Assert.AreSame(typeof(IAppServiceCollection), type);
        }

        [Test]
        public void ResolveType_other_assembly_with_assembly_name_only_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType("IAppServiceCollection, Kephas.Services");
            Assert.AreSame(typeof(IAppServiceCollection), type);
        }

        [Test]
        public void ResolveType_other_assembly_with_assembly_name_only_name_ambiguous()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => new[] { this.GetType().Assembly };
            var resolver = new DefaultTypeResolver(getAssemblies);
            Assert.Throws<TypeLoadException>(() => resolver.ResolveType($"{nameof(One.TestSameName)}, {this.GetType().Assembly.GetName().Name}"));
        }

        [Test]
        public void ResolveType_other_assembly_without_assembly_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType("Kephas.IAdapter");
            Assert.AreEqual("Kephas.IAdapter", type.FullName);
        }

        [Test]
        public void ResolveType_other_assembly_without_assembly_name_only_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var type = resolver.ResolveType("IAdapter");
            Assert.AreEqual("Kephas.IAdapter", type.FullName);
        }

        [Test]
        public void ResolveType_other_assembly_without_assembly_name_only_name_ambiguous()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => new[] { this.GetType().Assembly };
            var resolver = new DefaultTypeResolver(getAssemblies);
            Assert.Throws<TypeLoadException>(() => resolver.ResolveType($"{nameof(One.TestSameName)}"));
        }

        [Test]
        public void ResolveType_other_assembly_generic_without_assembly_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name?.StartsWith("Kephas") ?? false)
                .ToList();
            var resolver = new DefaultTypeResolver(getAssemblies);
            var adapterFullName = typeof(IAdapter).FullName;
            var appServiceMetadataFullName = typeof(AppServiceMetadata).FullName;
            var type = resolver.ResolveType($"Kephas.Services.Behaviors.IServiceBehaviorContext`2[[{adapterFullName}],[{appServiceMetadataFullName}]]");

            Assert.AreEqual("IServiceBehaviorContext`2", type.Name);
            Assert.AreEqual("IAdapter", type.GenericTypeArguments[0].Name);
            Assert.AreEqual("AppServiceMetadata", type.GenericTypeArguments[1].Name);
        }

        [Test]
        public void ResolveType_other_assembly_generic_without_assembly_name_only_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();
            var resolver = new DefaultTypeResolver(getAssemblies);
            Assert.Throws<TypeLoadException>(() => resolver.ResolveType("IServiceBehaviorContext`2[[Kephas.Interaction.ISignal],[Kephas.Services.AppServiceMetadata]]"));
        }

        [Test]
        public void ResolveType_NotFound_Exception_not_thrown_no_log_if_no_assembly_name()
        {
            Func<IEnumerable<Assembly>> getAssemblies = () => AppDomain.CurrentDomain.GetAssemblies();

            var sb = new StringBuilder();

            var logger = Substitute.For<ILogger<DefaultTypeResolver>>();
            logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => sb.Append(ci.Arg<LogLevel>()).Append(":").Append(ci.Arg<string>()));

            var resolver = new DefaultTypeResolver(getAssemblies) { Logger = logger };

            var type = resolver.ResolveType("bla bla", throwOnNotFound: false);
            Assert.IsNull(type);

            var log = sb.ToString();
            Assert.IsEmpty(log);
        }

        [Test]
        public void ResolveType_NotFound_Exception_not_thrown_log_if_bad_assembly_name()
        {
            var sb = new StringBuilder();

            var logger = Substitute.For<ILogger<DefaultTypeResolver>>();
            logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => sb.Append(ci.Arg<LogLevel>()).Append(":").Append(ci.Arg<string>()).Append(ci.Arg<object[]>()[0]));

            var resolver = new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()) { Logger = logger };

            var type = resolver.ResolveType("bla, bla", throwOnNotFound: false);
            Assert.IsNull(type);

            var log = sb.ToString();
            Assert.AreEqual("Warning:Errors occurred when trying to resolve type '{type}'.bla, bla", log);
        }
    }

    namespace One
    {
        public class TestSameName { }
    }

    namespace Two
    {
        public class TestSameName { }
    }
}