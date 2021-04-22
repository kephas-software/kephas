// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Integration.Reflection.Dynamic
{
    using Kephas.IO;
    using Kephas.Reflection.Dynamic;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicTypeRegistryTest : SerializationTestBase
    {
        [Test]
        public void Deserialize_Schema1()
        {
            var container = this.CreateContainer();
            var runtimeTypeRegistry = container.GetExport<IRuntimeTypeRegistry>();
            var serializationService = this.CreateContainer().GetExport<ISerializationService>();
            var dynTypeRegistry = serializationService.JsonDeserialize<DynamicTypeRegistry>(
                this.GetJson("Schema1.json").ReadAllString(),
                cfg => cfg.RootObjectFactory = () => new DynamicTypeRegistry(runtimeTypeRegistry));
        }
    }
}