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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default implementation of a command processor.
    /// </summary>
    [Priority(Priority.Low)]
    public class DefaultCommandProcessor : ICommandProcessor
    {
        private readonly ICommandResolver resolver;
        private readonly ICommandIdentityResolver identityResolver;
        private readonly IServiceFactory serviceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandProcessor"/> class.
        /// </summary>
        /// <param name="resolver">The command resolver.</param>
        /// <param name="identityResolver">The identity resolver.</param>
        /// <param name="serviceFactory">The injectable factory.</param>
        public DefaultCommandProcessor(
            ICommandResolver resolver,
            ICommandIdentityResolver identityResolver,
            IServiceFactory serviceFactory)
        {
            resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            identityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            this.resolver = resolver;
            this.identityResolver = identityResolver;
            this.serviceFactory = serviceFactory;
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
        public virtual async Task<object?> ProcessAsync(string command, IDynamic? args = null, IContext? context = null, CancellationToken cancellationToken = default)
        {
            var commandInfo = this.resolver.ResolveCommand(command, args);
            var ownsContext = context == null;
            if (ownsContext)
            {
                context = this.serviceFactory.Create<Context>();
            }

            if (context!.Identity == null)
            {
                context.Impersonate(this.identityResolver.ResolveIdentity(context));
            }

            object? result;
            if (commandInfo is IPrototype commandPrototype)
            {
                var operation = (IOperation)commandPrototype.CreateInstance(new object?[] { args });
                result = await operation.ExecuteAsync(context, cancellationToken);
            }
            else
            {
                result = await commandInfo!.InvokeAsync(null, new object?[] { args, context, cancellationToken });
            }

            if (ownsContext)
            {
                context.Dispose();
            }

            return result;
        }
    }
}
