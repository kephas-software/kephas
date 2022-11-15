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
    using Kephas.Testing;
    using Kephas.Testing.Services;

    public abstract class DataTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
                                {
                                    typeof(IDataContext).Assembly,      // Kephas.Data
                                };
            return assemblies;
        }
    }
}
