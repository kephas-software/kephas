// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacScriptingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests.Injection.Autofac
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Testing.Services;

    public class AutofacScriptingTestBase : AutofacInjectionTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(DefaultScriptProcessor).Assembly, /* Kephas.Scripting */
            };
        }
    }
}
