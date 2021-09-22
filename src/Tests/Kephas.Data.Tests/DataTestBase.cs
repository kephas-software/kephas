// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Testing.Injection;

    public abstract class DataTestBase : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
                                {
                                    typeof(IDataContext).Assembly,      // Kephas.Data
                                };
            return assemblies;
        }
    }
}
