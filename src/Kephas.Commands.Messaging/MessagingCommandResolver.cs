// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging
{
    using System;
    using System.Collections.Generic;

    using Kephas.Commands.Messaging.Reflection;
    using Kephas.Dynamic;
    using Kephas.Messaging.Distributed;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// Command resolver when messaging is available.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class MessagingCommandResolver : DefaultCommandResolver
    {
        private readonly Lazy<IMessageBroker> lazyMessageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommandResolver"/> class.
        /// </summary>
        /// <param name="lazyMessageBroker">The lazy message broker.</param>
        /// <param name="registries">The command registries.</param>
        public MessagingCommandResolver(
            Lazy<IMessageBroker> lazyMessageBroker,
            ICollection<Lazy<ICommandRegistry, AppServiceMetadata>> registries)
            : base(registries)
        {
            this.lazyMessageBroker = lazyMessageBroker;
        }

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
        public override IOperationInfo? ResolveCommand(string command, IExpando? args = null, bool throwOnNotFound = true)
        {
            var runAt = args?[RunAtOperationInfo.RunAtArg];
            if (runAt == null)
            {
                return base.ResolveCommand(command, args, throwOnNotFound);
            }

            args![RunAtOperationInfo.RunAtArg] = null;
            return new RunAtOperationInfo(this.lazyMessageBroker, runAt, command, args);
        }
    }
}