// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicMethod.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamically invoking a method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Contract for dynamically invoking a method.
    /// </summary>
    public interface IDynamicMethod
    {
        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <value>
        /// The method information.
        /// </value>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        object Invoke(object instance, IEnumerable<object> args);

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result, if the method exists, otherwise <see cref="Undefined.Value"/>.</returns>
        object TryInvoke(object instance, IEnumerable<object> args);
    }
}
