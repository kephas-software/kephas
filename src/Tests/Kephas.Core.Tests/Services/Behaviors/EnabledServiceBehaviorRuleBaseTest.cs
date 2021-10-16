// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceBehaviorRuleBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enabled service behavior rule base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Tests.Services.Behaviors
{
    using Kephas.Behaviors;
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
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService>(Substitute.For<IInjector>(), () => new TestService()));
            Assert.IsTrue(canApply);
        }

        [Test]
        public void CanApply_failure_mismatched_type()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService>(Substitute.For<IInjector>(), () => new AnotherTestService()));
            Assert.IsFalse(canApply);
        }

        [Test]
        public void CanApply_failure_mismatched_derived_type()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService>(Substitute.For<IInjector>(), () => new DerivedTestService()));
            Assert.IsFalse(canApply);
        }

        private interface ITestService { }
        private class TestService : ITestService { }
        private class DerivedTestService : TestService { }
        private class AnotherTestService : ITestService { }

        private class TestEnabledServiceBehaviorRule : EnabledServiceBehaviorRuleBase<ITestService, TestService>
        {
            /// <summary>
            /// Gets the behavior value.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>
            /// The behavior value.
            /// </returns>
            public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<ITestService> context)
            {
                return BehaviorValue.True;
            }
        }
    }
}