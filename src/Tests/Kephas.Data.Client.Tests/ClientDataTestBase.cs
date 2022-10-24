﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientDataTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client data test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Data.Client.Queries;
    using Kephas.Reflection;
    using Kephas.Testing;
    using Kephas.Testing.Services;

    public abstract class ClientDataTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(ITypeResolver).Assembly,            // Kephas.Data
                typeof(IDataSpace).Assembly,            // Kephas.Data
                typeof(IClientQueryProcessor).Assembly, // Kephas.Data.Client
            };
        }
    }
}
