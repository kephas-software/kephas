// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ExpandoJsonConverterTest : SerializationTestBase
    {

        [Test]
        public async Task SerializeAsync_Expando()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new ExpandoEntity
            {
                Description = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}", serializedObj);
        }

        private static DefaultJsonSerializerSettingsProvider GetJsonSerializerSettingsProvider()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(
                new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()), new RuntimeTypeRegistry(), Substitute.For<ILogManager>());
            return settingsProvider;
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

        public class TestWithType
        {
            public Type Type { get; set; }
        }

        public class NestedValues
        {
            public object Values { get; set; }
        }
    }
}