// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectConstructorAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Specifies that a constructor should be used when constructing an attributed part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.AttributedModel
{
    using System;

    /// <summary>
    /// Specifies that a constructor should be used when constructing an attributed part.
    /// </summary>
    /// <remarks>
    /// By default, only a default parameter-less constructor, if available, is used to
    ///                 construct an attributed part. Use this attribute to indicate that a specific constructor
    ///                 should be used.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class InjectConstructorAttribute : Attribute, IInjectConstructorAnnotation
    {
    }
}