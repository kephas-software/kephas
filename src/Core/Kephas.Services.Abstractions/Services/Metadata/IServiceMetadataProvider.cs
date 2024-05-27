// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceMetadataProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Metadata
{
    /// <summary>
    /// Provides metadata for injection.
    /// </summary>
    public interface IServiceMetadataProvider
    {
        /// <summary>
        /// Gets the metadata as an enumeration of (name, value) pairs.
        /// </summary>
        /// <returns>An enumeration of (name, value) pairs.</returns>
        IEnumerable<(string name, object? value)> GetMetadata();

        /// <summary>
        /// Gets the <see cref="IServiceMetadataProvider"/> for the <see paramref="genericType"/>.
        /// </summary>
        /// <param name="genericType">The generic type.</param>
        /// <returns>A metadata provider.</returns>
        public static IServiceMetadataProvider GetGenericTypeMetadataProvider(Type genericType)
            => GenericTypeServiceMetadataProvider.GetProvider(genericType);
    }
}
