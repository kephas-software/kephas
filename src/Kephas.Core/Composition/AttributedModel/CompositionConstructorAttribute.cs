// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionConstructorAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Specifies that a constructor should be used when constructing an attributed part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.AttributedModel
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
    public sealed class CompositionConstructorAttribute : Attribute
    {
    }
}