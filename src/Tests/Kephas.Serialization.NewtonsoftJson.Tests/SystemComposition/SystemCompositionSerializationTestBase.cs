﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serialization test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.SystemComposition
{
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Testing.Injection;

    public class SystemCompositionSerializationTestBase : SystemCompositionInjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
                                {
                                    typeof(JsonSerializer).Assembly,      // Kephas.Serialization.NewtonsoftJson
                                };
            return assemblies;
        }
    }
}
