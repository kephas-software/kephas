// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicOperationInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic operation information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the dynamic operation.
    /// </summary>
    public class DynamicOperationInfo : DynamicElementInfo, IOperationInfo
    {
        /// <summary>
        /// Gets or sets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo? ReturnType { get; protected internal set; }

        /// <summary>
        /// Gets or sets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        public IEnumerable<IParameterInfo> Parameters { get; protected internal set; } = new List<IParameterInfo>();

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public virtual object? Invoke(object? instance, IEnumerable<object?> args)
        {
            // TODO localization
            throw new NotSupportedException();
        }
    }
}