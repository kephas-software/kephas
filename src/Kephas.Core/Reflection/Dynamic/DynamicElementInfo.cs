// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicElementInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic element information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Dynamic;
    using Kephas.Runtime;

    /// <summary>
    /// Dynamic element information.
    /// </summary>
    public abstract class DynamicElementInfo : Expando, IElementInfo
    {
        private readonly IList<object> annotations = new List<object>();
        private string? fullName;

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public virtual string FullName
        {
            get => this.fullName ?? this.Name;
            set => this.fullName = value;
        }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.annotations;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public ICollection<object> Annotations => this.annotations;

        /// <summary>
        /// Gets or sets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public virtual IElementInfo? DeclaringContainer { get; protected internal set; }

        /// <summary>
        /// Gets or sets display information.
        /// </summary>
        public virtual DynamicDisplayInfo? Display { get; set; }

        /// <summary>
        /// Gets or sets the position within its container.
        /// </summary>
        protected internal int Position { get; set; }

        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public virtual IDisplayInfo? GetDisplayInfo() => this.Display ??= new DynamicDisplayInfo();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.GetType().Name})";
        }

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            var attributes = new List<TAttribute>(this.Annotations.OfType<TAttribute>());
            attributes.AddRange(this.Annotations.OfType<IAttributeProvider>()
                .SelectMany(a => a.GetAttributes<TAttribute>()));
            return attributes;
        }

        /// <summary>
        /// Tries to get the type navigating through the containers upwards.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns>The type or <c>null</c>.</returns>
        protected virtual ITypeInfo? TryGetType(string? typeName)
        {
            return typeName == null
                ? null
                : this.GetTypeRegistry()?.GetTypeInfo(typeName, throwOnNotFound: false);
        }

        /// <summary>
        /// Tries to get the type registry navigating the declaring containers upwards.
        /// </summary>
        /// <returns>The root type registry or <c>null</c>.</returns>
        protected virtual ITypeRegistry? GetTypeRegistry()
        {
            IElementInfo ancestor = this;
            while (ancestor != null && ancestor is not ITypeRegistry)
            {
                ancestor = ancestor.DeclaringContainer;
            }

            return ancestor as ITypeRegistry;
        }
    }
}