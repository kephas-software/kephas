// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.DependencyInjection;
using Kephas.Services;
using NSubstitute;
using NUnit.Framework;

namespace Kephas.Tests.Services
{
    [TestFixture]
    public class EnumerableServiceExtensionsTest
    {
        [Test]
        public void SelectServices_lazy_no_predicate()
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "x"),
                new(() => "123", "y"),
            };
            
            CollectionAssert.AreEqual(new[] { "abc", "123" }, services.SelectServices());
        }

        [Test]
        [TestCase("a", new[] { "abc", "a123" })]
        [TestCase("ab", new[] { "abc" })]
        [TestCase("x", new string[0])]
        public void SelectServices_lazy_predicate(string start, string[] result)
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "x"),
                new(() => "a123", "y"),
            };
            
            CollectionAssert.AreEqual(result, services.SelectServices(s => s.Value.StartsWith(start)));
        }

        [Test]
        [TestCase("m-a", "abc")]
        [TestCase("m-a1", "a123")]
        [TestCase("x", null)]
        public void TryGetService_lazy_predicate(string start, string? result)
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "m-abc"),
                new(() => "a123", "m-a123"),
            };
            
            Assert.AreEqual(result, services.TryGetService(m => m.StartsWith(start)));
        }

        [Test]
        [TestCase("m-a", "abc")]
        [TestCase("m-a1", "a123")]
        public void GetService_lazy_predicate_found(string start, string? result)
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "m-abc"),
                new(() => "a123", "m-a123"),
            };
            
            Assert.AreEqual(result, services.GetService(m => m.StartsWith(start)));
        }

        [Test]
        public void GetService_lazy_predicate_notfound()
        {
            var services = Array.Empty<Lazy<string, string>>();
            
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
        }

        [Test]
        public void SelectServices_factory_no_predicate()
        {
            var f1 = Substitute.For<IExportFactory<string, string>>();
            f1.CreateExportedValue().Returns("a", "b");
            var f2 = Substitute.For<IExportFactory<string, string>>();
            f2.CreateExportedValue().Returns("1", "2");

            var services = new[] { f1, f2 };
            
            CollectionAssert.AreEqual(new[] { "a", "1" }, services.SelectServices());
            CollectionAssert.AreEqual(new[] { "b", "2" }, services.SelectServices());
        }

        [Test]
        [TestCase(new[] { "abc", "a123" }, "m-a", new[] { "abc", "a123" })]
        [TestCase(new[] { "abc", "a123" }, "m-ab", new[] { "abc" })]
        [TestCase(new[] { "abc", "a123" }, "m-x", new string[0])]
        public void SelectServices_factory_predicate(string[] values, string start, string[] result)
        {
            var services = values.Select(v =>
            {
                var f = Substitute.For<IExportFactory<string, string>>();
                f.CreateExportedValue().Returns(v, v + "-2");
                f.Metadata.Returns("m-" + v);
                return f;
            }).ToList();
            
            CollectionAssert.AreEqual(result, services.SelectServices(s => s.Metadata.StartsWith(start)));
            CollectionAssert.AreEqual(result.Select(v => v + "-2"), services.SelectServices(s => s.Metadata.StartsWith(start)));
        }

        [Test]
        [TestCase(new[] { "abc", "a123" }, "m-a", "abc")]
        [TestCase(new[] { "abc", "a123" }, "m-a1", "a123")]
        [TestCase(new[] { "abc", "a123" }, "m-x", null)]
        public void TryGetService_factory_predicate(string[] values, string start, string? result)
        {
            var services = values.Select(v =>
            {
                var f = Substitute.For<IExportFactory<string, string>>();
                f.CreateExportedValue().Returns(v, v + "-2");
                f.Metadata.Returns("m-" + v);
                return f;
            }).ToList();
            
            Assert.AreEqual(result, services.TryGetService(m => m.StartsWith(start)));
            Assert.AreEqual(result is null ? null : result + "-2", services.TryGetService(m => m.StartsWith(start)));
        }

        [Test]
        [TestCase(new[] { "abc", "a123" }, "m-a", "abc")]
        [TestCase(new[] { "abc", "a123" }, "m-a1", "a123")]
        public void GetService_factory_predicate_found(string[] values, string start, string? result)
        {
            var services = values.Select(v =>
            {
                var f = Substitute.For<IExportFactory<string, string>>();
                f.CreateExportedValue().Returns(v, v + "-2");
                f.Metadata.Returns("m-" + v);
                return f;
            }).ToList();
            
            Assert.AreEqual(result, services.GetService(m => m.StartsWith(start)));
            Assert.AreEqual(result is null ? null : result + "-2", services.GetService(m => m.StartsWith(start)));
        }

        [Test]
        public void GetService_factory_predicate_notfound()
        {
            var services = Array.Empty<IExportFactory<string, string>>();
            
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
        }
    }
}