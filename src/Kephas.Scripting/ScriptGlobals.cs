// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptGlobals.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the script globals class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using Kephas.Collections;
    using Kephas.Dynamic;

    /// <summary>
    /// The script globals.
    /// </summary>
    public class ScriptGlobals : Expando, IScriptGlobals
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptGlobals"/> class.
        /// </summary>
        public ScriptGlobals()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptGlobals"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="deconstruct"><c>true</c> to deconstruct the arguments, <c>false</c> otherwise.</param>
        public ScriptGlobals(IDynamic args, bool deconstruct = true)
        {
            this.SetArgs(args, deconstruct);
        }

        /// <summary>
        /// Gets the global arguments.
        /// </summary>
        /// <value>
        /// The global arguments.
        /// </value>
        public IDynamic? Args { get; private set; }

        /// <summary>
        /// Sets the arguments, optionally deconstructing them.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="deconstruct"><c>true</c> to deconstruct the arguments, <c>false</c> otherwise.</param>
        public void SetArgs(IDynamic args, bool deconstruct = true)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            if (deconstruct)
            {
                args.ToDictionary().ForEach(kv => this[kv.Key] = kv.Value);
            }

            this.Args = args;
        }
    }
}
