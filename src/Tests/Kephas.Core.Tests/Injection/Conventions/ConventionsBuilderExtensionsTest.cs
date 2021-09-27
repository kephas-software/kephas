// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilderExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the conventions builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using NUnit.Framework;

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
            var newBuilder = ((IConventionsBuilder)builder).RegisterConventions(
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new CalculatorAppServiceInfosProvider()));

            Assert.AreSame(builder, newBuilder);
            Assert.AreEqual(1, builder.DerivedConventionsBuilders.Count);
            var partBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builder.DerivedConventionsBuilders.First().Value;
            Assert.IsTrue(partBuilder.IsSingleton);
            Assert.AreEqual(typeof(ICalculator), partBuilder.Type);
            var exportBuilder = partBuilder;
            Assert.AreEqual(typeof(ICalculator), exportBuilder.ServiceType);
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

    public class CalculatorAppServiceInfosProvider : IAppServiceInfosProvider
    {
        public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
        {
            yield return (
                typeof(ICalculator),
                new AppServiceInfo(typeof(ICalculator), typeof(ScientificCalculator))
                    .AddMetadata("type", "Scientific"));

            yield return (
                typeof(ICalculator),
                new AppServiceInfo(typeof(ICalculator), typeof(StandardCalculator))
                    .AddMetadata("type", "Standard"));
        }
    }

    public class ScientificCalculator : ICalculator { }

    public class StandardCalculator : ICalculator { }
}