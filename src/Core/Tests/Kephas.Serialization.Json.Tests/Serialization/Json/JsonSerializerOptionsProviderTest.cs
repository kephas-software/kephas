// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerOptionsProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using FluentAssertions;
using Kephas.DependencyInjection.Metadata;
using Kephas.Serialization.Json;
using Kephas.Testing;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Serialization.Json;

public class JsonSerializerOptionsProviderTest
{
    [Fact]
    public void GetOptions_contains_converter()
    {
        var converter = new TestConverter();
        var lazyConverters = Test.GetTestLazyEnumerable<JsonConverter, ServiceMetadata>(converter);
        var provider = new JsonSerializerOptionsProvider(lazyConverters);
        var options = provider.GetOptions();

        options.Converters.Should().Contain(converter);
    }

    [Fact]
    public void GetOptions_contains_modifier()
    {
        var modifier = Substitute.For<IJsonModifier>();
        var lazyModifiers = Test.GetTestLazyEnumerable<IJsonModifier, ServiceMetadata>(modifier);
        var provider = new JsonSerializerOptionsProvider(lazyModifiers: lazyModifiers);
        var options = provider.GetOptions();

        options.TypeInfoResolver.Should().BeOfType<DefaultJsonTypeInfoResolver>();
        var resolver = (DefaultJsonTypeInfoResolver)options.TypeInfoResolver!;
        resolver.Modifiers.Count.Should().Be(1);
        resolver.Modifiers[0](null!);
        modifier.Received(1).Modify(null!);
    }

    private class TestConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}