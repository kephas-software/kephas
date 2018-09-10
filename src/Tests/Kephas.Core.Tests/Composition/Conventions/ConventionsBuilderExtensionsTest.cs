// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilderExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the conventions builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;

    using NUnit.Framework;

    /// <summary>
    /// The conventions builder extensions test.
    /// </summary>
    [TestFixture]
    public class ConventionsBuilderExtensionsTest
    {
        [Test]
        public void RegisterConventions_shared_derived_conventions_with_metadata()
        {
            var builder = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();
            var newBuilder = builder.RegisterConventions(
                new[] { typeof(CalculatorConventionsRegistrar) },
                new[] { typeof(ScientificCalculator), typeof(StandardCalculator) },
                new TestRegistrationContext());

            Assert.AreSame(builder, newBuilder);
            Assert.AreEqual(1, builder.DerivedConventionsBuilders.Count);
            var partBuilder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)builder.DerivedConventionsBuilders.First().Value;
            Assert.IsTrue(partBuilder.IsShared);
            Assert.AreEqual(typeof(ICalculator), partBuilder.Type);
            var exportBuilder = partBuilder.ExportBuilder;
            Assert.AreEqual(typeof(ICalculator), exportBuilder.ContractType);
            Assert.AreEqual(1, exportBuilder.Metadata.Count);
            Assert.AreEqual("type", exportBuilder.Metadata.Keys.First());
            var metadataExtractor = (Func<Type, object>)exportBuilder.Metadata.Values.First();
            Assert.AreEqual("Scientific", metadataExtractor(typeof(ScientificCalculator)));
            Assert.AreEqual("Classical", metadataExtractor(typeof(StandardCalculator)));
        }

        [Test]
        public void RegisterConventions_shared_derived_conventions_with_metadata_from_context_registrar()
        {
            var builder = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();
            var newBuilder = builder.RegisterConventions(
                new Type[0],
                new[] { typeof(ScientificCalculator), typeof(StandardCalculator) },
                new TestRegistrationContext
                {
                    Registrars = new IConventionsRegistrar[] { new CalculatorConventionsRegistrar() }
                });

            Assert.AreSame(builder, newBuilder);
            Assert.AreEqual(1, builder.DerivedConventionsBuilders.Count);
            var partBuilder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)builder.DerivedConventionsBuilders.First().Value;
            Assert.IsTrue(partBuilder.IsShared);
            Assert.AreEqual(typeof(ICalculator), partBuilder.Type);
            var exportBuilder = partBuilder.ExportBuilder;
            Assert.AreEqual(typeof(ICalculator), exportBuilder.ContractType);
            Assert.AreEqual(1, exportBuilder.Metadata.Count);
            Assert.AreEqual("type", exportBuilder.Metadata.Keys.First());
            var metadataExtractor = (Func<Type, object>)exportBuilder.Metadata.Values.First();
            Assert.AreEqual("Scientific", metadataExtractor(typeof(ScientificCalculator)));
            Assert.AreEqual("Classical", metadataExtractor(typeof(StandardCalculator)));
        }
    }

    public interface ICalculator
    {
    }

    public class CalculatorConventionsRegistrar : IConventionsRegistrar
    {
        public void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            builder
                .ForTypesDerivedFrom(typeof(ICalculator))
                .Export(
                    b => b.AsContractType(typeof(ICalculator))
                          .AddMetadata("type", t => t.Name.StartsWith("Scientific") ? "Scientific" : "Classical"))
                .Shared();
        }
    }

    public class ScientificCalculator : ICalculator { }

    public class StandardCalculator : ICalculator { }
}