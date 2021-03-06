﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Services;
    using Kephas.Testing.Model;
    using Kephas.Workflow.Application;
    using Kephas.Workflow.Model.Elements;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class StateMachinesTest : WorkflowModelTestBase
    {
        [Test]
        public async Task InitializeAsync_statemachineinfo_support()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var behavior = new WorkflowAppLifecycleBehavior(typeRegistry);
            await behavior.BeforeAppInitializeAsync(Substitute.For<IContext>());

            var container = this.CreateContainerForModel(
                new AmbientServices(typeRegistry: typeRegistry),
                typeof(IDocumentStateMachine));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var stateMachine = (StateMachineType)modelSpace.Classifiers.Single(c => c.Name == "Document");

            Assert.AreEqual(1, stateMachine.Parts.Count());
            Assert.AreEqual(1, stateMachine.Transitions.Count());
        }
    }
}