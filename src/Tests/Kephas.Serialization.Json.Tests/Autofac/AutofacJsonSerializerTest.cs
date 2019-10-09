// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacJsonSerializerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="JsonSerializer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Autofac
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Kephas.Dynamic;
    using Kephas.Serialization.Json;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="JsonSerializer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacJsonSerializerTest : AutofacSerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_Composition()
        {
            var container = this.CreateContainer();
            var serializationService = container.GetExport<ISerializationService>();

            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };

            var serializedObj = await serializationService.JsonSerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Autofac.AutofacJsonSerializerTest+TestEntity"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}", serializedObj);
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