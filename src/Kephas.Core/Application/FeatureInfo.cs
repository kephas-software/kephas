// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the feature information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Provides information about an application feature.
    /// </summary>
    public class FeatureInfo : Expando, IFeatureInfo
    {
        /// <summary>
        /// The empty annotations collection.
        /// </summary>
        private static readonly IEnumerable<object> EmptyAnnotations = new ReadOnlyCollection<object>(new List<object>());

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfo"/> class.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="dependencies">The feature dependencies (optional).</param>
        public FeatureInfo(string name, string[] dependencies = null)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            this.Name = name;
            this.Dependencies = dependencies ?? new string[0];
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the feature dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public string[] Dependencies { get; }

        /// <summary>
        /// Full name of the <see cref="FeatureInfo"/>.
        /// </summary>
        string IElementInfo.FullName => this.Name;

        /// <summary>
        /// Annotations of the <see cref="FeatureInfo"/>.
        /// </summary>
        IEnumerable<object> IElementInfo.Annotations => EmptyAnnotations;

        /// <summary>
        /// Declaring container of the <see cref="FeatureInfo"/>.
        /// </summary>
        IElementInfo IElementInfo.DeclaringContainer => null;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var deps = string.Join(", ", this.Dependencies ?? new string[0]);
            return $"{this.Name}({deps})";
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
            return new TAttribute[0];
        }
    }
}