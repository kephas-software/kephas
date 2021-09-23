// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceMetadataResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceMetadataResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for application service metadata resolver.
    /// </summary>
    public interface IAppServiceMetadataResolver
    {
        /// <summary>
        /// Gets the metadata value properties which should be retrieved from the attribute.
        /// </summary>
        /// <param name="attributeType">The type of the attribute providing metadata.</param>
        /// <returns>
        /// The metadata properties.
        /// </returns>
        IDictionary<string, IPropertyInfo> GetMetadataValueProperties(Type attributeType);

        /// <summary>
        /// Gets the metadata value from attribute.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="property">The metadata property.</param>
        /// <returns>
        /// The metadata value from attribute.
        /// </returns>
        object? GetMetadataValueFromAttribute(Type implementationType, Type attributeType, IPropertyInfo property);

        /// <summary>
        /// Gets the metadata name from the attribute type.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata name from the attribute type.</returns>
        string GetMetadataNameFromAttributeType(Type attributeType);

        /// <summary>
        /// Gets the metadata value from generic parameter.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="position">The position.</param>
        /// <param name="serviceType">Type of the service contract.</param>
        /// <returns>The metadata value.</returns>
        object GetMetadataValueFromGenericParameter(Type implementationType, int position, Type serviceType);

        /// <summary>
        /// Gets the metadata name from generic type parameter.
        /// </summary>
        /// <param name="genericTypeParameter">The generic type parameter.</param>
        /// <returns>The metadata name.</returns>
        string GetMetadataNameFromGenericTypeParameter(Type genericTypeParameter);
    }
}