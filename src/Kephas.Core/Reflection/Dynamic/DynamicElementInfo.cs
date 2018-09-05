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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Runtime;

    /// <summary>
    /// Dynamic element information.
    /// </summary>
    public abstract class DynamicElementInfo : Expando, IElementInfo
    {
        /// <summary>
        /// The annotations.
        /// </summary>
        private readonly IList<object> annotations = new List<object>();

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name { get; protected internal set; }

        /// <summary>
        /// Gets or sets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public virtual string FullName { get; protected internal set; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public IEnumerable<object> Annotations => this.annotations;

        /// <summary>
        /// Gets or sets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer { get; protected internal set; }

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
            attributes.AddRange(this.Annotations.OfType<IAttributeProvider>().SelectMany(a => a.GetAttributes<TAttribute>()));
            return attributes;
        }

        /// <summary>
        /// Adds an annotation to the dynamic element.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        protected internal virtual void AddAnnotation(object annotation)
        {
            Requires.NotNull(annotation, nameof(annotation));

            this.annotations.Add(annotation);
        }
    }
}