// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityPartAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity part attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a navigation property as modelling an entity part,
    /// meaning that the property content is an aggregated part of the declaring entity.
    /// If applied to an entity, marks it as being a second class entity used only
    /// as an aggregate of another entity.
    /// </summary>
    /// <remarks>
    /// Entity parts can be both self contained entities or collections.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class EntityPartAttribute : Attribute
    {
    }
}