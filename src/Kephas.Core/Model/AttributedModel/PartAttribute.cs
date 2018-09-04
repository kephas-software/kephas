// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the part attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a navigation property (single or collection) as modeling an object part,
    /// meaning that the property content is an aggregated part of the declaring object.
    /// If applied to a class or interface, marks it as being an aggregate of another object,
    /// typically found in the model space.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class PartAttribute : Attribute
    {
    }
}