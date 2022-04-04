// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceBehaviorRuleBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enabled service behavior rule base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services.Behaviors
{
    using Kephas.Behaviors;
    using Kephas.Injection;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class EnabledServiceBehaviorRuleBaseTest
    {
        [Test]
        public void CanApply_success()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService, AppServiceMetadata>(Substitute.For<IInjector>(), () => new TestService(), new AppServiceMetadata()));
            Assert.IsTrue(canApply);
        }

        [Test]
        public void CanApply_failure_mismatched_type()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService, AppServiceMetadata>(Substitute.For<IInjector>(), () => new AnotherTestService(), new AppServiceMetadata()));
            Assert.IsFalse(canApply);
        }

        [Test]
        public void CanApply_failure_mismatched_derived_type()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService, AppServiceMetadata>(Substitute.For<IInjector>(), () => new DerivedTestService(), new AppServiceMetadata()));
            Assert.IsFalse(canApply);
        }

        private interface ITestService { }
        private class TestService : ITestService { }
        private class DerivedTestService : TestService { }
        private class AnotherTestService : ITestService { }

        private class TestEnabledServiceBehaviorRule : EnabledServiceBehaviorRuleBase<ITestService, AppServiceMetadata, TestService>
        {
            /// <summary>
            /// Gets the behavior value.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>
            /// The behavior value.
            /// </returns>
            public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<ITestService, AppServiceMetadata> context)
            {
                return BehaviorValue.True;
            }
        }
    }
}