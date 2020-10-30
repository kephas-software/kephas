// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac
{
    using System;

    using global::Autofac;
    using global::Autofac.Core.Resolving;

    using Kephas.Composition.Autofac.Resources;

    /// <summary>
    /// Helper class for Autofac.
    /// </summary>
    public static class AutofacHelper
    {
        /// <summary>
        /// Gets the lifetime scope from a component context.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="c">An IComponentContext to process.</param>
        /// <returns>
        /// The lifetime scope.
        /// </returns>
        public static ILifetimeScope GetLifetimeScope(this IComponentContext c)
        {
            return c switch
            {
                ILifetimeScope lifetimeScope => lifetimeScope,
                IInstanceLookup instanceLookup => instanceLookup.ActivationScope,
                _ => throw new InvalidOperationException(
                    string.Format(Strings.AutofacCompositionContainer_MismatchedLifetimeScope_Exception, c))
            };
        }
    }
}
