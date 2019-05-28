// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition helper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data;
    using Kephas.Services.Composition;

    using NUnit.Framework;

    [TestFixture]
    public class CompositionHelperTest
    {
        [Test]
        public void ToDictionary_empty()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>();

            var dict = factories.ToPrioritizedDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void ToDictionary_ordered()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: 2,
                                            overridePriority: 1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: 5,
                                            overridePriority: 0,
                                            serviceName: "two")),
                                };

            var dict = factories.ToPrioritizedDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void ToDictionary_override_non_deterministic_siblings()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: 2,
                                            overridePriority: 1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service 2",
                                        new AppServiceMetadata(
                                            processingPriority: 2,
                                            overridePriority: 1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: 5,
                                            overridePriority: 0,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: 5,
                                            overridePriority: -1,
                                            serviceName: "one")),
                                };

            var dict = factories.ToPrioritizedDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue());

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict["one"]);
            Assert.AreEqual("two service", dict["two"]);
        }

        [Test]
        public void ToDictionary_override_non_deterministic_exception()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: 2,
                                            overridePriority: 1,
                                            serviceName: "one")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: 5,
                                            overridePriority: 0,
                                            serviceName: "two")),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: 2,
                                            overridePriority: 1,
                                            serviceName: "one")),
                                };

            Assert.Throws<DuplicateKeyException>(() => factories.ToPrioritizedDictionary(f => f.Metadata.ServiceName, f => f.CreateExportedValue()));
        }

        [Test]
        public void ToDictionary_override_non_sortable_keys()
        {
            var factories = new List<IExportFactory<string, AppServiceMetadata>>()
                                {
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service",
                                        new AppServiceMetadata(
                                            processingPriority: 2,
                                            overridePriority: 1,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "two service",
                                        new AppServiceMetadata(
                                            processingPriority: 5,
                                            overridePriority: 0,
                                            serviceName: "two") { ["Key"] = typeof(string) }),
                                    new ExportFactory<string, AppServiceMetadata>(
                                        () => "one service reloaded",
                                        new AppServiceMetadata(
                                            processingPriority: 5,
                                            overridePriority: -1,
                                            serviceName: "one") { ["Key"] = typeof(int) }),
                                };

            var dict = factories.ToPrioritizedDictionary(f => (Type)f.Metadata["Key"], f => f.CreateExportedValue());

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one service reloaded", dict[typeof(int)]);
            Assert.AreEqual("two service", dict[typeof(string)]);
        }
    }
}