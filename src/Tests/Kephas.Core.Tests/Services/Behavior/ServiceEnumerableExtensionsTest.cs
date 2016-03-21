namespace Kephas.Core.Tests.Services.Behavior
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Behavior;
    using Kephas.Composition;
    using Kephas.Services.Behavior;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    [TestFixture]
    public class ServiceEnumerableExtensionsTest
    {
        [Test]
        public void WhereEnabled_no_behaviors()
        {
            var compositionContextMock = this.CreateCompositionContext();

            var services = new List<ITestService> { Mock.Create<ITestService>() };
            var filteredServices = services.WhereEnabled(compositionContextMock).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_exclude_all_behaviors()
        {
            var excludeAllBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: false);
            var compositionContextMock = this.CreateCompositionContext(excludeAllBehaviorMock);

            var services = new List<ITestService> { Mock.Create<ITestService>() };
            var filteredServices = services.WhereEnabled(compositionContextMock).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }

        [Test]
        public void WhereEnabled_priority_with_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: false, processingPriority: 1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: true, value: true, processingPriority: 0);
            var compositionContextMock = this.CreateCompositionContext(excludeBehaviorMock, includeBehaviorMock);

            var services = new List<ITestService> { Mock.Create<ITestService>() };
            var filteredServices = services.WhereEnabled(compositionContextMock).ToList();
            Assert.AreEqual(services.Count, filteredServices.Count);
            Assert.AreEqual(services[0], filteredServices[0]);
        }

        [Test]
        public void WhereEnabled_priority_without_end_rule()
        {
            var excludeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: false, processingPriority: 1);
            var includeBehaviorMock = this.CreateEnabledServiceBehaviorRule(canApply: true, isEndRule: false, value: true, processingPriority: 0);
            var compositionContextMock = this.CreateCompositionContext(excludeBehaviorMock, includeBehaviorMock);

            var services = new List<ITestService> { Mock.Create<ITestService>() };
            var filteredServices = services.WhereEnabled(compositionContextMock).ToList();
            Assert.AreEqual(0, filteredServices.Count);
        }

        private ICompositionContext CreateCompositionContext(params IEnabledServiceBehaviorRule<ITestService>[] rules)
        {
            var compositionContextMock = Mock.Create<ICompositionContext>();
            compositionContextMock.Arrange(c => c.GetExports<IEnabledServiceBehaviorRule<ITestService>>(Arg.IsAny<string>()))
                .Returns(new List<IEnabledServiceBehaviorRule<ITestService>>(rules));
            return compositionContextMock;
        }

        private IEnabledServiceBehaviorRule<ITestService> CreateEnabledServiceBehaviorRule(bool canApply, bool isEndRule, bool value, int processingPriority = 0)
        {
            var behaviorMock = Mock.Create<IEnabledServiceBehaviorRule<ITestService>>();
            behaviorMock.Arrange(b => b.CanApply(Arg.IsAny<IServiceBehaviorContext<ITestService>>())).Returns(canApply);
            behaviorMock.Arrange(b => b.IsEndRule).Returns(isEndRule);
            behaviorMock.Arrange(b => b.ProcessingPriority).Returns(processingPriority);
            behaviorMock.Arrange(b => b.GetValue(Arg.IsAny<IServiceBehaviorContext<ITestService>>()))
                .Returns(value ? BehaviorValue.True : BehaviorValue.False);
            return behaviorMock;
        }

        public interface ITestService { }
    }
}