﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Model;
    using Kephas.Testing.Model;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class DataModelTestBase : ModelTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IModelSpace).Assembly,
                typeof(IEntityType).Assembly,
            };
        }
    }
}