// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultServiceBehaviorProviderTest.cs" company="Quartz Software SRL">
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
    using Kephas.Services;
    using Kephas.Services.Behavior;
    using Kephas.Services.Behavior.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultServiceBehaviorProviderTest
    {
        [Test]
        public void WhereEnabled_no_behaviors()
        {
            var ambientServicesMock = this.CreateAmbientServicesWithFactories();
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            var provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>());
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_exclude_all_behaviors()
        {
            var excludeAllBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false);
            var ambientServicesMock = this.CreateAmbientServicesWithFactories(excludeAllBehaviorMock);
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            var provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> { excludeAllBehaviorMock });
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }

        [Test]
        public void WhereEnabled_priority_with_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false, processingPriority: 1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: true, value: true, processingPriority: 0);
            var ambientServicesMock = this.CreateAmbientServicesWithFactories(excludeBehaviorMock, includeBehaviorMock);
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            var provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> { excludeBehaviorMock, includeBehaviorMock });
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_priority_without_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false, processingPriority: 1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: true, processingPriority: 0);
            var ambientServicesMock = this.CreateAmbientServicesWithFactories(excludeBehaviorMock, includeBehaviorMock);
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            var provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> { excludeBehaviorMock, includeBehaviorMock });
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }


        private IAmbientServices CreateAmbientServicesWithFactories(params IExportFactory<IEnabledServiceBehaviorRule<ITestService>, ServiceBehaviorRuleMetadata>[] ruleFactories)
        {
            var exporter = Substitute.For<ICollectionExportFactoryImporter>();
            exporter.ExportFactories.Returns(ruleFactories);

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport(typeof(ICollectionExportFactoryImporter<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>), null)
                .Returns(exporter);

            var ambientServicesMock = Substitute.For<IAmbientServices>();
            ambientServicesMock.CompositionContainer.Returns(compositionContext);
            return ambientServicesMock;
        }

        private IExportFactory<IEnabledServiceBehaviorRule<ITestService>, ServiceBehaviorRuleMetadata> CreateEnabledServiceBehaviorRuleFactory(
                bool canApply,
                bool isEndRule,
                bool value,
                int processingPriority = 0)
        {
            var service = this.CreateEnabledServiceBehaviorRule(canApply, isEndRule, value, processingPriority);
            return new ExportFactory<IEnabledServiceBehaviorRule<ITestService>, ServiceBehaviorRuleMetadata>(() => service, new ServiceBehaviorRuleMetadata(typeof(ITestService), processingPriority: processingPriority));
        }

        private IEnabledServiceBehaviorRule<ITestService> CreateEnabledServiceBehaviorRule(bool canApply, bool isEndRule, bool value, int processingPriority = 0)
        {
            var behaviorMock = Substitute.For<IEnabledServiceBehaviorRule<ITestService>>();
            behaviorMock
                .CanApply(Arg.Any<IServiceBehaviorContext<ITestService>>())
                .Returns(canApply);
            behaviorMock
                .CanApply(Arg.Any<IContext>())
                .Returns(canApply);
            behaviorMock.IsEndRule.Returns(isEndRule);
            behaviorMock.ProcessingPriority.Returns(processingPriority);
            behaviorMock
                .GetValue(Arg.Any<IServiceBehaviorContext<ITestService>>())
                .Returns(value ? BehaviorValue.True : BehaviorValue.False);
            behaviorMock
                .GetValue(Arg.Any<IContext>())
                .Returns(value ? BehaviorValue.True : BehaviorValue.False);
            return behaviorMock;
        }

        public interface ITestService { }
    }
}