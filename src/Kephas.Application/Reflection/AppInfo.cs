// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Information about the application.
    /// </summary>
    public class AppInfo : Expando, IAppInfo
    {
        private static readonly IEnumerable<object> EmptyAnnotations = new ReadOnlyCollection<object>(new List<object>());
        private static readonly IEnumerable<IParameterInfo> EmptyParameters = new ReadOnlyCollection<IParameterInfo>(new List<IParameterInfo>());

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">Optional. The version.</param>
        public AppInfo(string name, Version version = null)
        {
            this.Name = name;
            this.Version = version;
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
        /// Gets the application parameters.
        /// </summary>
        /// <value>
        /// The application parameters.
        /// </value>
        IEnumerable<IParameterInfo> IAppInfo.Parameters => EmptyParameters;

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
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        IEnumerable<TAttribute> IAttributeProvider.GetAttributes<TAttribute>() => new TAttribute[0];
    }
}
