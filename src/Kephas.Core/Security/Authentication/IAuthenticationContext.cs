// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthenticationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for authentication context.
    /// </summary>
    public interface IAuthenticationContext : IContext
    {
        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        ICredentials? Credentials { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        bool ThrowOnFailure { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IAuthenticationContext"/>.
    /// </summary>
    public static class AuthenticationContextExtensions
    {
        /// <summary>
        /// Sets a value indicating whether to throw on failure.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the authentication context.</typeparam>
        /// <param name="context">The authentication context.</param>
        /// <param name="value">True to throw on failure, false otherwise.</param>
        /// <returns>
        /// This <see cref="IAuthenticationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ThrowOnFailure<TContext>(this TContext context, bool value)
            where TContext : class, IAuthenticationContext
        {
            Requires.NotNull(context, nameof(context));

            context.ThrowOnFailure = value;
            return context;
        }
    }
}