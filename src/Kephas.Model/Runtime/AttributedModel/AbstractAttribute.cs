// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks a classifier as being abstract (cannot be instantiated).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a classifier as being abstract (cannot be instantiated).
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AbstractAttribute : Attribute
    {
    }
}