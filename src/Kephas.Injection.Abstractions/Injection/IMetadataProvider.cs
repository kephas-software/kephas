// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetadataProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides metadata for injection.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Gets the metadata as an enumeration of (name, value) pairs.
        /// </summary>
        /// <returns>An enumeration of (name, value) pairs.</returns>
        IEnumerable<(string name, object? value)> GetMetadata();

        /// <summary>
        /// Gets the <see cref="IMetadataProvider"/> for the <see cref="genericType"/>.
        /// </summary>
        /// <param name="genericType">The generic type.</param>
        /// <returns>A metadata provider.</returns>
        public static IMetadataProvider GetGenericTypeMetadataProvider(Type genericType)
            => GenericTypeMetadataProvider.GetProvider(genericType);
    }

    /// <summary>
    /// Implementation of a <see cref="IMetadataProvider"/> for generic types.
    /// </summary>
    internal class GenericTypeMetadataProvider : IMetadataProvider
    {
        /// <summary>
        /// The 'T' prefix in generic type arguments.
        /// </summary>
        private const string TypePrefix = "T";

        /// <summary>
        /// The 'Type' suffix in generic type arguments.
        /// </summary>
        private const string TypeSuffix = "Type";

        private static readonly IDictionary<Type, IMetadataProvider> Providers = new Dictionary<Type, IMetadataProvider>();
        private readonly Type genericType;

        private GenericTypeMetadataProvider(Type genericType)
        {
            this.genericType = genericType;
        }

        /// <summary>
        /// Gets the metadata provider for the indicated generic type.
        /// </summary>
        /// <param name="genericType">The generic type.</param>
        /// <returns>The <see cref="IMetadataProvider"/>.</returns>
        public static IMetadataProvider GetProvider(Type genericType)
        {
            if (!genericType.IsConstructedGenericType)
            {
                return NullMetadataProvider.Instance;
            }

            if (Providers.TryGetValue(genericType, out var provider))
            {
                return provider;
            }

            provider = new GenericTypeMetadataProvider(genericType);
            Providers[genericType] = provider;
            return provider;
        }

        /// <summary>
        /// Gets the metadata as an enumeration of (name, value) pairs.
        /// </summary>
        /// <returns>An enumeration of (name, value) pairs.</returns>
        public IEnumerable<(string name, object? value)> GetMetadata()
        {
            var genericTypeDefinition = this.genericType.GetGenericTypeDefinition();

            var genericTypeParameters = genericTypeDefinition.GetTypeInfo().GenericTypeParameters;
            var genericTypeArguments = this.genericType.GetGenericArguments();
            for (var i = 0; i < genericTypeParameters.Length; i++)
            {
                var genericTypeParameter = genericTypeParameters[i];
                yield return (this.GetNameFromGenericTypeParameter(genericTypeParameter), this.GetValueFromGenericTypeArgument(genericTypeArguments[i]));
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

        private class NullMetadataProvider : IMetadataProvider
        {
            public static readonly IMetadataProvider Instance = new NullMetadataProvider();

            public IEnumerable<(string name, object? value)> GetMetadata() => Enumerable.Empty<(string name, object? value)>();
        }
    }
}