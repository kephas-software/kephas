// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixinAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks a classifier as being a mixin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a classifier as being a mixin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class MixinAttribute : Attribute
    {
    }
}