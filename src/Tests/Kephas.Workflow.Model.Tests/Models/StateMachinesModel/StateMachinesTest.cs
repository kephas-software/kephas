// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachinesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Tests.Models.StateMachinesModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model;
    using Kephas.Runtime;
    using Kephas.Workflow.Model.Elements;
    using Kephas.Workflow.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class StateMachinesTest : WorkflowModelTestBase
    {
        [Test]
        public async Task InitializeAsync_statemachineinfo_support()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            typeRegistry.RegisterFactory(new WorkflowTypeInfoFactory());

            var container = this.CreateServicesBuilderForModel(
                new AmbientServices().Add<IRuntimeTypeRegistry>(typeRegistry, b => b.ExternallyOwned()),
                typeof(IDocumentStateMachine));
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var stateMachine = (StateMachineType)modelSpace.Classifiers.Single(c => c.Name == "Document");

            Assert.AreEqual(1, stateMachine.Parts.Count());
            Assert.AreEqual(1, stateMachine.Transitions.Count());
        }
    }
}