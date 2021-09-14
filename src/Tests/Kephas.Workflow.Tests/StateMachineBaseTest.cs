// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the state machine base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Workflow.AttributedModel;
    using Kephas.Workflow.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class StateMachineBaseTest
    {
        public StateMachineBaseTest()
        {
            this.TypeRegistry = new RuntimeTypeRegistry();
            this.TypeRegistry.RegisterFactory(new WorkflowTypeInfoFactory());
        }

        public IRuntimeTypeRegistry TypeRegistry { get; }

        [Test]
        public async Task TransitionAsync_changes_state_on_success()
        {
            var stateMachine = new TestStateMachine(new TestEntity(), this.TypeRegistry);
            var context = new TransitionContext(Substitute.For<IInjector>(), stateMachine)
            {
                To = TestState.Valid,
            };

            Assert.AreEqual(TestState.None, stateMachine.Target.State);
            var result = await stateMachine.TransitionAsync(context);
            Assert.AreEqual(TestState.Valid, stateMachine.Target.State);
            Assert.AreEqual("Success", result);
        }

        public enum TestState
        {
            None,
            Valid,
            Error,
        }

        public class TestEntity
        {
            public TestState State { get; set; }
        }

        public class TestStateMachine : StateMachineBase<TestEntity, TestState>
        {
            public TestStateMachine(TestEntity target, IRuntimeTypeRegistry typeRegistry)
                : base(target, typeRegistry)
            {
            }

            [Transition(TestState.None, TestState.Valid)]
            public async Task<string> ValidateAsync(CancellationToken token)
            {
                return "Success";
            }
        }
    }
}