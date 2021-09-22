// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilderExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the conventions builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Kephas.Injection.Conventions;
using Kephas.Injection.Hosting;
using Kephas.Injection.Lite;
using NUnit.Framework;

namespace Kephas.Core.Tests.Injection.Conventions
{
    /// <summary>
    /// The conventions builder extensions test.
    /// </summary>
    [TestFixture]
    public class ConventionsBuilderExtensionsTest
    {
        [Test]
        public void RegisterConventions_singleton_derived_conventions_with_metadata()
        {
            var builder = new InjectorBuilderBaseTest.TestConventionsBuilder();
            var newBuilder = builder.RegisterConventions(
                new[] { typeof(ScientificCalculator), typeof(StandardCalculator) },
                new TestBuildContext(new AmbientServices().Register<IConventionsRegistrar>(b => b.WithType<CalculatorConventionsRegistrar>().AllowMultiple())));

            Assert.AreSame(builder, newBuilder);
            Assert.AreEqual(1, builder.DerivedConventionsBuilders.Count);
            var partBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builder.DerivedConventionsBuilders.First().Value;
            Assert.IsTrue(partBuilder.IsSingleton);
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
        public void RegisterConventions_singleton_derived_conventions_with_metadata_from_context_registrar()
        {
            var builder = new InjectorBuilderBaseTest.TestConventionsBuilder();
            var newBuilder = builder.RegisterConventions(
                new[] { typeof(ScientificCalculator), typeof(StandardCalculator) },
                new TestBuildContext
                {
                    Registrars = new IConventionsRegistrar[] { new CalculatorConventionsRegistrar() }
                });

            Assert.AreSame(builder, newBuilder);
            Assert.AreEqual(1, builder.DerivedConventionsBuilders.Count);
            var partBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builder.DerivedConventionsBuilders.First().Value;
            Assert.IsTrue(partBuilder.IsSingleton);
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
        public void RegisterConventions(
            IConventionsBuilder builder,
            IList<Type> candidateTypes,
            IInjectionBuildContext buildContext)
        {
            builder
                .ForTypesDerivedFrom(typeof(ICalculator))
                .Export(
                    b => b.AsContractType(typeof(ICalculator))
                          .AddMetadata("type", t => t.Name.StartsWith("Scientific") ? "Scientific" : "Classical"))
                .Singleton();
        }
    }

    public class ScientificCalculator : ICalculator { }

    public class StandardCalculator : ICalculator { }
}