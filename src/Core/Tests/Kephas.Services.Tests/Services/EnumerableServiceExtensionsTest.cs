// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Services
{
        public class EnumerableServiceExtensionsTest
    {
        [Fact]
        public void SelectServices_lazy_no_predicate()
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "x"),
                new(() => "123", "y"),
            };
            
            Assert.Equal(new[] { "abc", "123" }, services.SelectServices());
        }

        [Theory]
        [InlineData("a", new[] { "abc", "a123" })]
        [InlineData("ab", new[] { "abc" })]
        [InlineData("x", new string[0])]
        public void SelectServices_lazy_predicate(string start, string[] result)
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "x"),
                new(() => "a123", "y"),
            };
            
            Assert.Equal(result, services.SelectServices(s => s.Value.StartsWith(start)));
        }

        [Theory]
        [InlineData("m-a", "abc")]
        [InlineData("m-a1", "a123")]
        [InlineData("x", null)]
        public void TryGetService_lazy_predicate(string start, string? result)
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "m-abc"),
                new(() => "a123", "m-a123"),
            };
            
            Assert.Equal(result, services.TryGetService(m => m.StartsWith(start)));
        }

        [Theory]
        [InlineData("m-a", "abc")]
        [InlineData("m-a1", "a123")]
        public void GetService_lazy_predicate_found(string start, string? result)
        {
            var services = new Lazy<string, string>[]
            {
                new(() => "abc", "m-abc"),
                new(() => "a123", "m-a123"),
            };
            
            Assert.Equal(result, services.GetService(m => m.StartsWith(start)));
        }

        [Fact]
        public void GetService_lazy_predicate_notfound()
        {
            var services = Array.Empty<Lazy<string, string>>();
            
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
        }

        [Fact]
        public void SelectServices_factory_no_predicate()
        {
            var f1 = Substitute.For<IExportFactory<string, string>>();
            f1.CreateExportedValue().Returns("a", "b");
            var f2 = Substitute.For<IExportFactory<string, string>>();
            f2.CreateExportedValue().Returns("1", "2");

            var services = new[] { f1, f2 };
            
            Assert.Equal(new[] { "a", "1" }, services.SelectServices());
            Assert.Equal(new[] { "b", "2" }, services.SelectServices());
        }

        [Theory]
        [InlineData(new[] { "abc", "a123" }, "m-a", new[] { "abc", "a123" })]
        [InlineData(new[] { "abc", "a123" }, "m-ab", new[] { "abc" })]
        [InlineData(new[] { "abc", "a123" }, "m-x", new string[0])]
        public void SelectServices_factory_predicate(string[] values, string start, string[] result)
        {
            var services = values.Select(v =>
            {
                var f = Substitute.For<IExportFactory<string, string>>();
                f.CreateExportedValue().Returns(v, v + "-2");
                f.Metadata.Returns("m-" + v);
                return f;
            }).ToList();
            
            Assert.Equal(result, services.SelectServices(s => s.Metadata.StartsWith(start)));
            Assert.Equal(result.Select(v => v + "-2"), services.SelectServices(s => s.Metadata.StartsWith(start)));
        }

        [Theory]
        [InlineData(new[] { "abc", "a123" }, "m-a", "abc")]
        [InlineData(new[] { "abc", "a123" }, "m-a1", "a123")]
        [InlineData(new[] { "abc", "a123" }, "m-x", null)]
        public void TryGetService_factory_predicate(string[] values, string start, string? result)
        {
            var services = values.Select(v =>
            {
                var f = Substitute.For<IExportFactory<string, string>>();
                f.CreateExportedValue().Returns(v, v + "-2");
                f.Metadata.Returns("m-" + v);
                return f;
            }).ToList();
            
            Assert.Equal(result, services.TryGetService(m => m.StartsWith(start)));
            Assert.Equal(result is null ? null : result + "-2", services.TryGetService(m => m.StartsWith(start)));
        }

        [Theory]
        [InlineData(new[] { "abc", "a123" }, "m-a", "abc")]
        [InlineData(new[] { "abc", "a123" }, "m-a1", "a123")]
        public void GetService_factory_predicate_found(string[] values, string start, string? result)
        {
            var services = values.Select(v =>
            {
                var f = Substitute.For<IExportFactory<string, string>>();
                f.CreateExportedValue().Returns(v, v + "-2");
                f.Metadata.Returns("m-" + v);
                return f;
            }).ToList();
            
            Assert.Equal(result, services.GetService(m => m.StartsWith(start)));
            Assert.Equal(result is null ? null : result + "-2", services.GetService(m => m.StartsWith(start)));
        }

        [Fact]
        public void GetService_factory_predicate_notfound()
        {
            var services = Array.Empty<IExportFactory<string, string>>();
            
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
            Assert.Throws<InvalidOperationException>(() => services.GetService(m => m.StartsWith("x")));
        }
    }
}
