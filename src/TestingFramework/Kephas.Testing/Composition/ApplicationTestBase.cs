// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;

    public class ApplicationTestBase : CompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>(base.GetDefaultConventionAssemblies())
                       {
                           typeof(IAppManager).GetTypeInfo().Assembly,     /* Kephas.Application*/
                       };
        }
    }
}
