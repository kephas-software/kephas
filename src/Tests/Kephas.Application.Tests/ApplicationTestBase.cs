// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Testing.Injection;

    public class ApplicationTestBase : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(AppLifecycleBehaviorBase).Assembly,          // Kephas.Application
            };
        }
    }
}
