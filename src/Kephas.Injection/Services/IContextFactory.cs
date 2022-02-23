// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IContextFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Interface for context factory.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IContextFactory
    {
        /// <summary>
        /// Creates a typed context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The new context.
        /// </returns>
        [return: NotNull]
        TContext CreateContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContext>(params object?[] args)
            where TContext : class;
    }
}
