// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Activation
{
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Contract for a service used for object instantiation based on <see cref="ITypeInfo"/>.
    /// </summary>
    public interface IActivator
    {
        /// <summary>
        /// Creates an instance of the provided <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">Indicates the <see cref="ITypeInfo"/> used for the instantiation.</param>
        /// <param name="args">Constructor arguments.</param>
        /// <param name="activationContext">Context for the activation.</param>
        /// <remarks>
        /// The <paramref name="typeInfo"/> may be either an implementation type or an abstract type.
        /// If an abstract type is provided, then an implementation type is determined first and then instantiated.
        /// </remarks>
        /// <returns>
        /// An instance of the provided <see cref="ITypeInfo"/>.
        /// </returns>
        object CreateInstance(ITypeInfo typeInfo, IEnumerable<object> args = null, IContext activationContext = null);

        /// <summary>
        /// Gets the type implementing the abstract type provided as the parameter.
        /// </summary>
        /// <param name="abstractType">Indicates the abstract type.</param>
        /// <param name="activationContext">Context for the activation.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw an exception if an implementation type is not found.</param>
        /// <returns>
        /// The implementation type for the provided <see cref="ITypeInfo"/>.
        /// </returns>
        ITypeInfo GetImplementationType(ITypeInfo abstractType, IContext activationContext = null, bool throwOnNotFound = true);
    }
}