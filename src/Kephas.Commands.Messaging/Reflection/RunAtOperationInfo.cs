// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunAtOperationInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Commands.Messaging.Endpoints;
    using Kephas.Dynamic;
    using Kephas.Messaging.Distributed;
    using Kephas.Reflection.Dynamic;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Operation info for running a command by a specific application instance.
    /// </summary>
    public class RunAtOperationInfo : DynamicOperationInfo
    {
        /// <summary>
        /// The argument for indicating the app ID where the command should be executed.
        /// </summary>
        public static readonly string RunAtArg = "@runat";

        private readonly Lazy<IMessageBroker> lazyMessageBroker;
        private readonly string command;
        private readonly IExpando? args;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunAtOperationInfo"/> class.
        /// </summary>
        /// <param name="lazyMessageBroker">The lazy message broker.</param>
        /// <param name="runAt">The application instance where the command should be executed.</param>
        /// <param name="command">The command to be executed.</param>
        /// <param name="args">The command arguments.</param>
        public RunAtOperationInfo(Lazy<IMessageBroker> lazyMessageBroker, object runAt, string command, IExpando? args)
        {
            this.lazyMessageBroker = lazyMessageBroker;
            this.Endpoint = this.GetEndpoint(runAt);
            this.command = command;
            this.args = args;
        }

        /// <summary>
        /// Gets the endpoint where the command should be executed.
        /// </summary>
        public IEndpoint Endpoint { get; }

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public override object? Invoke(object? instance, IEnumerable<object?> args)
        {
            return this.ProcessAsync(instance, args);
        }

        /// <summary>
        /// Executes the command remote asynchronously.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>An asynchronous result yielding the execution return value.</returns>
        protected virtual async Task<object?> ProcessAsync(object? instance, IEnumerable<object?> args)
        {
            var argsList = args?.ToArray() ?? Array.Empty<object?>();

            var opArgs = argsList.Length > 0 ? (IExpando?)argsList[0] : null;
            var opContext = argsList.Length > 1 ? (IContext?)argsList[1] : null;
            var opToken = argsList.Length > 2 ? (CancellationToken?)argsList[2] : default;

            var responseMessage = await this.lazyMessageBroker.Value.ProcessAsync(
                new ExecuteCommandMessage
                {
                    Command = this.command,
                    Args = (opArgs ?? this.args)?.AsArgs(),
                },
                ctx => ctx.To(this.Endpoint).Impersonate(opContext),
                opToken ?? default).PreserveThreadContext();

            var returnValue = responseMessage is ExecuteCommandResponseMessage response
                ? response.ReturnValue
                : responseMessage;
            return returnValue;
        }

        /// <summary>
        /// Infers the endpoint from the '@runat' argument value.
        /// </summary>
        /// <param name="runAt">The '@runat' argument value.</param>
        /// <returns>The endpoint.</returns>
        protected virtual IEndpoint GetEndpoint(object runAt)
        {
            var runAtEndpoint = runAt switch
            {
                Endpoint endpoint => endpoint,
                Uri uri => new Endpoint(uri),
                string appInstanceIdOrUri =>
                    Uri.TryCreate(appInstanceIdOrUri, UriKind.Absolute, out var endpointUri)
                        ? new Endpoint(endpointUri)
                        : new Endpoint(appInstanceId: appInstanceIdOrUri),
                _ => throw new InvalidOperationException(
                    $"RunAt parameter '{runAt}' not supported ({runAt?.GetType()})."),
            };
            return runAtEndpoint;
        }
    }
}