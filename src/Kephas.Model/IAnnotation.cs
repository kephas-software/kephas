﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for model annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Contract for model annotations.
    /// </summary>
    /// <remarks>
    /// Annotations have names starting with @ (the at sign).
    /// </remarks>
    [MemberNameDiscriminator("@")]
    public interface IAnnotation : INamedElement
    {
        /// <summary>
        /// Gets a value indicating whether multiple annotations of the same kind are allowed to be placed the same model element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple annotations of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple annotations of the same kind are allowed, the qualified name will have a generated suffix
        /// to allow the annotation to be unique within the members collection.
        /// </remarks>
        bool AllowMultiple { get; }
    }
}