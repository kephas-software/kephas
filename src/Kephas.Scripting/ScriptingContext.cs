// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Services;

    /// <summary>
    /// A scripting context.
    /// </summary>
    public class ScriptingContext : Context, IScriptingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        public ScriptingContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingContext"/> class.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        public ScriptingContext(IContext executionContext)
            : base(executionContext ?? throw new ArgumentNullException(nameof(executionContext)))
        {
        }

        /// <summary>
        /// Gets or sets the script to execute.
        /// </summary>
        /// <value>
        /// The script to execute.
        /// </value>
        public IScript Script { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IDynamic Args { get; set; }

        /// <summary>
        /// Gets or sets the script globals.
        /// </summary>
        /// <value>
        /// The script globals.
        /// </value>
        public IScriptGlobals ScriptGlobals { get; set; }

        /// <summary>
        /// Gets or sets a context for the execution.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        public IContext? ExecutionContext { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public object? Result { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Args"/> should be deconstructed.
        /// If <c>true</c>, the values in <see cref="Args"/> are globally available by their name,
        /// otherwise the arguments are available through the global Args value.
        /// </summary>
        public bool DeconstructArgs { get; set; } = true;
    }
}