// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultServiceBehaviorProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service enumerable extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.ExportFactoryImporters;

namespace Kephas.Core.Tests.Services.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Behaviors;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultServiceBehaviorProviderTest
    {
        [Test]
        public void WhereEnabled_no_behaviors()
        {
            var ambientServicesMock = this.CreateInjectorWithFactories();
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            IServiceBehaviorProvider provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>());
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_exclude_all_behaviors()
        {
            var excludeAllBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false);
            var ambientServicesMock = this.CreateInjectorWithFactories(excludeAllBehaviorMock);
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            IServiceBehaviorProvider provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> { excludeAllBehaviorMock });
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }

        [Test]
        public void WhereEnabled_priority_with_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false, processingPriority: (Priority)1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: true, value: true, processingPriority: 0);
            var ambientServicesMock = this.CreateInjectorWithFactories(excludeBehaviorMock, includeBehaviorMock);
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            IServiceBehaviorProvider provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> { excludeBehaviorMock, includeBehaviorMock });
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_priority_without_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false, processingPriority: (Priority)1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: true, processingPriority: 0);
            var ambientServicesMock = this.CreateInjectorWithFactories(excludeBehaviorMock, includeBehaviorMock);
            var services = new List<ITestService> { Substitute.For<ITestService>() };

            IServiceBehaviorProvider provider = new DefaultServiceBehaviorProvider(ambientServicesMock, new List<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> { excludeBehaviorMock, includeBehaviorMock });
            var filteredServices = provider.WhereEnabled(services).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }


        private IInjector CreateInjectorWithFactories(params Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>[] ruleFactories)
        {
            var exporter = Substitute.For<ICollectionExportFactoryImporter>();
            exporter.ExportFactories.Returns(ruleFactories);

            var injector = Substitute.For<IInjector>();
            injector.Resolve(typeof(ICollectionExportFactoryImporter<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>))
                .Returns(exporter);

            return injector;
        }

        private Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata> CreateEnabledServiceBehaviorRuleFactory(
                bool canApply,
                bool isEndRule,
                bool value,
                Priority processingPriority = 0)
        {
            var service = this.CreateEnabledServiceBehaviorRule(canApply, isEndRule, value, processingPriority);
            return new Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>(() => service, new ServiceBehaviorRuleMetadata(typeof(ITestService), processingPriority: processingPriority));
        }

        private IEnabledServiceBehaviorRule CreateEnabledServiceBehaviorRule(bool canApply, bool isEndRule, bool value, Priority processingPriority = 0)
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