// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Kephas.Services;

    /// <summary>
    /// Interface for authorization context.
    /// </summary>
    public interface IAuthorizationContext : IContext
    {
        /// <summary>
        /// Gets the required permissions.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        IEnumerable<object> RequiredPermissions { get; }

        /// <summary>
        /// Gets the authorization scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        object? Scope { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on authorization failure.
        /// If <c>false</c> is indicated, the authorization check will return <c>false</c> upon failure,
        /// otherwise an exception will occur.
        /// </summary>
        /// <value>
        /// True to throw on authorization failure, false to not throw and return <c>false</c>.
        /// </value>
        bool ThrowOnFailure { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IAuthorizationContext"/>.
    /// </summary>
    public static class AuthorizationContextExtensions
    {
        /// <summary>
        /// Sets a value indicating whether to throw on failure.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the authorization context.</typeparam>
        /// <param name="context">The authorization context.</param>
        /// <param name="value">True to throw on failure, false otherwise.</param>
        /// <returns>
        /// This <see cref="IAuthorizationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ThrowOnFailure<TContext>(this TContext context, bool value)
            where TContext : class, IAuthorizationContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.ThrowOnFailure = value;
            return context;
        }
    }
}