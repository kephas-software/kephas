// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using Kephas.Serialization.Json;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Serialization.Json;

public class IJsonServiceTest
{
    [Fact]
    public async Task SerializeAsync_Of_T()
    {
        var obj = Substitute.For<ICloneable>();
        IJsonService service = new TestJsonService(obj, "cloneable");
        Assert.Same("cloneable", await service.SerializeAsync(obj));
    }

    [Fact]
    public async Task SerializeAsync()
    {
        var obj = Substitute.For<ICloneable>();
        IJsonService service = new TestJsonService(obj, "cloneable");
        Assert.Same("cloneable", await service.SerializeAsync(obj, typeof(ICloneable)));
    }

    [Fact]
    public async Task DeserializeAsync_Of_T()
    {
        var obj = Substitute.For<ICloneable>();
        IJsonService service = new TestJsonService(obj, "cloneable");
        Assert.Same(obj, await service.DeserializeAsync<ICloneable>("cloneable"));
    }

    [Fact]
    public async Task DeserializeAsync()
    {
        var obj = Substitute.For<ICloneable>();
        IJsonService service = new TestJsonService(obj, "cloneable");
        Assert.Same(obj, await service.DeserializeAsync("cloneable", typeof(ICloneable)));
    }

    private class TestJsonService : IJsonService
    {
        private readonly object _deserialized;
        private readonly string _serialized;

        public TestJsonService(object deserialized, string serialized)
        {
            _deserialized = deserialized;
            _serialized = serialized;
        }
        
        public T Deserialize<T>(string serializedObj, Action<JsonSerializerOptions>? optionsConfig = null)
        {
            return serializedObj == _serialized
                ? (T) _deserialized
                : throw new InvalidOperationException();
        }

        public object? Deserialize(string serializedObj, Type objType, Action<JsonSerializerOptions>? optionsConfig = null)
        {
            return serializedObj == _serialized
                ? _deserialized
                : throw new InvalidOperationException();
        }

        public string Serialize<T>(T obj, Action<JsonSerializerOptions>? optionsConfig = null)
        {
            return _serialized;
        }

        public string Serialize(object obj, Type? objType, Action<JsonSerializerOptions>? optionsConfig = null)
        {
            return _serialized;
        }
    }
}