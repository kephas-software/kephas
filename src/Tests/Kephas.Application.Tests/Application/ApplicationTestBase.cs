// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Application
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Application;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Testing;
    using Kephas.Testing.Services;

    public class ApplicationTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(ITypeRegistry).Assembly,             // Kephas.Reflection
                typeof(IOperation).Assembly,                // Kephas.Operations
                typeof(IAppLifecycleBehavior).Assembly,     // Kephas.Application.Abstractions
                typeof(AppBase).Assembly,                 // Kephas.Application
            };
        }
    }
}
