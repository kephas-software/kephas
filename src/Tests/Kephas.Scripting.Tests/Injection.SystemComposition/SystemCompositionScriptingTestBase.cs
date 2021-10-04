﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionScriptingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests.Injection.SystemComposition
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Testing.Injection;

    public class SystemCompositionScriptingTestBase : SystemCompositionInjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(DefaultScriptProcessor).Assembly, /* Kephas.Scripting */
            };
        }
    }
}