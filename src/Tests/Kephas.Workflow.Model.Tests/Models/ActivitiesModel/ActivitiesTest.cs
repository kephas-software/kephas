﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivitiesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Tests.Models.ActivitiesModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model;
    using Kephas.Runtime;
    using Kephas.Workflow.Model.Elements;
    using Kephas.Workflow.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class ActivitiesTest : WorkflowModelTestBase
    {
        [Test]
        public async Task InitializeAsync_activityinfo_support()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            typeRegistry.RegisterFactory(new WorkflowTypeInfoFactory());

            var container = this.CreateInjectorForModel(
                new AmbientServices().Add<IRuntimeTypeRegistry>(typeRegistry, b => b.ExternallyOwned()),
                typeof(ILaughActivity),
                typeof(IEnjoyActivity));
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var laughActivity = (ActivityType)modelSpace.Classifiers.Single(c => c.Name == "Laugh");
            var enjoyActivity = (ActivityType)modelSpace.Classifiers.Single(c => c.Name == "Enjoy");

            Assert.AreEqual(1, laughActivity.Parts.Count());
            Assert.AreEqual(0, laughActivity.Parameters.Count());

            Assert.AreEqual(1, enjoyActivity.Parts.Count());
            Assert.AreEqual(1, enjoyActivity.Parameters.Count());
        }
    }
}