// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCommandIdentityResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null command identity resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// A null command identity resolver.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullCommandIdentityResolver : ICommandIdentityResolver
    {
        /// <summary>
        /// Resolves the identity of the processing context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An IIdentity.
        /// </returns>
        public IIdentity? ResolveIdentity(IContext? context) => null;
    }
}
