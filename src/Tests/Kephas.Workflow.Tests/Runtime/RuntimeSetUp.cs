// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeSetUp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime set up class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Runtime
{
    using Kephas.Application;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow.Application;

    using NSubstitute;

    using NUnit.Framework;

    [SetUpFixture]
    public class RuntimeSetUp
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            var lifecycleBehavior = new WorkflowApplicationLifecycleBehavior();
            lifecycleBehavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>()).WaitNonLocking();
        }

        [OneTimeTearDown]
        public void Finalize()
        {
        }
    }
}