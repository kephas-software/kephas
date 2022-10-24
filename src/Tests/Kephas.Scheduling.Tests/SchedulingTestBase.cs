// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Scheduling.Reflection;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using Kephas.Workflow;

    public abstract class SchedulingTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IJobInfo).Assembly,                  // Kephas.Scheduling.Abstractions
                typeof(InProcessScheduler).Assembly,        // Kephas.Scheduling
                typeof(IWorkflowProcessor).Assembly,        // Kephas.Workflow.Abstractions
                typeof(DefaultWorkflowProcessor).Assembly,  // Kephas.Workflow
            };
        }
    }
}
