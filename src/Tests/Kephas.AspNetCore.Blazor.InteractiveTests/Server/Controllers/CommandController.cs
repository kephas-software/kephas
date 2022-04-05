// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandController.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Server.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Commands;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// A controller for handling commands.
    /// </summary>
    [Route("api/cmd/{command}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class CommandController : ControllerBase
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly IInjectableFactory injectableFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandController"/> class.
        /// </summary>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="injectableFactory">The injectable factory.</param>
        public CommandController(
            ICommandProcessor commandProcessor,
            IInjectableFactory injectableFactory)
        {
            this.commandProcessor = commandProcessor;
            this.injectableFactory = injectableFactory;
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
        protected async Task<object?> ProcessCoreAsync(string command, IDynamic? args, CancellationToken cancellationToken = default)
        {
            try
            {
                if (command == null)
                {
                    throw new Exception($"The command provided is null or is not recognized by the application. "
                                                 + $"Check the command sent by the client component: '{command} {args}'. "
                                                 + $"Possible cause: an application component is not properly installed (missing plugin?).");
                }

                using var context = this.injectableFactory.Create<Context>();
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
