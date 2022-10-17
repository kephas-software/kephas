// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowModelTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Testing.Model;

    public abstract class WorkflowModelTestBase : ModelTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IActivity).Assembly,         // Kephas.Workflow
                typeof(IActivityType).Assembly,     // Kephas.Workflow.Model
            };
        }
    }
}