// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromCompositionAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Indicates that a specific service should be excluded from composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a class eligible as an application service implementation to be excluded from composition.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromCompositionAttribute : Attribute
    {
    }
}