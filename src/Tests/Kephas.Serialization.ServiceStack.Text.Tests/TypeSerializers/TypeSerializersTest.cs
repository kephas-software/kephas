// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSerializersTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type serializers test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Tests.TypeSerializers
{
    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class TypeSerializersTest : SerializationTestBase
    {
        [Test]
        public void Serialize_type_serializers()
        {
            var container = this.CreateContainer();
            var serialization = container.GetExport<ISerializationService>();

            var dto = new TestDto
                {
                    Data = new Expando { ["Name"] = "gigi", ["FamilyName"] = "Belogea" },
                    CustomData = new Expando { ["HasChildren"] = true },
                };

            var expected = "{\"$type\":\"Kephas.Serialization.ServiceStack.Text.Tests.TypeSerializers.TypeSerializersTest+TestDto\",\"data\":{\"Name\":\"gigi\",\"FamilyName\":\"Belogea\"},\"customData\":{\"HasChildren\":true}}";
            var dtoString = serialization.JsonSerialize(dto);

            Assert.AreEqual(expected, dtoString);
        }

        [Test]
        public void Deserialize_type_serializers()
        {
            var container = this.CreateContainer();
            var serialization = container.GetExport<ISerializationService>();

            var dtoString = "{\"$type\":\"Kephas.Serialization.ServiceStack.Text.Tests.TypeSerializers.TypeSerializersTest+TestDto\",\"data\":{\"Name\":\"gigi\",\"FamilyName\":\"Belogea\"},\"customData\":{\"HasChildren\":true}}";

            var dto = (TestDto)serialization.JsonDeserialize(dtoString);

            Assert.AreEqual("gigi", dto.Data["Name"]);
            Assert.AreEqual("Belogea", dto.Data["FamilyName"]);
            Assert.AreEqual("true", dto.CustomData["HasChildren"]);
        }

        public class TestDto
        {
            public IExpando Data { get; set; }

            public Expando CustomData { get; set; }
        }
    }
}
