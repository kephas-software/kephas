// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceMetadataResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceMetadataResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Runtime;

    internal interface IAppServiceMetadataResolver
    {
        /// <summary>
        /// Gets the metadata value properties which should be retrieved from the attribute.
        /// </summary>
        /// <param name="attributeTypeInfo">Information describing the attribute type.</param>
        /// <returns>
        /// The metadata properties.
        /// </returns>
        IDictionary<string, IPropertyInfo> GetMetadataValueProperties(IRuntimeTypeInfo attributeTypeInfo);

        /// <summary>
        /// Gets the metadata value from attribute.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="property">The metadata property.</param>
        /// <returns>
        /// The metadata value from attribute.
        /// </returns>
        object GetMetadataValueFromAttribute(Type partType, Type attributeType, IPropertyInfo property);

        /// <summary>
        /// Gets the metadata name from the attribute type.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata name from the attribute type.</returns>
        string GetMetadataNameFromAttributeType(Type attributeType);
    }
}