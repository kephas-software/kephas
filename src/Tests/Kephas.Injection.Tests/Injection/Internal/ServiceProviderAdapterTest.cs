// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderAdapterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector service provider adapter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection;
    using Kephas.Injection.Internal;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceProviderAdapterTest
    {
        [Test]
        public void GetService_success()
        {
            var context = Substitute.For<IInjector>();
            context.TryResolve(typeof(string)).Returns("hello");
            var adapter = new ServiceProviderAdapter(context);

            var result = (string?)adapter.GetService(typeof(string));
            Assert.AreEqual("hello", result);
        }

        [Test]
        public void GetService_not_found()
        {
            var context = Substitute.For<IInjector>();
            context.TryResolve(typeof(string)).Returns(null);
            var adapter = new ServiceProviderAdapter(context);

            var result = (string?)adapter.GetService(typeof(string));
            Assert.IsNull(result);
        }

        [Test]
        public void GetService_Collection_string_success()
        {
            var context = Substitute.For<IInjector>();
            context.TryResolve(typeof(ICollection<string>)).Returns(new[] { "hello", "world" });
            var adapter = new ServiceProviderAdapter(context);

            var result = (ICollection<string>?)adapter.GetService(typeof(ICollection<string>))!;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Skip(1).First());
        }
    }
}