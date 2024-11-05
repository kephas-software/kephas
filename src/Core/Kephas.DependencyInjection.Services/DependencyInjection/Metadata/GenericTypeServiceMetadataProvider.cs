// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTypeServiceMetadataProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Implementation of a <see cref="IServiceMetadataProvider"/> for generic types.
/// </summary>
internal class GenericTypeServiceMetadataProvider : IServiceMetadataProvider
{
    /// <summary>
    /// The 'T' prefix in generic type arguments.
    /// </summary>
    private const string TypePrefix = "T";

    /// <summary>
    /// The 'Type' suffix in generic type arguments.
    /// </summary>
    private const string TypeSuffix = "Type";

    private readonly Type genericType;

    private GenericTypeServiceMetadataProvider(Type genericType)
    {
        this.genericType = genericType;
    }

    /// <summary>
    /// Gets the metadata provider for the indicated generic type.
    /// </summary>
    /// <param name="genericType">The generic type.</param>
    /// <returns>The <see cref="IServiceMetadataProvider"/>.</returns>
    public static IServiceMetadataProvider GetProvider(Type genericType)
    {
        if (!genericType.IsConstructedGenericType)
        {
            return NullServiceMetadataProvider.Instance;
        }

        return new GenericTypeServiceMetadataProvider(genericType);
    }

    /// <summary>
    /// Gets the metadata as an enumeration of (name, value) pairs.
    /// </summary>
    /// <returns>An enumeration of (name, value) pairs.</returns>
    public IEnumerable<KeyValuePair<string, object?>> GetMetadata()
    {
        var genericTypeDefinition = genericType.GetGenericTypeDefinition();

        var genericTypeParameters = genericTypeDefinition.GetTypeInfo().GenericTypeParameters;
        var genericTypeArguments = genericType.GetGenericArguments();
        for (var i = 0; i < genericTypeParameters.Length; i++)
        {
            var genericTypeParameter = genericTypeParameters[i];
            yield return KeyValuePair.Create<string, object?>(GetNameFromGenericTypeParameter(genericTypeParameter), GetValueFromGenericTypeArgument(genericTypeArguments[i]));
        }
    }

    /// <summary>
    /// Gets the metadata name from generic type parameter.
    /// </summary>
    /// <param name="genericTypeParameter">The generic type parameter.</param>
    /// <returns>The metadata name.</returns>
    public string GetNameFromGenericTypeParameter(Type genericTypeParameter)
    {
        var name = genericTypeParameter.Name;
        if (name.StartsWith(TypePrefix) && name.Length > 1 && name[1] == char.ToUpperInvariant(name[1]))
        {
            name = name[1..];
        }

        if (!name.EndsWith(TypeSuffix))
        {
            name += TypeSuffix;
        }

        return name;
    }

    private Type? GetValueFromGenericTypeArgument(Type genericArg)
    {
        return genericArg.IsGenericParameter ? genericArg.BaseType : genericArg;
    }

    private sealed class NullServiceMetadataProvider : IServiceMetadataProvider
    {
        public static readonly IServiceMetadataProvider Instance = new NullServiceMetadataProvider();

        public IEnumerable<KeyValuePair<string, object?>> GetMetadata() => [];
    }
}