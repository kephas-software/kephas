// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonSerializerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bson serializer test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Bson.Tests
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class BsonSerializerTest : SerializationTestBase
    {
        [Test]
        [Ignore("Not relevant for the moment, should be fixed in the future.")]
        public async Task SerializeAsync_Injection()
        {
            var container = this.BuildServiceProvider();
            var serializationService = container.Resolve<ISerializationService>();

            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };

            var serializedObj = await serializationService.BsonSerializeAsync(obj);

            Assert.AreEqual(@"{ ""_t"" : ""TestEntity"", ""Name"" : ""John Doe"", ""PersonalSite"" : ""http://site.com/my-site"" }", serializedObj);
        }

        public class TestEntity
        {
            public string Name { get; set; }

            public Uri PersonalSite { get; set; }
        }

        public class ExpandoEntity : Expando
        {
            public string Description { get; set; }
        }
    }
}
