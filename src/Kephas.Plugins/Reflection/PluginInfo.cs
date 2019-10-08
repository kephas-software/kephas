// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Information about the plugin.
    /// </summary>
    public class PluginInfo : Expando, IPluginInfo
    {
        private static readonly IEnumerable<object> EmptyAnnotations = new ReadOnlyCollection<object>(new List<object>());

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">Optional. the version.</param>
        /// <param name="description">Optional. The description.</param>
        public PluginInfo(string name, Version version = null, string description = null)
        {
            this.Name = name;
            this.Version = version;
            this.Description = description;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>
        /// The application name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public Version Version { get; }

        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        public IEnumerable<IPluginDependency> Dependencies => throw new NotImplementedException();

        /// <summary>
        /// Gets the application full name.
        /// </summary>
        /// <value>
        /// The application full name.
        /// </value>
        string IElementInfo.FullName => this.Name;

        /// <summary>
        /// Gets the annotations.
        /// </summary>
        /// <value>
        /// The annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => EmptyAnnotations;

        /// <summary>
        /// Gets the declaring container.
        /// </summary>
        /// <value>
        /// The declaring container.
        /// </value>
        IElementInfo IElementInfo.DeclaringContainer => null;

        /// <summary>
        /// Gets the attributes in this collection.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the attributes in this collection.
        /// </returns>
        IEnumerable<TAttribute> IAttributeProvider.GetAttributes<TAttribute>() => new TAttribute[0];
    }
}
