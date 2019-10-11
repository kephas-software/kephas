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

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// A scripting context.
    /// </summary>
    public class ScriptingContext : Context, IScriptingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingContext"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        public ScriptingContext(ICompositionContext compositionContext)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingContext"/> class.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        public ScriptingContext(IContext executionContext)
            : base(executionContext)
        {
            Requires.NotNull(executionContext, nameof(executionContext));
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
        public IExpando Args { get; set; }

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
        public IContext ExecutionContext { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }
    }
}