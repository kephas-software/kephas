// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacConnectivityTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.Tests.Injection.Autofac;

using System.Collections.Generic;
using System.Reflection;

using Kephas.Testing.Services;

public class AutofacConnectivityTestBase : AutofacInjectionTestBase
{
    protected override IEnumerable<Assembly> GetAssemblies()
    {
        return new List<Assembly>(base.GetAssemblies())
            {
                typeof(DefaultConnectionProvider).Assembly, /* Kephas.Connectivity */
            };
    }
}
