// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElementAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Attribute marking dimension elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute marking dimension elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ModelDimensionElementAttribute : Attribute
    {
    }
}