// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Messaging;
    using Kephas.Testing.Composition;

    public abstract class ConsoleTestBase : CompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = base.GetDefaultConventionAssemblies().ToList();
            assemblies.AddRange(new[]
                        {
                            typeof(IAppManager).Assembly,           // Kephas.Application
                            typeof(IMessageProcessor).Assembly,     // Kephas.Messaging
                            typeof(ICommandProcessor).Assembly,     // Kephas.Application.Console
                        });

            return assemblies;
        }
    }
}
