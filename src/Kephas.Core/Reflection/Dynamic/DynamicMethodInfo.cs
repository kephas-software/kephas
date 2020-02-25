// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicMethodInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic method information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the dynamic method.
    /// </summary>
    public class DynamicMethodInfo : DynamicElementInfo, IMethodInfo
    {
        /// <summary>
        /// Gets or sets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo ReturnType { get; protected internal set; }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        public IEnumerable<IParameterInfo> Parameters { get; } = new List<IParameterInfo>();

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public object? Invoke(object instance, IEnumerable<object> args)
        {
            // TODO localization
            throw new NotSupportedException();
        }
    }
}