// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServicCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition helper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Kephas.Data;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceEnumerableExtensionsTest
    {
        [Test]
        public void OrderAsDictionary_empty()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>();

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void OrderAsDictionary_lazy_empty()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>();

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value);

            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void OrderAsDictionary_ordered()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void OrderAsDictionary_lazy_ordered()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one")),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)0,
                        serviceName: "two")),
            };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void OrderAsDictionary_override_non_deterministic_siblings()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)(-1),
                                            serviceName: "one")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void OrderAsDictionary_lazy_override_non_deterministic_siblings()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
                                {
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)(-1),
                                            serviceName: "one")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void OrderAsDictionary_override_non_deterministic_siblings_with_key_comparer()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "One")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "oNe")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)(-1),
                                            serviceName: "onE")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue(), StringComparer.OrdinalIgnoreCase);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void OrderAsDictionary_lazy_override_non_deterministic_siblings_with_key_comparer()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
                                {
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "One")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "oNe")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                    new Lazy<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)(-1),
                                            serviceName: "onE")),
                                };

            var dict = factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value, StringComparer.OrdinalIgnoreCase);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void OrderAsDictionary_override_non_deterministic_exception()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue()));
        }

        [Test]
        public void OrderAsDictionary_lazy_override_non_deterministic_exception()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one")),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)0,
                        serviceName: "two")),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one")),
            };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName, f => f.Value));
        }

        [Test]
        public void OrderAsDictionary_export_factory_override_non_deterministic_exception()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one")),
                                };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName));
        }

        [Test]
        public void OrderAsDictionary_lazy_export_factory_override_non_deterministic_exception()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one")),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)0,
                        serviceName: "two")),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one")),
            };

            Assert.Throws<DuplicateKeyException>(() => factories.OrderAsDictionary(f => f.Metadata.ServiceName));
        }

        [Test]
        public void OrderAsDictionary_override_non_sortable_keys()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two") { ["Key"] = typeof(string) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)(-1),
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"], f => f.CreateExportedValue());

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict[typeof(int)]);
            Assert.AreEqual("two service", dict[typeof(string)]);
        }

        [Test]
        public void OrderAsDictionary_lazy_override_non_sortable_keys()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one") { ["Key"] = typeof(int) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)0,
                        serviceName: "two") { ["Key"] = typeof(string) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)(-1),
                        serviceName: "one") { ["Key"] = typeof(int) }),
            };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"], f => f.Value);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict[typeof(int)]);
            Assert.AreEqual("two service", dict[typeof(string)]);
        }

        [Test]
        public void OrderAsDictionary_export_factory_override_non_sortable_keys()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)2,
                                            overridePriority: (Priority)1,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)0,
                                            serviceName: "two") { ["Key"] = typeof(string) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: (Priority)5,
                                            overridePriority: (Priority)(-1),
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"]);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict[typeof(int)].CreateExportedValue());
            Assert.AreEqual("two service", dict[typeof(string)].CreateExportedValue());
        }

        [Test]
        public void OrderAsDictionary_lazy_export_factory_override_non_sortable_keys()
        {
            var factories = new List<Lazy<string, AppServiceMetadata>>()
            {
                new Lazy<string, AppServiceMetadata>(
                    () => "one service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)2,
                        overridePriority: (Priority)1,
                        serviceName: "one") { ["Key"] = typeof(int) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "two service",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)0,
                        serviceName: "two") { ["Key"] = typeof(string) }),
                new Lazy<string, AppServiceMetadata>(
                    () => "one service reloaded",
                    new AppServiceMetadata(
                        processingPriority: (Priority)5,
                        overridePriority: (Priority)(-1),
                        serviceName: "one") { ["Key"] = typeof(int) }),
            };

            var dict = factories.OrderAsDictionary(f => (Type)f.Metadata["Key"]);

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict[typeof(int)].Value);
            Assert.AreEqual("two service", dict[typeof(string)].Value);
        }
    }
}