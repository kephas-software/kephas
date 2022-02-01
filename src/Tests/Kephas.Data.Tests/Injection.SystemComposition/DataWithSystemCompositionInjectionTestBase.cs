// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWithSystemCompositionInjectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Injection.SystemComposition
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Serialization;
    using Kephas.Testing.Injection;

    public class DataWithSystemCompositionInjectionTestBase : SystemCompositionInjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
                                {
                                    typeof(DefaultSerializationService).Assembly,      // Kephas.Data
                                    typeof(IDataContext).Assembly,      // Kephas.Data
                                };
            return assemblies;
        }
    }
}