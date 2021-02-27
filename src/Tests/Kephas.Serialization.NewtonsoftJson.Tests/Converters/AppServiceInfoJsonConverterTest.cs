// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppServiceInfoJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_transient()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new AppServiceInfo(typeof(string), AppServiceLifetime.Transient);
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""ContractType"":""System.String"",""Lifetime"":""Transient"",""AllowMultiple"":false,""AsOpenGeneric"":false}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_transient()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""ContractType"":""System.String"",""Lifetime"":""Transient"",""AllowMultiple"":false,""AsOpenGeneric"":false}",
                new SerializationContext(Substitute.For<ICompositionContext>(), Substitute.For<ISerializationService>()) { RootObjectType = typeof(IAppServiceInfo) });

            Assert.IsInstanceOf<AppServiceInfo>(obj);
            var appServiceInfo = (IAppServiceInfo)obj!;
            Assert.AreEqual(typeof(string), appServiceInfo.ContractType);
            Assert.AreEqual(AppServiceLifetime.Transient, appServiceInfo.Lifetime);
            Assert.IsFalse(appServiceInfo.AllowMultiple);
            Assert.IsNull(appServiceInfo.InstanceType);
        }
    }
}