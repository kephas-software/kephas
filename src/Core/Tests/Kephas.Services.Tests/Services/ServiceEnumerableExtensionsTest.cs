// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServicCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition helper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.Services.Metadata;

namespace Kephas.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services;
    using Xunit;

        public class ServiceEnumerableExtensionsTest
    {
        [Fact]
        public void OrderAsDictionary_empty()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>();

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.Equal(0, dict.Count);
        }

        [Fact]
        public void OrderAsDictionary_lazy_empty()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>();

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value);

            Assert.Equal(0, dict.Count);
        }

        [Fact]
        public void OrderAsDictionary_ordered()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service", dict["one"]);
            Assert.Equal("two service", dict["two"]);
        }

        [Fact]
        public void OrderAsDictionary_lazy_ordered()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one")),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "two")),
            };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service", dict["one"]);
            Assert.Equal("two service", dict["two"]);
        }

        [Fact]
        public void OrderAsDictionary_override_non_deterministic_siblings()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "one")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict["one"]);
            Assert.Equal("two service", dict["two"]);
        }

        [Fact]
        public void OrderAsDictionary_lazy_override_non_deterministic_siblings()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
                                {
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "one")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict["one"]);
            Assert.Equal("two service", dict["two"]);
        }

        [Fact]
        public void OrderAsDictionary_override_non_deterministic_siblings_with_key_comparer()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "One")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "oNe")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "onE")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue(), StringComparer.OrdinalIgnoreCase);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict["one"]);
            Assert.Equal("two service", dict["two"]);
        }

        [Fact]
        public void OrderAsDictionary_lazy_override_non_deterministic_siblings_with_key_comparer()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
                                {
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "One")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "oNe")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "onE")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value, StringComparer.OrdinalIgnoreCase);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict["one"]);
            Assert.Equal("two service", dict["two"]);
        }

        [Fact]
        public void OrderAsDictionary_override_non_deterministic_exception()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue()));
        }

        [Fact]
        public void OrderAsDictionary_lazy_override_non_deterministic_exception()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one")),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "two")),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one")),
            };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value));
        }

        [Fact]
        public void OrderAsDictionary_export_factory_override_non_deterministic_exception()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one")),
                                };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName));
        }

        [Fact]
        public void OrderAsDictionary_lazy_export_factory_override_non_deterministic_exception()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one")),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "two")),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one")),
            };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName));
        }

        [Fact]
        public void OrderAsDictionary_override_non_sortable_keys()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two") { ["Key"] = typeof(string) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"], f => f.CreateExportedValue());

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict[typeof(int)]);
            Assert.Equal("two service", dict[typeof(string)]);
        }

        [Fact]
        public void OrderAsDictionary_lazy_override_non_sortable_keys()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one") { ["Key"] = typeof(int) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "two") { ["Key"] = typeof(string) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "one") { ["Key"] = typeof(int) }),
            };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"], f => f.Value);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict[typeof(int)]);
            Assert.Equal("two service", dict[typeof(string)]);
        }

        [Fact]
        public void OrderAsDictionary_export_factory_override_non_sortable_keys()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            priority: (Priority)2,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "two") { ["Key"] = typeof(string) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            priority: (Priority)5,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"]);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict[typeof(int)].CreateExportedValue());
            Assert.Equal("two service", dict[typeof(string)].CreateExportedValue());
        }

        [Fact]
        public void OrderAsDictionary_lazy_export_factory_override_non_sortable_keys()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        priority: (Priority)2,
                        serviceName: "one") { ["Key"] = typeof(int) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "two") { ["Key"] = typeof(string) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        priority: (Priority)5,
                        serviceName: "one") { ["Key"] = typeof(int) }),
            };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"]);

            Assert.Equal(2, dict.Count);
            Assert.Equal("one service reloaded", dict[typeof(int)].Value);
            Assert.Equal("two service", dict[typeof(string)].Value);
        }
    }
}
