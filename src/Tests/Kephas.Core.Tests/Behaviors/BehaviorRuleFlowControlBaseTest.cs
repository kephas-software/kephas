// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorRuleFlowControlBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the behavior rule flow control base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Behaviors
{
    using Kephas.Behaviors;
    using Kephas.Runtime;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class BehaviorRuleFlowControlBaseTest
    {
        [Test]
        public void PriorityOrder_default()
        {
            var behavior = new DefaultBehaviorRuleFlowControl();
            Assert.AreEqual(0, behavior.ProcessingPriority);
        }

        [Test]
        public void PriorityOrder_attribute()
        {
            var behavior = new PriorityBehaviorRuleFlowControl();
            Assert.AreEqual(12, behavior.ProcessingPriority);
        }

        [Test]
        public void IsEndRule_default()
        {
            var behavior = new DefaultBehaviorRuleFlowControl();
            Assert.IsFalse(behavior.IsEndRule);
        }

        [Test]
        public void IsEndRule_attribute()
        {
            var behavior = new EndRuleBehaviorRuleFlowControl();
            Assert.IsTrue(behavior.IsEndRule);
        }

        private class DefaultBehaviorRuleFlowControl : BehaviorRuleFlowControlBase
        {
        }

        [ProcessingPriority(12)]
        private class PriorityBehaviorRuleFlowControl : BehaviorRuleFlowControlBase
        {
        }

        [EndRule]
        private class EndRuleBehaviorRuleFlowControl : BehaviorRuleFlowControlBase
        {
        }
    }
}