// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAttributeAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAttributeAnnotation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;

    /// <summary>
    /// Contract for annotations based on attributes.
    /// </summary>
    public interface IAttributeAnnotation : IAnnotation
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        Attribute Attribute { get; }
    }
}