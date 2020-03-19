// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeStateMachineInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime state machine information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Runtime
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Workflow.AttributedModel;
    using Kephas.Workflow.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimeStateMachineInfoTest
    {
        [Test]
        public void Transitions()
        {
            var stateMachineInfo = new RuntimeStateMachineInfo(typeof(IPluginStateMachine));

            Assert.AreEqual(2, stateMachineInfo.Transitions.Count());

            var installTransition = stateMachineInfo.Transitions.First(t => t.Name == "Install");
            Assert.AreEqual(PluginState.None, installTransition.From.Single());
            Assert.AreEqual(PluginState.Installed, installTransition.To);

            var enableTransition = stateMachineInfo.Transitions.First(t => t.Name == "Enable");
            Assert.AreEqual(PluginState.Installed, enableTransition.From.Single());
            Assert.AreEqual(PluginState.Enabled, enableTransition.To);
        }

        [Test]
        public void TargetType()
        {
            var stateMachineInfo = new RuntimeStateMachineInfo(typeof(IPluginStateMachine));

            Assert.AreEqual(typeof(IPlugin).AsRuntimeTypeInfo(), stateMachineInfo.TargetType);
        }

        [Test]
        public void TargetStateProperty()
        {
            var stateMachineInfo = new RuntimeStateMachineInfo(typeof(IPluginStateMachine));

            Assert.AreEqual(typeof(IPlugin).AsRuntimeTypeInfo().Properties["State"], stateMachineInfo.TargetStateProperty);
        }

        public enum PluginState
        {
            None,
            Installed,
            Enabled,
        }

        public interface IPlugin
        {
            PluginState State { get; set; }
        }

        public interface IPluginStateMachine : IStateMachine<IPlugin, PluginState>
        {
            void OtherMethod();

            [Transition(PluginState.None, PluginState.Installed)]
            Task<IOperationResult<int>> InstallAsync();

            [Transition(PluginState.Installed, PluginState.Enabled)]
            IOperationResult<int> Enable();
        }
    }
}
