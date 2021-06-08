// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for resolving commands.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ICommandResolver
    {
        /// <summary>
        /// Resolves the command based on the command name.
        /// </summary>
        /// <param name="command">The command name.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="throwOnNotFound">
        ///     If true, an exception will be thrown if a command could not be resolved,
        ///     otherwise <c>null</c> will be returned.
        /// </param>
        /// <returns>The command as an <see cref="IOperation"/> or <c>null</c>.</returns>
        IOperationInfo? ResolveCommand(string command, IExpandoBase? args = null, bool throwOnNotFound = true);
    }
}