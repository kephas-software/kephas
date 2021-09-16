// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefScriptingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests.Composition.Mef
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Testing.Composition;

    public class MefScriptingTestBase : SystemCompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>(base.GetDefaultConventionAssemblies())
            {
                typeof(DefaultScriptProcessor).Assembly, /* Kephas.Scripting */
            };
        }
    }
}
