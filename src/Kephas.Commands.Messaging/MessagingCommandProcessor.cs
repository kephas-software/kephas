// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging command processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Command processor based on messaging. It packs the command line in messages and delegates execution to the <see cref="IMessageProcessor"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MessagingCommandProcessor : CommandProcessorBase
    {
        private readonly ICommandIdentityResolver identityResolver;
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommandProcessor"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="identityResolver">The identity resolver.</param>
        /// <param name="messageProcessor">The message processor.</param>
        public MessagingCommandProcessor(ICommandRegistry registry, ICommandIdentityResolver identityResolver, IMessageProcessor messageProcessor)
            : base(registry)
        {
            Requires.NotNull(identityResolver, nameof(identityResolver));
            Requires.NotNull(messageProcessor, nameof(messageProcessor));

            this.identityResolver = identityResolver;
            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Process the command asynchronously.
        /// </summary>
        /// <param name="command">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the result.
        /// </returns>
        protected override async Task<object?> ProcessCommandAsync(object command, IExpando? args, IContext? context, CancellationToken cancellationToken)
        {
            var identity = this.identityResolver.ResolveIdentity(context);
            var result = await this.messageProcessor.ProcessAsync(command, ctx => ctx.Impersonate(identity), cancellationToken).PreserveThreadContext();
            return result.GetContent();
        }
    }
}
