// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultServiceProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service enumerable extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services.Behavior
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Behavior;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Services.Behavior;
    using Kephas.Services.Behavior.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultServiceProviderTest
    {
        [Test]
        public void WhereEnabled_no_behaviors()
        {
            var ambientServicesMock = this.CreateAmbientServicesMock();

            var services = new List<ITestService> { Substitute.For<ITestService>() };
            var filteredServices = services.WhereEnabled(ambientServicesMock).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_exclude_all_behaviors()
        {
            var excludeAllBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: false);
            var ambientServicesMock = this.CreateAmbientServicesMock(excludeAllBehaviorMock);

            var services = new List<ITestService> { Substitute.For<ITestService>() };
            var filteredServices = services.WhereEnabled(ambientServicesMock).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }

        [Test]
        public void WhereEnabled_priority_with_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: false, processingPriority: 1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: true, value: true, processingPriority: 0);
            var ambientServicesMock = this.CreateAmbientServicesMock(excludeBehaviorMock, includeBehaviorMock);

            var services = new List<ITestService> { Substitute.For<ITestService>() };
            var filteredServices = services.WhereEnabled(ambientServicesMock).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_priority_without_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: false, processingPriority: 1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: true, processingPriority: 0);
            var ambientServicesMock = this.CreateAmbientServicesMock(excludeBehaviorMock, includeBehaviorMock);

            var services = new List<ITestService> { Substitute.For<ITestService>() };
            var filteredServices = services.WhereEnabled(ambientServicesMock).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }


        private IAmbientServices CreateAmbientServicesMock(params IEnabledServiceBehaviorRule<ITestService>[] rules)
        {
            var exporter = Substitute.For<ICollectionExportFactoryImporter>();
            exporter.ExportFactories.Returns(
                rules.Select(
                    r => new ExportFactory<IEnabledServiceBehaviorRule<ITestService>, ServiceBehaviorRuleMetadata>(
                        () => r,
                        new ServiceBehaviorRuleMetadata(typeof(ITestService)))).ToList());

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport(typeof(ICollectionExportFactoryImporter<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>), null)
                .Returns(exporter);

            var ambientServicesMock = Substitute.For<IAmbientServices>();
            ambientServicesMock.CompositionContainer.Returns(compositionContext);
            return ambientServicesMock;
        }

        private IEnabledServiceBehaviorRule<ITestService> CreateEnabledServiceBehaviorRule(bool canApply, bool isEndRule, bool value, int processingPriority = 0)
        {
            var behaviorMock = Substitute.For<IEnabledServiceBehaviorRule<ITestService>>();
            behaviorMock
                .CanApply(Arg.Any<IServiceBehaviorContext<ITestService>>())
                .ReturnsForAnyArgs(canApply);
            behaviorMock.IsEndRule.Returns(isEndRule);
            behaviorMock.ProcessingPriority.Returns(processingPriority);
            behaviorMock.GetValue(Arg.Any<IServiceBehaviorContext<ITestService>>())
                .Returns(value ? BehaviorValue.True : BehaviorValue.False);
            return behaviorMock;
        }

        public interface ITestService { }
    }
}