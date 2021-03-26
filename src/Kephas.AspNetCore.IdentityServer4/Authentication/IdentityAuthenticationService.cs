// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityAuthenticationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Authentication
{
    using System;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.AspNetCore.IdentityServer4.Stores;
    using Kephas.Dynamic;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// An authentication service based on the IdentityUser.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class IdentityAuthenticationService : IdentityAuthenticationService<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityAuthenticationService"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="lazyPasswordHasher">The lazy password hasher.</param>
        /// <param name="lazyUserStore">The lazy user store.</param>
        public IdentityAuthenticationService(
            IContextFactory contextFactory,
            Lazy<IPasswordHasher<IdentityUser>> lazyPasswordHasher,
            Lazy<IUserStoreService<IdentityUser>> lazyUserStore)
            : base(contextFactory, lazyPasswordHasher, lazyUserStore)
        {
        }
    }

    /// <summary>
    /// Base class for <see cref="IAuthenticationService"/> implementations.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    public abstract class IdentityAuthenticationService<TUser> : IAuthenticationService
        where TUser : class
    {
        private readonly Lazy<IPasswordHasher<TUser>> lazyPasswordHasher;
        private readonly Lazy<IUserStoreService<TUser>> lazyUserStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityAuthenticationService{TUser}"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="lazyPasswordHasher">The lazy password hasher.</param>
        /// <param name="lazyUserStore">The lazy user store.</param>
        protected IdentityAuthenticationService(
            IContextFactory contextFactory,
            Lazy<IPasswordHasher<TUser>> lazyPasswordHasher,
            Lazy<IUserStoreService<TUser>> lazyUserStore)
        {
            this.lazyPasswordHasher = lazyPasswordHasher;
            this.lazyUserStore = lazyUserStore;
            this.ContextFactory = contextFactory;
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets the password hasher.
        /// </summary>
        protected IPasswordHasher<TUser> PasswordHasher => this.lazyPasswordHasher.Value;

        /// <summary>
        /// Gets the user store.
        /// </summary>
        /// <value>The user store service.</value>
        protected IUserStoreService<TUser> UserStore => this.lazyUserStore.Value;

        /// <summary>
        /// Authenticates the user asynchronously.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">Optional. The authentication configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public virtual async Task<IIdentity?> AuthenticateAsync(
            ICredentials credentials,
            Action<IAuthenticationContext>? authConfig = null,
            CancellationToken cancellationToken = default)
        {
            string? userName = null;
            string? passwordHash = null;
            string? password = null;
            switch (credentials)
            {
                case UserPasswordHashCredentials hashCredentials:
                    userName = hashCredentials.UserName;
                    passwordHash = Encoding.UTF8.GetString(hashCredentials.PasswordHash);
                    break;
                case UserPasswordCredentials pwdCredentials:
                    userName = pwdCredentials.UserName;
                    password = pwdCredentials.Password;
                    break;
                default:
                    throw new AuthenticationException($"Credentials type '{credentials?.GetType()}' not supported. Use one of '{typeof(UserPasswordHashCredentials)}' or '{typeof(UserPasswordCredentials)}' instead.");
            }

            using var context = this.ContextFactory.CreateContext<AuthenticationContext>().Merge(authConfig);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName),
            });
            var user = await this.TryGetUserByIdentityAsync(identity, context, cancellationToken)
                .PreserveThreadContext();

            if (user == null)
            {
                return null;
            }

            var userStore = this.UserStore as IUserPasswordStore<TUser>;
            if (userStore == null)
            {
                throw new NotSupportedException($"The user store does not implement the {nameof(IUserPasswordStore<TUser>)} interface.");
            }

            if (password != null)
            {
                passwordHash = this.PasswordHasher.HashPassword(user, password);
            }

            var storedPasswordHash = await userStore.GetPasswordHashAsync(user, cancellationToken).PreserveThreadContext();
            if (passwordHash != storedPasswordHash)
            {
                return null;
            }

            return await this.ToIdentityAsync(user, userStore, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Gets asynchronously the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public virtual async Task<IIdentity?> GetIdentityAsync(
            object token,
            Action<IAuthenticationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            using var context = this.ContextFactory.CreateContext<AuthenticationContext>().Merge(optionsConfig);

            var userId = this.ParseId(
                token is ClaimsPrincipal appUserPrincipal
                    ? appUserPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    : token);

            IIdentity? identity = null;
            identity = userId switch
            {
                long appUserIdLong => await this.TryGetIdentityByIdAsync(appUserIdLong, context, cancellationToken)
                    .PreserveThreadContext(),
                Guid appUserIdGuid => await this.TryGetIdentityByGuidAsync(appUserIdGuid, context, cancellationToken)
                    .PreserveThreadContext(),
                string appUserIdString => appUserIdString.IndexOf('@') > 0
                    ? await this.TryGetIdentityByEmailAsync(appUserIdString, context, cancellationToken)
                        .PreserveThreadContext()
                    : await this.TryGetIdentityByNameAsync(appUserIdString, context, cancellationToken)
                        .PreserveThreadContext(),
                _ => identity
            };

            // add also the claims from the original identity.
            if (identity is ClaimsIdentity claimsIdentityValue && token is ClaimsPrincipal claimsPrincipal)
            {
                foreach (var claimsIdentity in claimsPrincipal.Identities)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        claimsIdentityValue.AddClaim(claim);
                    }
                }
            }

            return identity;
        }

        /// <summary>
        /// Gets asynchronously a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the token.
        /// </returns>
        public virtual Task<object?> GetTokenAsync(
            IIdentity identity,
            Action<IAuthenticationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object?>(this.GetUserId(identity) ?? this.GetUserName(identity) ?? this.GetUserEmail(identity));
        }

        /// <summary>
        /// Tries to the get identity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the identity.</returns>
        protected virtual async Task<IIdentity?> TryGetIdentityByIdAsync(long id, IAuthenticationContext? context, CancellationToken cancellationToken)
        {
            var userStore = this.UserStore;
            var user = await userStore.FindByIdAsync(id.ToString(), cancellationToken).PreserveThreadContext();
            return user == null ? null : await this.ToIdentityAsync(user, userStore, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Tries to get the identity by unique identifier.
        /// </summary>
        /// <param name="guid">The identifier.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the identity.</returns>
        protected virtual async Task<IIdentity?> TryGetIdentityByGuidAsync(Guid guid, IAuthenticationContext? context, CancellationToken cancellationToken)
        {
            var userStore = this.UserStore;
            var user = await userStore.FindByIdAsync(guid.ToString(), cancellationToken).PreserveThreadContext();
            return user == null ? null : await this.ToIdentityAsync(user, userStore, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Tries to get the identity by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the identity.</returns>
        protected virtual async Task<IIdentity?> TryGetIdentityByNameAsync(string name, IAuthenticationContext? context, CancellationToken cancellationToken)
        {
            var userStore = this.UserStore;
            var user = await userStore.FindByNameAsync(name, cancellationToken).PreserveThreadContext();
            return user == null ? null : await this.ToIdentityAsync(user, userStore, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Tries to get the identity by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the identity.</returns>
        protected virtual async Task<IIdentity?> TryGetIdentityByEmailAsync(string email, IAuthenticationContext? context, CancellationToken cancellationToken)
        {
            var userStore = this.UserStore;
            if (!(userStore is IUserEmailStore<TUser> emailUserStore))
            {
                return null;
            }

            var user = await emailUserStore.FindByEmailAsync(email, cancellationToken).PreserveThreadContext();
            return user == null ? null : await this.ToIdentityAsync(user, userStore, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Tries to get the user having the <see cref="IIdentity"/>.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the identity.</returns>
        protected virtual async Task<TUser?> TryGetUserByIdentityAsync(IIdentity identity, IAuthenticationContext? context, CancellationToken cancellationToken)
        {
            var userStore = this.UserStore;
            var id = this.GetUserId(identity);
            if (id != null)
            {
                var user = await userStore.FindByIdAsync(id, cancellationToken).PreserveThreadContext();
                if (user != null)
                {
                    return user;
                }
            }

            var name = this.GetUserName(identity);
            if (name != null)
            {
                var user = await userStore.FindByNameAsync(name, cancellationToken).PreserveThreadContext();
                if (user != null)
                {
                    return user;
                }
            }

            var email = this.GetUserEmail(identity);
            if (email != null && userStore is IUserEmailStore<TUser> emailUserStore)
            {
                var user = await emailUserStore.FindByEmailAsync(email, cancellationToken).PreserveThreadContext();
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses the ID providing a normalized value.
        /// </summary>
        /// <param name="token">The ID token.</param>
        /// <returns>A normalized ID value.</returns>
        protected virtual object? ParseId(object? token) =>
            token switch
            {
                null => null,
                string idString when long.TryParse(idString, out var idLong) => idLong,
                string idString when Guid.TryParse(idString, out var idGuid) => idGuid,
                _ => token
            };

        /// <summary>
        /// Converts the user to an <see cref="IIdentity"/>.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userStore">The user store.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The identity.</returns>
        protected virtual async Task<IIdentity> ToIdentityAsync(TUser user, IUserStore<TUser> userStore, IAuthenticationContext? context, CancellationToken cancellationToken)
        {
            var emailUserStore = userStore as IUserEmailStore<TUser>;
            var phoneNumberUserStore = userStore as IUserPhoneNumberStore<TUser>;
            return new ClaimsIdentity(new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        (await userStore.GetUserIdAsync(user, cancellationToken).PreserveThreadContext()) ?? string.Empty),
                    new Claim(
                        ClaimTypes.Name,
                        (await userStore.GetUserNameAsync(user, cancellationToken).PreserveThreadContext()) ?? string.Empty),
                    new Claim(
                        ClaimTypes.Email,
                        emailUserStore == null
                                ? string.Empty
                                : (await emailUserStore.GetEmailAsync(user, cancellationToken).PreserveThreadContext()) ?? string.Empty),
                    new Claim(
                        ClaimTypes.MobilePhone,
                        phoneNumberUserStore == null
                                ? string.Empty
                                : await phoneNumberUserStore.GetPhoneNumberAsync(user, cancellationToken).PreserveThreadContext() ?? string.Empty),
                });
        }

        /// <summary>
        /// Gets the user ID.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>The user ID.</returns>
        protected virtual string? GetUserId(IIdentity identity)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>The user name.</returns>
        protected virtual string? GetUserName(IIdentity identity)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets the user email.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>The user email.</returns>
        protected virtual string? GetUserEmail(IIdentity identity)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            }

            return null;
        }
    }
}