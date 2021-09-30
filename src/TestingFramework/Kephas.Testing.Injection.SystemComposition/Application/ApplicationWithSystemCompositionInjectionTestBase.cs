// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationWithSystemCompositionInjectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Application
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Testing.Injection;

    public class ApplicationWithSystemCompositionInjectionTestBase : SystemCompositionInjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
                       {
                           typeof(IAppManager).GetTypeInfo().Assembly,     /* Kephas.Application*/
                       };
        }
    }
}
