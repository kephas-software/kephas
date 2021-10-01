// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationScopeContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationScopeContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for authorization scope context.
    /// </summary>
    public interface IAuthorizationScopeContext : IContext
    {
        /// <summary>
        /// Gets or sets the calling context.
        /// </summary>
        /// <value>
        /// The calling context.
        /// </value>
        IContext CallingContext { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IAuthorizationScopeContext"/>.
    /// </summary>
    public static class AuthorizationScopeContextExtensions
    {
        /// <summary>
        /// Sets a value indicating whether to throw on failure.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the authorization context.</typeparam>
        /// <param name="context">The authorization context.</param>
        /// <param name="callingContext">The calling context.</param>
        /// <returns>
        /// This <see cref="IAuthorizationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ThrowOnFailure<TContext>(this TContext context, IContext callingContext)
            where TContext : class, IAuthorizationScopeContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            Requires.NotNull(callingContext, nameof(callingContext));

            context.CallingContext = callingContext;
            return context;
        }
    }
}
