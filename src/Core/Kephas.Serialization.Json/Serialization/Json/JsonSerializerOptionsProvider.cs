// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerOptionsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Serialization.Json;

/// <summary>
/// Default implementation of the <see cref="IJsonSerializerOptionsProvider"/>.
/// </summary>
public class JsonSerializerOptionsProvider(
    ILazyEnumerable<JsonConverter, ServiceMetadata>? lazyConverters = null,
    ILazyEnumerable<IJsonModifier, ServiceMetadata>? lazyModifiers = null) : IJsonSerializerOptionsProvider
{
    private JsonSerializerOptions? options;

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <returns>The <see cref="JsonSerializerOptions"/>.</returns>
    public JsonSerializerOptions GetOptions()
    {
        return options ??= ConfigureOptions(new JsonSerializerOptions());
    }

    /// <summary>
    /// Configures the provided options and returns them.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    /// <returns>The <paramref name="options"/>.</returns>
    public virtual JsonSerializerOptions ConfigureOptions(JsonSerializerOptions options)
    {
        options.PropertyNameCaseInsensitive = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.AllowTrailingCommas = true;
        options.ReadCommentHandling = JsonCommentHandling.Skip;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        ConfigureModifiers(options);
        var converters = lazyConverters?.SelectServices() ?? [];
        foreach (var jsonConverter in converters)
        {
            options.Converters.Add(jsonConverter);            
        }

        return options;
    }

    private void ConfigureModifiers(JsonSerializerOptions options)
    {
        var typeInfoResolver = options.TypeInfoResolver as DefaultJsonTypeInfoResolver
                               ?? new DefaultJsonTypeInfoResolver();
        var modifiers = lazyModifiers?.SelectServices() ?? [];
        foreach (var modifier in modifiers.Select(m => (Action<JsonTypeInfo>)m.Modify))
        {
            typeInfoResolver.Modifiers.Add(modifier);
        }
        options.TypeInfoResolver = typeInfoResolver;
    }
}
