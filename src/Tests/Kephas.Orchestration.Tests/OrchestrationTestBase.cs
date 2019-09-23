// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Messaging;
    using Kephas.Testing.Composition;
    using NUnit.Framework;

    [TestFixture]
    public class OrchestrationTestBase : CompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
                                {
                                    typeof(IMessageProcessor).Assembly,
                                    typeof(IOrchestrationManager).Assembly,
                                };
            return assemblies;
        }
    }
}
