// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandShell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command shell class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Commands;
    using Kephas.Composition.AttributedModel;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default command shell.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class DefaultCommandShell : Loggable, ICommandShell
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly ISerializationService serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandShell"/> class.
        /// </summary>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="serializationService">The serialization service.</param>
        public DefaultCommandShell(ICommandProcessor commandProcessor, ISerializationService serializationService)
            : this(new DefaultConsole(), commandProcessor, serializationService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandShell"/> class.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="serializationService">The serialization service.</param>
        [InjectionConstructor]
        public DefaultCommandShell(IConsole console, ICommandProcessor commandProcessor, ISerializationService serializationService)
        {
            Requires.NotNull(console, nameof(console));
            Requires.NotNull(commandProcessor, nameof(commandProcessor));

            this.Console = console;
            this.commandProcessor = commandProcessor;
            this.serializationService = serializationService;
        }

        /// <summary>
        /// Gets the console.
        /// </summary>
        /// <value>
        /// The console.
        /// </value>
        public IConsole Console { get; }

        /// <summary>
        /// Starts processing commands asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual async Task StartAsync(IContext context, CancellationToken cancellationToken)
        {
            this.WriteWelcomeScreen();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.WritePrompt();

                string? commandLine = null;
                try
                {
                    commandLine = await this.ReadCommandLineAsync(cancellationToken).PreserveThreadContext();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while reading from the console.");
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(commandLine))
                {
                    continue;
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var command = this.ParseCommandLine(commandLine);
                    var result = await this.commandProcessor.ProcessAsync(command.Name, command.Args, context, cancellationToken).PreserveThreadContext();
                    await this.WriteCommandOutputAsync(result, cancellationToken).PreserveThreadContext();

                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    this.Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Reads the command line asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the read command.
        /// </returns>
        protected virtual async Task<string> ReadCommandLineAsync(CancellationToken cancellationToken)
        {
            Func<string> consoleReadLine = this.Console.ReadLine;
            var command = await consoleReadLine.AsAsync(cancellationToken).PreserveThreadContext();
            return command;
        }

        /// <summary>
        /// Writes the welcome screen.
        /// </summary>
        protected virtual void WriteWelcomeScreen()
        {
            this.Console.WriteLine("Type 'quit' and hit <ENTER> to terminate the application.");
            this.Console.WriteLine("Type 'help' and hit <ENTER> to see a list of available commands.");
        }

        /// <summary>
        /// Writes the prompt.
        /// </summary>
        protected virtual void WritePrompt()
        {
            this.Console.Write("> ");
        }

        /// <summary>
        /// Parses the command line.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <returns>
        /// A Tuple consisting of the command and the command arguments.
        /// </returns>
        protected virtual CommandInfo ParseCommandLine(string commandLine) => new (commandLine);

        /// <summary>
        /// Writes a command output asynchronous.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task WriteCommandOutputAsync(object? result, CancellationToken cancellationToken)
        {
            var serializedResult = await this.serializationService.JsonSerializeAsync(
                                       result,
                                       ctx => ctx.Indent(true).IncludeTypeInfo(false),
                                       cancellationToken: cancellationToken).PreserveThreadContext();
            this.Console.WriteLine(serializedResult);
        }
    }
}
