// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceBehaviorRuleBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the enabled service behavior rule base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services.Behavior
{
    using Kephas.Behavior;
    using Kephas.Services.Behavior;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class EnabledServiceBehaviorRuleBaseTest
    {
        [Test]
        public void CanApply_success()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService>(Substitute.For<IAmbientServices>(), new TestService()));
            Assert.IsTrue(canApply);
        }

        [Test]
        public void CanApply_failure_mismatched_type()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService>(Substitute.For<IAmbientServices>(), new AnotherTestService()));
            Assert.IsFalse(canApply);
        }

        [Test]
        public void CanApply_failure_mismatched_derived_type()
        {
            var behavior = new TestEnabledServiceBehaviorRule();
            var canApply = behavior.CanApply(new ServiceBehaviorContext<ITestService>(Substitute.For<IAmbientServices>(), new DerivedTestService()));
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