// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class AppServiceMetadataJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_AppServiceMetadata()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new AppServiceMetadata(processingPriority: (Priority)12, overridePriority: (Priority)24, serviceName: "gigi", isOverride: true)
            {
                ServiceType = typeof(int),
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""processingPriority"":12,""overridePriority"":24,""isOverride"":true,""serviceName"":""gigi"",""serviceInstanceType"":""System.Int32"",""dependencies"":[]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_AppServiceMetadata()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""processingPriority"":12,""overridePriority"":24,""isOverride"":true,""serviceName"":""gigi"",""serviceInstanceType"":""System.Int32"",""dependencies"":[]}",
                GetSerializationContext(typeof(AppServiceMetadata)));

            Assert.IsInstanceOf<AppServiceMetadata>(obj);
            var expando = (AppServiceMetadata)obj;
            Assert.AreEqual("gigi", expando.ServiceName);
            Assert.AreEqual(12, expando.ProcessingPriority);
            Assert.AreEqual(24, expando.OverridePriority);
            Assert.AreEqual(typeof(int), expando.ServiceType);
            Assert.IsTrue(expando.IsOverride);
        }

        [Test]
        public async Task SerializeAsync_SerializerMetadata()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new SerializerMetadata(typeof(XmlMediaType), processingPriority: (Priority)12, overridePriority: (Priority)24, serviceName: "gigi", isOverride: true)
            {
                ServiceType = typeof(int),
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Composition.SerializerMetadata"",""mediaType"":""Kephas.Net.Mime.XmlMediaType"",""processingPriority"":12,""overridePriority"":24,""isOverride"":true,""serviceName"":""gigi"",""serviceInstanceType"":""System.Int32"",""dependencies"":[]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_SerializerMetadata()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Composition.SerializerMetadata"",""mediaType"":""Kephas.Net.Mime.XmlMediaType"",""processingPriority"":12,""overridePriority"":24,""isOverride"":true,""serviceName"":""gigi"",""serviceInstanceType"":""System.Int32"",""dependencies"":[]}",
                GetSerializationContext(typeof(SerializerMetadata)));

            Assert.IsInstanceOf<SerializerMetadata>(obj);
            var expando = (SerializerMetadata)obj;
            Assert.AreEqual("gigi", expando.ServiceName);
            Assert.AreEqual(12, expando.ProcessingPriority);
            Assert.AreEqual(24, expando.OverridePriority);
            Assert.AreEqual(typeof(int), expando.ServiceType);
            Assert.IsTrue(expando.IsOverride);
        }
    }
}