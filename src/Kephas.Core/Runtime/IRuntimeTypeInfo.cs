// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for a dynamic <see cref="TypeInfo" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="TypeInfo"/>.
    /// </summary>
    public interface IRuntimeTypeInfo : ITypeInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Gets the underlying <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; }

        /// <summary>
        /// Gets the underlying <see cref="TypeInfo"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        TypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the default value of the type.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        object? DefaultValue { get; }

        /// <summary>
        /// Gets the runtime type kind.
        /// </summary>
        /// <value>
        /// The runtime type kind.
        /// </value>
        RuntimeTypeKind Kind { get; }

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        new IDictionary<string, IRuntimeElementInfo> Members { get; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        IDictionary<string, IRuntimeFieldInfo> Fields { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        new IDictionary<string, IRuntimePropertyInfo> Properties { get; }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        IDictionary<string, ICollection<IRuntimeMethodInfo>> Methods { get; }

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the specified property.
        /// </returns>
        /// <remarks>
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        object? GetValue(object? instance, string propertyName);

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The property value.</param>
        /// <returns>
        /// A boolean value indicating whether the property is found.
        /// </returns>
        bool TryGetValue(object? instance, string propertyName, out object? value);

        /// <summary>
        /// Sets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        void SetValue(object? instance, string propertyName, object? value);

        /// <summary>
        /// Tries to set the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value could be set; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If a property with the provided name is not found, the method just returns.
        /// Also, the method just returns if the instance passed is <c>null</c>.
        /// </remarks>
        bool TrySetValue(object? instance, string propertyName, object? value);

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        object? Invoke(object? instance, string methodName, IEnumerable<object?> args);

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The invocation result.</param>
        /// <returns>A boolean value indicating whether the invocation was successful or not.</returns>
        bool TryInvoke(object? instance, string methodName, IEnumerable<object?> args, out object? result);
    }
}
