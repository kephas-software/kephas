﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serialization test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Bson.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Testing.Composition;

    public class SerializationTestBase : CompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
                                {
                                    typeof(BsonSerializer).Assembly,      // Kephas.Serialization.MongoDBBson
                                };
            return assemblies;
        }
    }
}
