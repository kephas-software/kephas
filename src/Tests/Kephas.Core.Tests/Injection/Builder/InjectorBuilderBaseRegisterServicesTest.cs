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
            Assert.AreEqual(2, builder.TypeBuilders.Count);
            var standardBuilder = builder.TypeBuilders[typeof(StandardCalculator)];
            Assert.IsTrue(standardBuilder.IsSingleton);
            Assert.AreEqual(typeof(StandardCalculator), standardBuilder.ServiceType);
            Assert.AreEqual(typeof(ICalculator), standardBuilder.ContractType);
            Assert.AreEqual("Standard", standardBuilder.Metadata!["type"]);

            var scientificBuilder = builder.TypeBuilders[typeof(ScientificCalculator)];
            Assert.IsTrue(scientificBuilder.IsSingleton);
            Assert.AreEqual(typeof(ScientificCalculator), scientificBuilder.ServiceType);
            Assert.AreEqual(typeof(ICalculator), scientificBuilder.ContractType);
            Assert.AreEqual("Scientific", scientificBuilder.Metadata!["type"]);
        }
    }

    public interface ICalculator
    {
    }

    public class CalculatorAppServiceInfosProvider : IAppServiceInfosProvider
    {
        public IEnumerable<ContractDeclaration> GetAppServiceContracts(IContext? context = null)
        {
            yield return new ContractDeclaration(
                typeof(ICalculator),
                new AppServiceInfo(typeof(ICalculator), typeof(ScientificCalculator))
                        { AllowMultiple = true }
                    .AddMetadata("type", "Scientific"));

            yield return new ContractDeclaration(
                typeof(ICalculator),
                new AppServiceInfo(typeof(ICalculator), typeof(StandardCalculator))
                        { AllowMultiple = true }
                    .AddMetadata("type", "Standard"));
        }
    }

    public class ScientificCalculator : ICalculator { }

    public class StandardCalculator : ICalculator { }
}