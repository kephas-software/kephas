// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandController.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.AspNetCore.Controllers;
    using Kephas.Commands;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// A controller for handling commands.
    /// </summary>
    [Route("api/cmd/{command}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public class CommandController : AuthenticatedControllerBase
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandController"/> class.
        /// </summary>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public CommandController(
            ICommandProcessor commandProcessor,
            IContextFactory contextFactory,
            IAuthenticationService authenticationService,
            ILogManager? logManager = null)
            : base(authenticationService, logManager)
        {
            this.commandProcessor = commandProcessor;
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Action that handles HTTP GET requests.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// An asynchronous result that yields the message response.
        /// </returns>
        [HttpGet]
        public Task<object?> GetAsync(string command)
        {
            var args = this.Request.Query.ToDictionary(kv => kv.Key, kv => (object?)kv.Value.ToString());

            return this.ProcessCoreAsync(command, new Args(args));
        }

        /// <summary>
        /// Process the message asynchronously.
        /// </summary>
        /// <exception cref="Exception">Thrown when a Messaging error condition occurs.</exception>
        /// <param name="command">The command.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the process result.
        /// </returns>
        protected async Task<object?> ProcessCoreAsync(string command, IExpando? args, CancellationToken cancellationToken = default)
        {
            try
            {
                if (command == null)
                {
                    throw new Exception($"The command provided is null or is not recognized by the application. "
                                                 + $"Check the command sent by the client component: '{command} {args}'. "
                                                 + $"Possible cause: an application component is not properly installed (missing plugin?).");
                }

                var identity = await this.GetSessionIdentityAsync(cancellationToken).PreserveThreadContext();
                using var context = this.contextFactory.CreateContext<Context>()
                                        .Impersonate(identity);
                var response = await this.commandProcessor.ProcessAsync(
                    command,
                    args,
                    context,
                    cancellationToken).PreserveThreadContext();
                return response;
            }
            catch (Exception ex)
            {
                return new ExceptionData(ex);
            }
        }
    }
}
