// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base class for command processors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default implementation of a command processor.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultCommandProcessor : ICommandProcessor
    {
        private readonly ICommandResolver resolver;
        private readonly ICommandIdentityResolver identityResolver;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandProcessor"/> class.
        /// </summary>
        /// <param name="resolver">The command resolver.</param>
        /// <param name="identityResolver">The identity resolver.</param>
        /// <param name="contextFactory">The context factory.</param>
        public DefaultCommandProcessor(
            ICommandResolver resolver,
            ICommandIdentityResolver identityResolver,
            IContextFactory contextFactory)
        {
            Requires.NotNull(resolver, nameof(resolver));
            Requires.NotNull(identityResolver, nameof(identityResolver));
            Requires.NotNull(contextFactory, nameof(contextFactory));

            this.resolver = resolver;
            this.identityResolver = identityResolver;
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Processes the provided command with the provided arguments and returns the result.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The asynchronous result returning the output of the command execution.
        /// </returns>
        public virtual async Task<object?> ProcessAsync(string command, IExpandoBase? args = null, IContext? context = null, CancellationToken cancellationToken = default)
        {
            var commandInfo = this.resolver.ResolveCommand(command, args);
            var ownsContext = context == null;
            if (ownsContext)
            {
                context = this.contextFactory.CreateContext<Context>();
            }

            if (context!.Identity == null)
            {
                context.Impersonate(this.identityResolver.ResolveIdentity(context));
            }

            object? result;
            if (commandInfo is IPrototype commandPrototype)
            {
                var operation = (IOperation)commandPrototype.CreateInstance(new object?[] { args });
                result = await operation.ExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }
            else
            {
                result = await commandInfo!.InvokeAsync(null, new object?[] { args, context, cancellationToken }).PreserveThreadContext();
            }

            if (ownsContext)
            {
                context.Dispose();
            }

            return result;
        }
    }
}
