// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service enumerable extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behaviors.Tests.Services.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Behaviors;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class EnabledServiceCollectionTest : TestBase
    {
        [Test]
        public void GetEnumerator_no_behaviors()
        {
            var servicesMock = this.CreateInjectorWithFactories();
            var services = new LazyEnumerable<ITestService, AppServiceMetadata>(
                new Lazy<ITestService, AppServiceMetadata>[]
                {
                    new (() => Substitute.For<ITestService>(), new AppServiceMetadata()),
                });

            var provider = new EnabledLazyEnumerable<ITestService, AppServiceMetadata>(
                services,
                servicesMock.Resolve<IInjectableFactory>(),
                new LazyEnumerable<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>());
            var filteredServices = provider.ToList();

            CollectionAssert.AreEqual(services, filteredServices);
        }

        [Test]
        public void GetEnumerator_exclude_all_behaviors()
        {
            var excludeAllBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false);
            var servicesMock = this.CreateInjectorWithFactories(excludeAllBehaviorMock);
            var services = new LazyEnumerable<ITestService, AppServiceMetadata>(
                new Lazy<ITestService, AppServiceMetadata>[]
                {
                    new (() => Substitute.For<ITestService>(), new AppServiceMetadata()),
                });

            var provider = new EnabledEnumerable<ITestService>(
                services,
                servicesMock.Resolve<IInjectableFactory>(),
                new LazyEnumerable<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>(new[] { excludeAllBehaviorMock }));

            var filteredServices = provider.ToList();
            CollectionAssert.IsEmpty(filteredServices);
        }

        [Test]
        public void GetEnumerator_priority_with_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false, processingPriority: (Priority)1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: true, value: true, processingPriority: 0);
            var servicesMock = this.CreateInjectorWithFactories(excludeBehaviorMock, includeBehaviorMock);
            var services = new FactoryEnumerable<ITestService, AppServiceMetadata>(
                new IExportFactory<ITestService, AppServiceMetadata>[]
                {
                    new ExportFactory<ITestService, AppServiceMetadata>(() => Substitute.For<ITestService>(), new AppServiceMetadata()),
                });

            var provider = new EnabledFactoryEnumerable<ITestService, AppServiceMetadata>(
                services,
                servicesMock.Resolve<IInjectableFactory>(),
                new LazyEnumerable<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>(new[] { excludeBehaviorMock, includeBehaviorMock }));

            var filteredServices = provider.ToList();

            CollectionAssert.AreEqual(services, filteredServices);
        }

        [Test]
        public void GetEnumerator_priority_without_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: false, processingPriority: (Priority)1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRuleFactory(canApply: true, isEndRule: false, value: true, processingPriority: 0);
            var servicesMock = this.CreateInjectorWithFactories(excludeBehaviorMock, includeBehaviorMock);
            var services = new LazyEnumerable<ITestService, AppServiceMetadata>(
                new Lazy<ITestService, AppServiceMetadata>[]
                {
                    new (() => Substitute.For<ITestService>(), new AppServiceMetadata()),
                });

            var provider = new EnabledEnumerable<ITestService>(
                services,
                servicesMock.Resolve<IInjectableFactory>(),
                new LazyEnumerable<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>(new[] { excludeBehaviorMock, includeBehaviorMock }));

            var filteredServices = provider.ToList();
            CollectionAssert.IsEmpty(filteredServices);
        }


        private IServiceProvider CreateInjectorWithFactories(params Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>[] ruleFactories)
        {
            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve(typeof(ICollection<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>))
                .Returns(ruleFactories);

            var injectableFactory = this.CreateInjectableFactoryMock((ci, _) => new ServiceBehaviorContext<ITestService, AppServiceMetadata>(
                injector,
                ci.Arg<object?[]>()[0] as Func<ITestService>,
                ci.Arg<object?[]>()[1] as AppServiceMetadata));

            injector.Resolve(typeof(IInjectableFactory)).Returns(injectableFactory);
            injector.Resolve<IInjectableFactory>().Returns(injectableFactory);

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
            var behaviorMock = Substitute.For<IEnabledServiceBehaviorRule<ITestService, AppServiceMetadata>>();
            behaviorMock
                .CanApply(Arg.Any<IServiceBehaviorContext<ITestService, AppServiceMetadata>>())
                .Returns(canApply);
            behaviorMock
                .CanApply(Arg.Any<IContext>())
                .Returns(canApply);
            behaviorMock.IsEndRule.Returns(isEndRule);
            behaviorMock.ProcessingPriority.Returns(processingPriority);
            behaviorMock
                .GetValue(Arg.Any<IServiceBehaviorContext<ITestService, AppServiceMetadata>>())
                .Returns(value ? BehaviorValue.True : BehaviorValue.False);
            behaviorMock
                .GetValue(Arg.Any<IContext>())
                .Returns(value ? BehaviorValue.True : BehaviorValue.False);
            return behaviorMock;
        }

        public interface ITestService { }
    }
}