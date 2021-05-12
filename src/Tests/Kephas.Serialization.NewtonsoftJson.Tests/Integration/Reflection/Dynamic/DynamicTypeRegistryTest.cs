// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Integration.Reflection.Dynamic
{
    using System.Linq;

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
            var serializationService = this.CreateContainer().GetExport<ISerializationService>();
            var dynTypeRegistry = serializationService.JsonDeserialize<DynamicTypeRegistry>(
                this.GetJson("Schema1.json").ReadAllString());

            Assert.AreEqual("schema-1", dynTypeRegistry.Name);
            Assert.AreEqual(2, dynTypeRegistry.Types.Count);

            var customerType = dynTypeRegistry.Types.First();
            Assert.AreEqual("Customer", customerType.Name);
            Assert.AreEqual("Customer", customerType.FullName);

            var docType = dynTypeRegistry.Types.Skip(1).First();
            Assert.AreEqual("Document", docType.Name);
            Assert.AreEqual("Document", docType.FullName);

            var syncIdProperty = docType.Properties.Single(p => p.Name == "SyncId");
            Assert.AreEqual(typeof(long), ((IRuntimeTypeInfo)syncIdProperty.ValueType).Type);
            Assert.AreEqual("System.Int64", ((DynamicPropertyInfo)syncIdProperty).ValueTypeName);
        }
    }
}