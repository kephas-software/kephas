// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Model
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Configuration;
    using Kephas.Operations;
    using Kephas.Runtime;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class ModelTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IRuntimeTypeInfo).Assembly,  // Kephas.Reflection
                typeof(IConfiguration<>).Assembly,  // Kephas.Configuration
                typeof(IOperationResult).Assembly,  // Kephas.Operations
            };
        }
    }
}