// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCommandRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the NullCommandRegistry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A null command registry.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullCommandRegistry : ICommandRegistry
    {
        /// <summary>
        /// Gets the command types.
        /// </summary>
        /// <param name="commandPattern">Optional. A pattern specifying the command types to retrieve.</param>
        /// <returns>
        /// The command types.
        /// </returns>
        public IEnumerable<ITypeInfo> GetCommandTypes(string? commandPattern = null) => Enumerable.Empty<ITypeInfo>();

        /// <summary>
        /// Resolves the command type.
        /// </summary>
        /// <exception cref="NullServiceException">Thrown always.</exception>
        /// <param name="command">The command.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        public ITypeInfo ResolveCommandType(string command) => throw new NullServiceException(typeof(ICommandRegistry));
    }
}
