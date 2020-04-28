// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Commands.Resources;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// The default implementation of a command resolver.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultCommandResolver : ICommandResolver
    {
        private readonly ICollection<ICommandRegistry> registries;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandResolver"/> class.
        /// </summary>
        /// <param name="registries">The command registries.</param>
        public DefaultCommandResolver(ICollection<Lazy<ICommandRegistry, AppServiceMetadata>> registries)
        {
            this.registries = registries.Order()
                .Select(r => r.Value)
                .ToList();
        }

        /// <summary>
        /// Resolves the command based on the command name.
        /// </summary>
        /// <param name="command">The command name.</param>
        /// <param name="throwOnNotFound">
        ///     If true, an exception will be thrown if a command could not be resolved,
        ///     otherwise <c>null</c> will be returned.
        /// </param>
        /// <returns>The command as an <see cref="IOperation"/> or <c>null</c>.</returns>
        public virtual IOperationInfo? ResolveCommand(string command, bool throwOnNotFound = true)
        {
            var partiallyMatchingCommands = this.registries
                .SelectMany(r => r.GetCommandTypes(command))
                .ToList();

            var matchingCommands = partiallyMatchingCommands
                .Where(c => c.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (matchingCommands.Count == 0)
            {
                matchingCommands = partiallyMatchingCommands;
            }

            if (matchingCommands.Count == 0)
            {
                return throwOnNotFound
                    ? throw new KeyNotFoundException(Strings.DefaultCommandRegistry_CommandNotFound.FormatWith(command))
                    : (IOperationInfo?)null;
            }

            if (matchingCommands.Count > 1)
            {
                throw new AmbiguousMatchException(Strings.DefaultCommandRegistry_AmbiguousCommandName.FormatWith(command, string.Join("', '", matchingCommands.Select(m => m.Name))));
            }

            return matchingCommands[0];
        }
    }
}