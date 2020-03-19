// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeMethodInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for dynamically invoking a method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="MethodInfo"/>.
    /// </summary>
    public interface IRuntimeMethodInfo : IOperationInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        new IRuntimeTypeInfo ReturnType { get; }

        /// <summary>
        /// Gets the runtime parameters.
        /// </summary>
        /// <value>
        /// The runtime parameters.
        /// </value>
        new IDictionary<string, IRuntimeParameterInfo> Parameters { get; }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <value>
        /// The method information.
        /// </value>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this method is static.
        /// </summary>
        /// <value>
        /// True if this method is static, false if not.
        /// </value>
        bool IsStatic { get; }

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The invocation result.</param>
        /// <returns>A boolean value indicating whether the invocation was successful or not.</returns>
        bool TryInvoke(object instance, IEnumerable<object> args, out object result);
    }
}
