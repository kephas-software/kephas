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
    /// Values that represent entity part kinds.
    /// </summary>
    public enum EntityPartKind
    {
        /// <summary>
        /// The part is a structural component of the containing entity.
        /// </summary>
        Structural,

        /// <summary>
        /// The part is tightly related to the containing entity, but can also be autonomous.
        /// </summary>
        Loose,
    }

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
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPartAttribute"/> class.
        /// </summary>
        public EntityPartAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPartAttribute"/> class.
        /// </summary>
        /// <param name="kind">The entity part kind.</param>
        protected EntityPartAttribute(EntityPartKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of the entity part.
        /// </summary>
        /// <value>
        /// The entity part kind.
        /// </value>
        public EntityPartKind Kind { get; }
    }
}