// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectableConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters;

using Kephas.Injection;
using Kephas.Services;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class InjectableConverterTest : SerializationTestBase
{
    [Test]
    public async Task DeserializeAsync_Injectable()
    {
        var injectableFactory = Substitute.For<IInjectableFactory>();
        injectableFactory.Create(typeof(TestInjectable)).Returns(new TestInjectable("Test"));
        var settingsProvider = GetJsonSerializerSettingsProvider(injectableFactory);
        var serializer = new JsonSerializer(settingsProvider);
        var obj = await serializer.DeserializeAsync(@"{""Name"":""John Doe""}", this.GetSerializationContext(typeof(TestInjectable)));

        Assert.IsInstanceOf<TestInjectable>(obj);
        var injectable = (TestInjectable)obj;
        Assert.AreEqual("Test", injectable.Service);
        Assert.AreEqual("John Doe", injectable.Name);
    }

    [Test]
    public async Task SerializeAsync_Injectable()
    {
        var settingsProvider = GetJsonSerializerSettingsProvider();
        var serializer = new JsonSerializer(settingsProvider);
        var serializedObj = await serializer.SerializeAsync(new TestInjectable("Test") { Name = "John Doe" }, this.GetSerializationContext());

        Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.InjectableConverterTest+TestInjectable"",""name"":""John Doe""}", serializedObj);
    }

    public class TestInjectable : IInjectable
    {
        public TestInjectable(string service)
        {
            Service = service;
        }

        [ExcludeFromSerialization]
        public string Service { get; }

        public string? Name { get; set; }
    }
}