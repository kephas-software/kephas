// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandIdentityResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICommandIdentityResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for the service resolving the executin command identity.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ICommandIdentityResolver
    {
        /// <summary>
        /// Resolves the identity of the processing context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An IIdentity.
        /// </returns>
        IIdentity ResolveIdentity(IContext context);
    }
}
