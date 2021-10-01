// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorBuilderBaseRegisterServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the conventions builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Injection;
    using NUnit.Framework;

    /// <summary>
    /// The conventions builder extensions test.
    /// </summary>
    [TestFixture]
    public class InjectorBuilderBaseRegisterServicesTest
    {
        [Test]
        public void RegisterServices_singleton_derived_conventions_with_metadata()
        {
            var builder = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();
            var newBuilder = builder
                .WithAppServiceInfosProvider(new CalculatorAppServiceInfosProvider())
                .RegisterServices();

            Assert.AreSame(builder, newBuilder);
            Assert.AreEqual(1, builder.TypeBuilders.Count);
            var partBuilder = (InjectorBuilderBaseTest.TestTypeBuilder)builder.TypeBuilders.First().Value;
            Assert.IsTrue(partBuilder.IsSingleton);
            Assert.AreEqual(typeof(ICalculator), partBuilder.Type);
            var exportBuilder = partBuilder;
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